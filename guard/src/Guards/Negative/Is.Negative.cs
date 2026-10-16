using System.Numerics;
using System.Runtime.CompilerServices;

namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// Determines whether the number is less than zero, and optionally invokes a callback with the result.
    /// </summary>
    /// <remarks>
    /// Like the other comparisons, <c>NaN</c> returns <c>false</c>. Negative zero (<c>-0.0</c>) equals zero and is not
    /// negative.
    /// </remarks>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <param name="value">The number to check.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if <paramref name="value"/> is less than zero; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Negative<T>(T value, SGuardCallback? callback = null) where T : INumber<T>
    {
        var isNegative = value < T.Zero;

        SGuard.InvokeCallbackSafely(isNegative, callback);

        return isNegative;
    }

    /// <summary>
    /// Determines whether the number is less than or equal to zero, and optionally invokes a callback with the result.
    /// </summary>
    /// <remarks>Like the other comparisons, <c>NaN</c> returns <c>false</c>.</remarks>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <param name="value">The number to check.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if <paramref name="value"/> is less than or equal to zero; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NegativeOrZero<T>(T value, SGuardCallback? callback = null) where T : INumber<T>
    {
        var isNegativeOrZero = value <= T.Zero;

        SGuard.InvokeCallbackSafely(isNegativeOrZero, callback);

        return isNegativeOrZero;
    }
}
