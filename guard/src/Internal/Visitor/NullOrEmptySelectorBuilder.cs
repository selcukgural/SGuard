using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace SGuard;

/// <summary>
/// Builds the body of a compiled <c>NullOrEmpty</c> selector: an expression that yields <c>null</c> when the selected value is
/// null or empty, and the value (as <see cref="object"/>) otherwise.
/// </summary>
/// <remarks>
/// A <c>null</c> on the path to the selected value, that is on the instance of a member access, an instance method call
/// (including indexers) or an array access, makes the result <c>null</c>, as C#'s <c>?.</c> would. Each value on the path
/// is evaluated once. Other expressions, such as method arguments or the operands of <c>+</c>, are evaluated as written
/// and keep their types.
/// </remarks>
internal static class NullOrEmptySelectorBuilder
{
    // Static caches to avoid repeated reflection/interface work
    private static readonly ConcurrentDictionary<Type, PropertyInfo?> CountPropertyCache = new();
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> TypePropertyCache = new();

    private static readonly Expression NullObject = Expression.Constant(null, typeof(object));

    /// <summary>
    /// The maximum number of nested complex types whose properties are inspected. Deeper complex members are only checked
    /// for null, which keeps self-referencing and recursively generic types from expanding without bound.
    /// </summary>
    internal const int MaxComplexTypeDepth = 8;

    /// <summary>
    /// Builds the null-or-empty evaluation of a selector body.
    /// </summary>
    /// <param name="body">The selector body, usually a member path converted to <see cref="object"/>.</param>
    /// <returns>An expression of type <see cref="object"/> that is <c>null</c> when the selected value is null or empty.</returns>
    public static Expression Build(Expression body)
    {
        // The compiler boxes value-typed selections with a Convert to object; check the value itself.
        if (body is UnaryExpression { NodeType: ExpressionType.Convert, Method: null } convert && convert.Type == typeof(object))
        {
            body = convert.Operand;
        }

        return Path(body, value => Evaluated(value, selected => BuildNullOrEmptyCheckExpression(selected, selected.Type, [], 0)));
    }

    /// <summary>
    /// Rebuilds a step of the path to the selected value, with a null check on its instance, and passes the rebuilt value to
    /// <paramref name="next"/>. <paramref name="next"/> returns the expression of type <see cref="object"/> to evaluate
    /// when nothing on the path is null.
    /// </summary>
    private static Expression Path(Expression node, Func<Expression, Expression> next)
    {
        switch (node)
        {
            case MemberExpression { Expression: { } instance } member:
                return Path(instance, value => NotNull(value, v => next(Expression.MakeMemberAccess(v, member.Member))));
            case MethodCallExpression { Object: { } instance } call:
                return Path(instance, value => NotNull(value, v => next(Expression.Call(v, call.Method, call.Arguments))));
            case BinaryExpression { NodeType: ExpressionType.ArrayIndex } index:
                return Path(index.Left, value => NotNull(value, v => next(Expression.ArrayIndex(v, index.Right))));
            case IndexExpression { Object: { } instance } index:
                return Path(instance, value => NotNull(value, v => next(Expression.MakeIndex(v, index.Indexer, index.Arguments))));
            case UnaryExpression { NodeType: ExpressionType.ArrayLength } length:
                return Path(length.Operand, value => NotNull(value, v => next(Expression.ArrayLength(v))));
            case UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked or ExpressionType.TypeAs } unary:
                // A cast of null is null, except to a non-nullable value type, which would throw.
                return Path(unary.Operand, value =>
                    CanBeNull(value.Type) && !CanBeNull(unary.Type)
                        ? NotNull(value, v => next(Expression.MakeUnary(unary.NodeType, v, unary.Type, unary.Method)))
                        : next(Expression.MakeUnary(unary.NodeType, value, unary.Type, unary.Method)));
            default:
                // The root of the path (the parameter, a captured value, a static member) or any other expression.
                return next(node);
        }
    }

    /// <summary>
    /// Evaluates <paramref name="value"/> once and yields <c>null</c> if it is null; otherwise, the result of
    /// <paramref name="next"/> for the evaluated value.
    /// </summary>
    private static Expression NotNull(Expression value, Func<Expression, Expression> next)
    {
        if (!CanBeNull(value.Type))
        {
            return next(value);
        }

        return Evaluated(value, v => Expression.Condition(IsNull(v), NullObject, next(v)));
    }

    /// <summary>
    /// Tests the value for null without boxing it or calling a user-defined <c>==</c> operator.
    /// </summary>
    private static Expression IsNull(Expression value)
    {
        if (!CanBeNull(value.Type))
        {
            return Expression.Constant(false);
        }

        return value.Type.IsValueType
            ? Expression.Equal(value, Expression.Constant(null, value.Type))
            : Expression.ReferenceEqual(value, Expression.Constant(null, value.Type));
    }

    /// <summary>
    /// Stores <paramref name="value"/> in a variable, unless it is already one, so that <paramref name="next"/> can use it
    /// more than once without evaluating it again.
    /// </summary>
    private static Expression Evaluated(Expression value, Func<Expression, Expression> next)
    {
        if (value is ParameterExpression)
        {
            return next(value);
        }

        var variable = Expression.Variable(value.Type);
        return Expression.Block(typeof(object), [variable], Expression.Assign(variable, value), next(variable));
    }

    private static bool CanBeNull(Type type) => !type.IsValueType || Nullable.GetUnderlyingType(type) is not null;

    /// <summary>
    /// Builds an expression to check if a member is null, empty, or default for its type.
    /// </summary>
    /// <param name="memberAccess">The expression representing the member to be checked.</param>
    /// <param name="memberType">The type of the member being checked.</param>
    /// <param name="complexTypePath">The complex types whose properties are being inspected on the current path.</param>
    /// <param name="complexTypeDepth">The number of complex types on the current path.</param>
    /// <returns>
    /// An expression that evaluates whether the member is null, empty, or holds a default value
    /// for its type. The method uses static caches to optimize reflection-based checks.
    /// </returns>
    private static Expression BuildNullOrEmptyCheckExpression(Expression memberAccess, Type memberType, HashSet<Type> complexTypePath,
                                                              int complexTypeDepth)
    {
        var memberAsObject = Expression.Convert(memberAccess, typeof(object));
        var isNull = IsNull(memberAccess);

        Expression specificCheck;

        if (memberType == typeof(string))
        {
            var method = typeof(string).GetMethod(nameof(string.IsNullOrEmpty), [typeof(string)]);
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
        else if (ImplementsGenericInterface(memberType, typeof(IReadOnlyCollection<>), out var roCollection))
        {
            var countProp = CountPropertyCache.GetOrAdd(roCollection!, t => t.GetProperty(nameof(IReadOnlyCollection<object>.Count)));
            specificCheck = Expression.Equal(Expression.Property(Expression.Convert(memberAccess, roCollection!), countProp!), Expression.Constant(0));
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
            specificCheck = Expression.Equal(Expression.Property(Expression.Convert(memberAccess, irod!), countProp!), Expression.Constant(0));
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
                var disposeMethod = typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose));
                var disposeIfNeeded = Expression.IfThen(Expression.TypeIs(enumeratorVar, typeof(IDisposable)),
                                                        Expression.Call(Expression.Convert(enumeratorVar, typeof(IDisposable)), disposeMethod!));
                var block = Expression.Block([enumeratorVar], assignEnum, Expression.TryFinally(Expression.IsFalse(moveNextCall), disposeIfNeeded));
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
            // A type already being inspected on this path (a cycle) or nested too deeply is only checked for null.
            if (complexTypeDepth >= MaxComplexTypeDepth || !complexTypePath.Add(memberType))
            {
                specificCheck = Expression.Constant(false);
            }
            else
            {
                // Complex type: all readable, non-indexed properties are null/empty
                var props = TypePropertyCache.GetOrAdd(memberType, GetInspectableProperties);
                Expression? allNullOrEmpty = null;

                foreach (var prop in props)
                {
                    var propAccess = Expression.Property(memberAccess, prop);
                    var propCheck = Evaluated(propAccess, value => BuildNullOrEmptyCheckExpression(value, prop.PropertyType, complexTypePath,
                                                                                               complexTypeDepth + 1));
                    // propCheck is null when the property is null or empty.
                    var condition = Expression.ReferenceEqual(propCheck, NullObject);
                    allNullOrEmpty = allNullOrEmpty is null ? condition : Expression.AndAlso(allNullOrEmpty, condition);
                }

                complexTypePath.Remove(memberType);
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

        static PropertyInfo[] GetInspectableProperties(Type type)
            => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                   .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                   .ToArray();

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
}
