using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace SGuard;

/// <summary>
/// Builds the body of a compiled <c>NullOrEmpty</c> selector: a <see cref="bool"/> expression that is <c>true</c> when the
/// selected value is null or empty.
/// </summary>
/// <remarks>
/// A <c>null</c> on the path to the selected value, that is on the instance of a member access, an instance method call
/// (including indexers) or an array access, counts as empty, as C#'s <c>?.</c> would yield null. Each value on the path is
/// evaluated once. Other expressions, such as method arguments or the operands of <c>+</c>, are evaluated as written and
/// keep their types.
/// </remarks>
[RequiresUnreferencedCode(AotMessages.Selector)]
[RequiresDynamicCode(AotMessages.Selector)]
internal static class NullOrEmptySelectorBuilder
{
    // Static caches to avoid repeated reflection/interface work
    private static readonly ConcurrentDictionary<Type, PropertyInfo> CountPropertyCache = new();
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> TypePropertyCache = new();

    private static readonly Expression True = Expression.Constant(true);

    private static readonly MethodInfo IsNullOrEmptyAtRunTime =
        typeof(Is).GetMethod(nameof(Is.InternalIsNullOrEmpty), BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(typeof(object));

    /// <summary>
    /// The maximum number of nested complex types whose properties are inspected. Deeper complex members are only checked
    /// for null, which keeps self-referencing and recursively generic types from expanding without bound.
    /// </summary>
    internal const int MaxComplexTypeDepth = 8;

    /// <summary>
    /// Builds the null-or-empty evaluation of a selector body.
    /// </summary>
    /// <param name="body">The selector body, usually a member path converted to <see cref="object"/>.</param>
    /// <returns>An expression of type <see cref="bool"/> that is <c>true</c> when the selected value is null or empty.</returns>
    public static Expression Build(Expression body)
    {
        // The compiler boxes value-typed selections with a Convert to object; check the value itself.
        if (body is UnaryExpression { NodeType: ExpressionType.Convert, Method: null } convert && convert.Type == typeof(object))
        {
            body = convert.Operand;
        }

        return Path(body, value => Evaluated(value, selected => IsNullOrEmpty(selected, [], 0)));
    }

    /// <summary>
    /// Rebuilds a step of the path to the selected value, with a null check on its instance, and passes the rebuilt value to
    /// <paramref name="next"/>. <paramref name="next"/> returns the <see cref="bool"/> expression to evaluate when nothing
    /// on the path is null.
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
    /// Evaluates <paramref name="value"/> once and yields <c>true</c> if it is null; otherwise, the result of
    /// <paramref name="next"/> for the evaluated value.
    /// </summary>
    private static Expression NotNull(Expression value, Func<Expression, Expression> next)
    {
        if (!CanBeNull(value.Type))
        {
            return next(value);
        }

        return Evaluated(value, v => Expression.Condition(IsNull(v), True, next(v)));
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
        var result = next(variable);
        return Expression.Block(result.Type, [variable], Expression.Assign(variable, value), result);
    }

    private static bool CanBeNull(Type type) => !type.IsValueType || Nullable.GetUnderlyingType(type) is not null;

    /// <summary>
    /// Builds a <see cref="bool"/> expression that is <c>true</c> when the value is null, empty, or the default value of its
    /// type.
    /// </summary>
    /// <param name="value">The value to check. It is read more than once, so it must be a variable or a parameter.</param>
    /// <param name="complexTypePath">The complex types whose properties are being inspected on the current path.</param>
    /// <param name="complexTypeDepth">The number of complex types on the current path.</param>
    private static Expression IsNullOrEmpty(Expression value, HashSet<Type> complexTypePath, int complexTypeDepth)
    {
        var isNull = IsNull(value);
        var isEmpty = IsEmpty(value, complexTypePath, complexTypeDepth);

        if (isEmpty is null)
        {
            return isNull;
        }

        return CanBeNull(value.Type) ? Expression.OrElse(isNull, isEmpty) : isEmpty;
    }

    /// <summary>
    /// Builds the emptiness check that applies to the value's type once it is known not to be null, or returns <c>null</c>
    /// when only a null check applies.
    /// </summary>
    private static Expression? IsEmpty(Expression value, HashSet<Type> complexTypePath, int complexTypeDepth)
    {
        var type = value.Type;

        if (type == typeof(string))
        {
            return Expression.Equal(Expression.Property(value, nameof(string.Length)), Expression.Constant(0));
        }

        if (Nullable.GetUnderlyingType(type) is not null)
        {
            // Checked after the null check, so the value is there: apply the rules of the underlying type to it.
            return Evaluated(Expression.Call(value, nameof(Nullable<int>.GetValueOrDefault), Type.EmptyTypes),
                             v => IsEmpty(v, complexTypePath, complexTypeDepth) ?? Expression.Constant(false));
        }

        if (type == typeof(object) || type.IsInterface)
        {
            // Only the run-time type tells whether such a value is a string, a collection or a boxed struct (a default
            // ImmutableArray<T> behind IReadOnlyList<T> throws from Count), so the rules for that type are applied. Inside a
            // complex type, other than collection interfaces these members are only checked for null.
            return complexTypeDepth == 0 || typeof(IEnumerable).IsAssignableFrom(type) ? AtRunTime(value) : null;
        }

        if (type is { IsValueType: true, IsGenericType: true } && type.GetGenericTypeDefinition() == typeof(ImmutableArray<>))
        {
            // A default ImmutableArray<T> throws from Count, so it is checked with IsDefaultOrEmpty instead.
            return Expression.Property(value, nameof(ImmutableArray<int>.IsDefaultOrEmpty));
        }

        if (type.IsArray)
        {
            return Expression.Equal(Expression.Property(value, nameof(Array.Length)), Expression.Constant(0));
        }

        if (typeof(ICollection).IsAssignableFrom(type))
        {
            return IsZero(Expression.Property(Expression.Convert(value, typeof(ICollection)), nameof(ICollection.Count)));
        }

        if (ImplementsGenericInterface(type, typeof(IReadOnlyCollection<>), out var readOnlyCollection))
        {
            var countProperty = CountPropertyCache.GetOrAdd(readOnlyCollection, t => t.GetProperty(nameof(IReadOnlyCollection<object>.Count))!);
            return IsZero(Expression.Property(Expression.Convert(value, readOnlyCollection), countProperty));
        }

        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            var enumerator = Expression.Variable(typeof(IEnumerator), "enumerator");
            var getEnumerator = Expression.Call(Expression.Convert(value, typeof(IEnumerable)), typeof(IEnumerable).GetMethod(nameof(IEnumerable.GetEnumerator))!);
            var moveNext = Expression.Call(enumerator, typeof(IEnumerator).GetMethod(nameof(IEnumerator.MoveNext))!);
            var dispose = Expression.IfThen(Expression.TypeIs(enumerator, typeof(IDisposable)),
                                            Expression.Call(Expression.Convert(enumerator, typeof(IDisposable)),
                                                            typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose))!));

            return Expression.Block([enumerator], Expression.Assign(enumerator, getEnumerator), Expression.TryFinally(Expression.IsFalse(moveNext), dispose));
        }

        if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(sbyte) || type == typeof(byte) ||
            type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong) || type == typeof(float) || type == typeof(double) ||
            type == typeof(decimal) || type == typeof(bool) || type == typeof(Guid) || type == typeof(DateOnly))
        {
            return Expression.Equal(value, Expression.Default(type));
        }

        if (type == typeof(DateTime) || type == typeof(TimeSpan) || type == typeof(TimeOnly) || type == typeof(DateTimeOffset))
        {
            return Expression.Equal(Expression.Property(value, nameof(DateTime.Ticks)), Expression.Constant(0L));
        }

        if (EmptyMemory.IsMemoryType(type))
        {
            return Expression.Property(value, nameof(Memory<int>.IsEmpty));
        }

        if (type.IsValueType)
        {
            // Any other struct, enum or primitive (char, nint, ...) is empty when it is its default value. Compared with
            // EqualityComparer<T>.Default, like the check without a selector, because most structs have no == operator.
            var comparer = typeof(EqualityComparer<>).MakeGenericType(type);

            return Expression.Call(Expression.Property(null, comparer, nameof(EqualityComparer<int>.Default)),
                                   comparer.GetMethod(nameof(EqualityComparer<int>.Equals), [type, type])!, value, Expression.Default(type));
        }

        if (IsFrameworkType(type))
        {
            // Framework classes that are not collections (Uri, Version, Type, ...) are only checked for null: their
            // properties are not data to validate, and some of them throw, such as Uri.AbsolutePath on a relative URI.
            return AtRunTimeIfDerivable(value, null, complexTypeDepth);
        }

        // A type already being inspected on this path (a cycle) or nested too deeply is only checked for null.
        if (complexTypeDepth >= MaxComplexTypeDepth || !complexTypePath.Add(type))
        {
            return null;
        }

        // Complex type: empty when all readable, non-indexed properties are null or empty.
        Expression? allNullOrEmpty = null;

        foreach (var property in TypePropertyCache.GetOrAdd(type, GetInspectableProperties))
        {
            var propertyIsNullOrEmpty = Evaluated(Expression.Property(value, property),
                                                  v => IsNullOrEmpty(v, complexTypePath, complexTypeDepth + 1));

            allNullOrEmpty = allNullOrEmpty is null ? propertyIsNullOrEmpty : Expression.AndAlso(allNullOrEmpty, propertyIsNullOrEmpty);
        }

        complexTypePath.Remove(type);

        return AtRunTimeIfDerivable(value, allNullOrEmpty, complexTypeDepth);

        static Expression IsZero(Expression count) => Expression.Equal(count, Expression.Constant(0));

        // The rules for the run-time type of the value, which applies them without inspecting properties.
        static Expression AtRunTime(Expression value) => Expression.Call(IsNullOrEmptyAtRunTime, Expression.Convert(value, typeof(object)));

        // The selected value of a class that isn't sealed may be of a derived type that is a collection, so the rules for
        // its run-time type are applied as well.
        static Expression? AtRunTimeIfDerivable(Expression value, Expression? isEmpty, int complexTypeDepth)
        {
            if (complexTypeDepth > 0 || value.Type.IsSealed)
            {
                return isEmpty;
            }

            return isEmpty is null ? AtRunTime(value) : Expression.OrElse(isEmpty, AtRunTime(value));
        }

        static PropertyInfo[] GetInspectableProperties(Type type)
            => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                   .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                   .ToArray();

        static bool IsFrameworkType(Type type) => type.Namespace is { } ns && (ns == "System" || ns.StartsWith("System.", StringComparison.Ordinal));

        static bool ImplementsGenericInterface(Type type, Type openGeneric, [NotNullWhen(true)] out Type? implemented)
        {
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
