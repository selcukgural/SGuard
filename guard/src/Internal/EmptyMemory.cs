using System.Runtime.CompilerServices;

namespace SGuard;

/// <summary>
/// Checks <see cref="Memory{T}"/> and <see cref="ReadOnlyMemory{T}"/> values of any element type for being empty,
/// without reflection or generated code, so the checks also work under trimming and Native AOT.
/// </summary>
internal static class EmptyMemory
{
    /// <summary>
    /// Determines whether <typeparamref name="T"/> is a memory type and the value has no elements. Doesn't box the value.
    /// </summary>
    /// <remarks>
    /// <see cref="Memory{T}"/> and <see cref="ReadOnlyMemory{T}"/> hold an object, an index and a length whatever their
    /// element type, so any of them can be read as a <c>ReadOnlyMemory&lt;byte&gt;</c>: the runtime converts between
    /// <see cref="Memory{T}"/> and <see cref="ReadOnlyMemory{T}"/> the same way. <c>MemoryTests</c> checks this.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is<T>(T value) => IsMemory<T>.Value && Unsafe.As<T, ReadOnlyMemory<byte>>(ref value).IsEmpty;

    /// <summary>
    /// Determines whether a boxed value is a memory type with no elements.
    /// </summary>
    /// <remarks>
    /// The element type isn't known here, so the check uses <see cref="Memory{T}.ToString"/>, which returns the characters
    /// of a <c>Memory&lt;char&gt;</c> and the type name followed by the length in brackets (<c>[0]</c>) otherwise. Only
    /// boxed memory values take this path: a memory value held by an <see cref="object"/>, an interface or a nullable.
    /// </remarks>
    public static bool IsBoxed(object value)
    {
        var type = value.GetType();

        if (!IsMemoryType(type))
        {
            return false;
        }

        var text = value.ToString() ?? string.Empty;

        return type.GetGenericArguments()[0] == typeof(char) ? text.Length == 0 : text.EndsWith("[0]", StringComparison.Ordinal);
    }

    /// <summary>
    /// Determines whether the type is <see cref="Memory{T}"/> or <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    public static bool IsMemoryType(Type type)
        => type is { IsValueType: true, IsGenericType: true } &&
           (type.GetGenericTypeDefinition() == typeof(Memory<>) || type.GetGenericTypeDefinition() == typeof(ReadOnlyMemory<>));

    /// <summary>
    /// Whether <typeparamref name="T"/> is a memory type. A static readonly field, so the JIT removes the check for other
    /// types.
    /// </summary>
    private static class IsMemory<T>
    {
        public static readonly bool Value = IsMemoryType(typeof(T));
    }
}
