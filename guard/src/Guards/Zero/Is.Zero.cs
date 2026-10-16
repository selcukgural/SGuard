using System.Numerics;
using System.Runtime.CompilerServices;

namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// Determines whether the number is zero, and optionally invokes a callback with the result.
    /// </summary>
    /// <remarks>Negative zero (<c>-0.0</c>) is zero. <c>NaN</c> is not.</remarks>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <param name="value">The number to check.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if <paramref name="value"/> is zero; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Zero<T>(T value, SGuardCallback? callback = null) where T : INumberBase<T>
    {
        var isZero = T.IsZero(value);

        SGuard.InvokeCallbackSafely(isZero, callback);

        return isZero;
    }
}
