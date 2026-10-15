using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// Determines whether the value is <c>null</c>, and optionally invokes a callback with the result.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="NullOrEmpty{T}(T, SGuardCallback?)"/>, only <c>null</c> counts: <c>""</c>, an empty collection,
    /// <c>0</c> and other default values don't.
    /// </remarks>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if <paramref name="value"/> is <c>null</c>; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Null<T>([NotNullWhen(false)] T? value, SGuardCallback? callback = null)
    {
        var isNull = value is null;

        SGuard.InvokeCallbackSafely(isNull, callback);

        return isNull;
    }
}
