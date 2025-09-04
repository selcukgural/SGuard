using System.Collections;
using System.Linq.Expressions;

namespace SGuard.Visitor;

/// <summary>
/// An internal class that visits member and unary expressions to check for null or default values.
/// </summary>
internal sealed class NullOrEmptyVisitor : ExpressionVisitor
{
    /// <summary>
    /// Visits a member expression to build a check for whether it is null or has a default/empty value.
    /// </summary>
    /// <param name="node">The member expression to visit.</param>
    /// <returns>An expression tree that performs the null/empty/default check.</returns>
    protected override Expression VisitMember(MemberExpression node)
    {
        // Build a null-safe chain for nested member access (e.g., x.L1.L2.Inner).
        // At each level, we check if the instance is null and short-circuit to null.
        // On the final member, we apply the type-specific "empty" check.
        var nullConst = Expression.Constant(null, typeof(object));

        // Collect the member chain from root to leaf
        var members = new List<MemberExpression>();

        for (var current = node; current is not null; current = current.Expression as MemberExpression)
        {
            members.Add(current);
        }

        members.Reverse();

        // Root instance (parameter or earlier expression)
        var instance = members[0].Expression!;

        return BuildChain(0, instance);

        // Recursively build: if (instance == null) return null; else continue to next member
        Expression BuildChain(int index, Expression currentInstance)
        {
            var currentMember = members[index];
            var access = Expression.MakeMemberAccess(currentInstance, currentMember.Member);

            if (index == members.Count - 1)
            {
                // Final member: apply "null or empty" logic
                var finalCheck = BuildNullOrEmptyCheckExpression(access, access.Type);

                // Guard currentInstance for null before accessing member
                var instanceIsNull = Expression.Equal(Expression.Convert(currentInstance, typeof(object)), nullConst);
                return Expression.Condition(instanceIsNull, nullConst, finalCheck);
            }
            else
            {
                // Guard currentInstance, then go deeper with the accessed member as new instance
                var instanceIsNull = Expression.Equal(Expression.Convert(currentInstance, typeof(object)), nullConst);
                var next = BuildChain(index + 1, access);
                return Expression.Condition(instanceIsNull, nullConst, next);
            }
        }
    }

    /// <summary>
    /// Builds an expression that evaluates whether a given member access expression is null or represents an empty/default value,
    /// based on its type and specific rules for certain types such as collections, strings, and primitives.
    /// </summary>
    /// <param name="memberAccess">The expression representing the member access to check.</param>
    /// <param name="memberType">The type of the member being evaluated.</param>
    /// <returns>An expression tree that performs the null, empty, or default value check for the specified member.</returns>
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
            var countProperty = typeof(ICollection).GetProperty(nameof(ICollection.Count));
            var countAccess = Expression.Property(Expression.Convert(memberAccess, typeof(ICollection)), countProperty!);
            specificCheck = Expression.Equal(countAccess, Expression.Constant(0));
        }
        else if (ImplementsGenericInterface(memberType, typeof(IReadOnlyCollection<>), out var iroc))
        {
            var countProperty = iroc!.GetProperty(nameof(IReadOnlyCollection<object>.Count));
            var countAccess = Expression.Property(Expression.Convert(memberAccess, iroc), countProperty!);
            specificCheck = Expression.Equal(countAccess, Expression.Constant(0));
        }
        else if (typeof(IDictionary).IsAssignableFrom(memberType))
        {
            var countProperty = typeof(IDictionary).GetProperty(nameof(IDictionary.Count));
            var countAccess = Expression.Property(Expression.Convert(memberAccess, typeof(IDictionary)), countProperty!);
            specificCheck = Expression.Equal(countAccess, Expression.Constant(0));
        }
        else if (ImplementsGenericInterface(memberType, typeof(IReadOnlyDictionary<,>), out var irod))
        {
            var countProperty = irod!.GetProperty(nameof(IReadOnlyDictionary<object, object>.Count));
            var countAccess = Expression.Property(Expression.Convert(memberAccess, irod), countProperty!);
            specificCheck = Expression.Equal(countAccess, Expression.Constant(0));
        }
        else if (typeof(IEnumerable).IsAssignableFrom(memberType) && memberType != typeof(string))
        {
            var countProp = memberType.GetProperty("Count");

            if (countProp is not null && countProp.PropertyType == typeof(int) && countProp.GetMethod is not null)
            {
                var countAccess = Expression.Property(memberAccess, countProp);
                specificCheck = Expression.Equal(countAccess, Expression.Constant(0));
            }
            else
            {
                var enumerableType = typeof(IEnumerable);
                var castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast))!.MakeGenericMethod(typeof(object));

                var anyMethod = typeof(Enumerable).GetMethods().First(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 1)
                                                  .MakeGenericMethod(typeof(object));

                var asEnumerable = Expression.Convert(memberAccess, enumerableType);
                var castCall = Expression.Call(castMethod, asEnumerable);
                var anyCall = Expression.Call(anyMethod, castCall);
                specificCheck = Expression.IsFalse(anyCall);
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
        else if (memberType is { IsValueType: true, IsPrimitive: false } && memberType != typeof(decimal))
        {
            specificCheck = Expression.Equal(memberAccess, Expression.Default(memberType));
        }
        else
        {
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
    /// Visits a unary expression and converts it to an object type.
    /// </summary>
    /// <param name="node">The unary expression to visit.</param>
    /// <returns>The visited expression, properly converted to handle boxing scenarios.</returns>
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