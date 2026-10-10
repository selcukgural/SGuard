using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace SGuard;

/// <summary>
/// Checks <see cref="Memory{T}"/> and <see cref="ReadOnlyMemory{T}"/> values of any element type for being empty.
/// </summary>
internal static class EmptyMemory
{
    // The IsEmpty check of each boxed type seen, or null when the type is not a memory type.
    private static readonly ConcurrentDictionary<Type, Func<object, bool>?> BoxedChecks = new();

    /// <summary>
    /// Determines whether <typeparamref name="T"/> is a memory type and the value has no elements. Doesn't box the value.
    /// </summary>
    public static bool Is<T>(T value) => Check<T>.IsEmpty is { } isEmpty && isEmpty(value);

    /// <summary>
    /// Determines whether a boxed value is a memory type with no elements.
    /// </summary>
    public static bool IsBoxed(object value) => BoxedChecks.GetOrAdd(value.GetType(), Create<object>) is { } isEmpty && isEmpty(value);

    /// <summary>
    /// Determines whether the type is <see cref="Memory{T}"/> or <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    public static bool IsMemoryType(Type type)
        => type is { IsValueType: true, IsGenericType: true } &&
           (type.GetGenericTypeDefinition() == typeof(Memory<>) || type.GetGenericTypeDefinition() == typeof(ReadOnlyMemory<>));

    private static Func<TArg, bool>? Create<TArg>(Type type)
    {
        if (!IsMemoryType(type))
        {
            return null;
        }

        var value = Expression.Parameter(typeof(TArg), "value");
        return Expression.Lambda<Func<TArg, bool>>(Expression.Property(Expression.Convert(value, type), nameof(Memory<int>.IsEmpty)), value)
                         .Compile();
    }

    /// <summary>
    /// The IsEmpty check of <typeparamref name="T"/>, or <c>null</c> when it is not a memory type. A static readonly field,
    /// so the JIT removes the check for other types.
    /// </summary>
    private static class Check<T>
    {
        public static readonly Func<T, bool>? IsEmpty = Create<T>(typeof(T));
    }
}
