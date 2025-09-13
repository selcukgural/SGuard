using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace SGuard;

/// <summary>
/// An internal class that visits member and unary expressions to check for null, empty, or default values.
/// </summary>
/// <remarks>
/// This class is optimized for performance by leveraging:
/// - Type-level reflection caching to avoid redundant reflection calls.
/// - Fast-path checks for common .NET types (e.g., string, arrays, collections).
/// - Avoidance of LINQ in performance-critical paths.
/// 
/// The visitor traverses expression trees and generates expressions that evaluate whether
/// a given member or unary expression is null, empty, or holds a default value for its type.
/// </remarks>
internal sealed class NullOrEmptyVisitor : ExpressionVisitor
{
    // Static caches to avoid repeated reflection/interface work
    private static readonly ConcurrentDictionary<Type, PropertyInfo?> CountPropertyCache = new();
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> TypePropertyCache = new();

    protected override Expression VisitMember(MemberExpression node)
    {
        var nullConst = Expression.Constant(null, typeof(object));
        var members = new List<MemberExpression>();

        for (var current = node; current is not null; current = current.Expression as MemberExpression)
            members.Add(current);
        members.Reverse();

        var instance = members[0].Expression!;
        return BuildChain(0, instance);

        Expression BuildChain(int index, Expression currentInstance)
        {
            var currentMember = members[index];
            var access = Expression.MakeMemberAccess(currentInstance, currentMember.Member);

            if (index == members.Count - 1)
            {
                var finalCheck = BuildNullOrEmptyCheckExpression(access, access.Type);
                var instanceIsNull = Expression.Equal(Expression.Convert(currentInstance, typeof(object)), nullConst);
                return Expression.Condition(instanceIsNull, nullConst, finalCheck);
            }
            else
            {
                var instanceIsNull = Expression.Equal(Expression.Convert(currentInstance, typeof(object)), nullConst);
                var next = BuildChain(index + 1, access);
                return Expression.Condition(instanceIsNull, nullConst, next);
            }
        }
    }

    /// <summary>
    /// Builds an expression to check if a member is null, empty, or default for its type.
    /// </summary>
    /// <param name="memberAccess">The expression representing the member to be checked.</param>
    /// <param name="memberType">The type of the member being checked.</param>
    /// <returns>
    /// An expression that evaluates whether the member is null, empty, or holds a default value
    /// for its type. The method uses static caches to optimize reflection-based checks.
    /// </returns>
    private static Expression BuildNullOrEmptyCheckExpression(Expression memberAccess, Type memberType)
    {
        var memberAsObject = Expression.Convert(memberAccess, typeof(object));
        var isNull = Expression.Equal(memberAsObject, Expression.Constant(null, typeof(object)));

        Expression specificCheck;

        if (memberType == typeof(string))
        {
            var method = typeof(string).GetMethod(nameof(string.IsNullOrEmpty), new[] { typeof(string) });
            specificCheck = Expression.Call(method!, memberAccess);
        }
        else if (memberType.IsArray)
        {
            var lengthProp = memberType.GetProperty(nameof(Array.Length));
            specificCheck = Expression.Equal(Expression.Property(memberAccess, lengthProp!), Expression.Constant(0));
        }
        else if (typeof(ICollection).IsAssignableFrom(memberType))
        {
            var countProp = typeof(ICollection).GetProperty(nameof(ICollection.Count));

            specificCheck = Expression.Equal(Expression.Property(Expression.Convert(memberAccess, typeof(ICollection)), countProp!),
                                             Expression.Constant(0));
        }
        else if (ImplementsGenericInterface(memberType, typeof(IReadOnlyCollection<>), out var iroc))
        {
            var countProp = CountPropertyCache.GetOrAdd(iroc!, t => t.GetProperty(nameof(IReadOnlyCollection<object>.Count)));
            specificCheck = Expression.Equal(Expression.Property(Expression.Convert(memberAccess, iroc), countProp!), Expression.Constant(0));
        }
        else if (typeof(IDictionary).IsAssignableFrom(memberType))
        {
            var countProp = typeof(IDictionary).GetProperty(nameof(IDictionary.Count));

            specificCheck = Expression.Equal(Expression.Property(Expression.Convert(memberAccess, typeof(IDictionary)), countProp!),
                                             Expression.Constant(0));
        }
        else if (ImplementsGenericInterface(memberType, typeof(IReadOnlyDictionary<,>), out var irod))
        {
            var countProp = CountPropertyCache.GetOrAdd(irod!, t => t.GetProperty(nameof(IReadOnlyDictionary<object, object>.Count)));
            specificCheck = Expression.Equal(Expression.Property(Expression.Convert(memberAccess, irod), countProp!), Expression.Constant(0));
        }
        else if (typeof(IEnumerable).IsAssignableFrom(memberType) && memberType != typeof(string))
        {
            if (CountPropertyCache.TryGetValue(memberType, out var cachedCountProp) && cachedCountProp is not null)
            {
                specificCheck = Expression.Equal(Expression.Property(memberAccess, cachedCountProp), Expression.Constant(0));
            }
            else
            {
                var getEnumeratorMethod = typeof(IEnumerable).GetMethod(nameof(IEnumerable.GetEnumerator));
                var moveNextMethod = typeof(IEnumerator).GetMethod(nameof(IEnumerator.MoveNext));
                var enumeratorVar = Expression.Variable(typeof(IEnumerator), "enum");

                var assignEnum =
                    Expression.Assign(enumeratorVar, Expression.Call(Expression.Convert(memberAccess, typeof(IEnumerable)), getEnumeratorMethod!));
                var moveNextCall = Expression.Call(enumeratorVar, moveNextMethod!);
                var block = Expression.Block(new[] { enumeratorVar }, assignEnum, Expression.IsFalse(moveNextCall));
                specificCheck = block;
            }
        }
        else if (memberType == typeof(int))
        {
            specificCheck = Expression.Equal(memberAccess, Expression.Constant(0));
        }
        else if (memberType == typeof(long))
        {
            specificCheck = Expression.Equal(memberAccess, Expression.Constant(0L));
        }
        else if (memberType == typeof(short))
        {
            specificCheck = Expression.Equal(memberAccess, Expression.Constant((short)0));
        }
        else if (memberType == typeof(sbyte))
        {
            specificCheck = Expression.Equal(memberAccess, Expression.Constant((sbyte)0));
        }
        else if (memberType == typeof(byte))
        {
            specificCheck = Expression.Equal(memberAccess, Expression.Constant((byte)0));
        }
        else if (memberType == typeof(ushort))
        {
            specificCheck = Expression.Equal(memberAccess, Expression.Constant((ushort)0));
        }
        else if (memberType == typeof(uint))
        {
            specificCheck = Expression.Equal(memberAccess, Expression.Constant(0u));
        }
        else if (memberType == typeof(ulong))
        {
            specificCheck = Expression.Equal(memberAccess, Expression.Constant(0ul));
        }
        else if (memberType == typeof(float))
        {
            specificCheck = Expression.Equal(memberAccess, Expression.Constant(0f));
        }
        else if (memberType == typeof(double))
        {
            specificCheck = Expression.Equal(memberAccess, Expression.Constant(0d));
        }
        else if (memberType == typeof(decimal))
        {
            specificCheck = Expression.Equal(memberAccess, Expression.Constant(0m));
        }
        else if (memberType == typeof(bool))
        {
            specificCheck = Expression.Equal(memberAccess, Expression.Constant(false));
        }
        else if (memberType == typeof(Guid))
        {
            specificCheck = Expression.Equal(memberAccess, Expression.Constant(Guid.Empty));
        }
        else if (memberType == typeof(DateTime))
        {
            specificCheck = Expression.Equal(Expression.Property(memberAccess, nameof(DateTime.Ticks)), Expression.Constant(0L));
        }
        else if (memberType == typeof(TimeSpan))
        {
            specificCheck = Expression.Equal(Expression.Property(memberAccess, nameof(TimeSpan.Ticks)), Expression.Constant(0L));
        }
        else if (memberType == typeof(DateOnly))
        {
            specificCheck = Expression.Equal(memberAccess, Expression.Constant(DateOnly.MinValue));
        }
        else if (memberType == typeof(TimeOnly))
        {
            specificCheck = Expression.Equal(Expression.Property(memberAccess, nameof(TimeOnly.Ticks)), Expression.Constant(0L));
        }
        else if (memberType == typeof(DateTimeOffset))
        {
            specificCheck = Expression.Equal(Expression.Property(memberAccess, nameof(DateTimeOffset.Ticks)), Expression.Constant(0L));
        }
        else if (memberType is { IsValueType: true, IsPrimitive: false } && memberType != typeof(decimal))
        {
            specificCheck = Expression.Equal(memberAccess, Expression.Default(memberType));
        }
        else if (memberType.IsClass || memberType.IsValueType)
        {
            // Complex type: all readable properties are null/empty
            var props = TypePropertyCache.GetOrAdd(memberType, t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));

            if (props.Length == 0)
            {
                specificCheck = Expression.Constant(false);
            }
            else
            {
                Expression? allNullOrEmpty = null;

                foreach (var prop in props.Where(e => e.CanRead))
                {
                    var propAccess = Expression.Property(memberAccess, prop);
                    var propCheck = BuildNullOrEmptyCheckExpression(propAccess, prop.PropertyType);
                    var isNull2 = Expression.Equal(Expression.Convert(propAccess, typeof(object)), Expression.Constant(null));
                    var condition = Expression.OrElse(isNull2, Expression.Equal(propCheck, Expression.Constant(null, typeof(object))));
                    allNullOrEmpty = allNullOrEmpty is null ? condition : Expression.AndAlso(allNullOrEmpty, condition);
                }

                specificCheck = allNullOrEmpty ?? Expression.Constant(false);
            }
        }
        else
        {
            // Fallback: just check null
            return memberAsObject;
        }

        var combinedCheck = Expression.OrElse(isNull, specificCheck);
        return Expression.Condition(combinedCheck, Expression.Constant(null, typeof(object)), memberAsObject);

        static bool ImplementsGenericInterface(Type type, Type openGeneric, out Type? implemented)
        {
            implemented = null;

            if (type is { IsInterface: true, IsGenericType: true } && type.GetGenericTypeDefinition() == openGeneric)
            {
                implemented = type;
                return true;
            }

            implemented = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGeneric);
            return implemented is not null;
        }
    }

    /// <summary>
    /// Visits a unary expression and converts it to an object type if necessary.
    /// </summary>
    /// <param name="node">The unary expression to visit.</param>
    /// <returns>
    /// An expression of a type object. If the operand of the unary expression is already of type object,
    /// it is returned as-is; otherwise, it is converted to type object.
    /// </returns>
    protected override Expression VisitUnary(UnaryExpression node)
    {
        if (node.NodeType == ExpressionType.Convert && node.Type == typeof(object))
        {
            var visited = Visit(node.Operand);
            return visited.Type == typeof(object) ? visited : Expression.Convert(visited, typeof(object));
        }

        var visitedOperand = Visit(node.Operand);
        return Expression.Convert(visitedOperand, typeof(object));
    }
}