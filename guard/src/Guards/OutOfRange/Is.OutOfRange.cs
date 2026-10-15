using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// Determines whether the value is outside the range from <paramref name="min"/> to <paramref name="max"/>, the
    /// opposite of <see cref="Between{TValue, TMin, TMax}(TValue, TMin, TMax, SGuardCallback?)"/>. Both bounds are
    /// inclusive, so <paramref name="min"/> and <paramref name="max"/> themselves are inside the range.
    /// </summary>
    /// <remarks>A <c>NaN</c> operand is outside every range.</remarks>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TMin">The type of the minimum bound.</typeparam>
    /// <typeparam name="TMax">The type of the maximum bound.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum bound (inclusive).</param>
    /// <param name="max">The maximum bound (inclusive).</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if <paramref name="value"/> is less than <paramref name="min"/> or greater than
    /// <paramref name="max"/>; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">An operand is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool OutOfRange<TValue, TMin, TMax>([NotNull] TValue value, [NotNull] TMin min, [NotNull] TMax max,
        SGuardCallback? callback = null)
        where TValue : IComparable<TMin>, IComparable<TMax>
    {
        var isOutOfRange = !Between(value, min, max);

        SGuard.InvokeCallbackSafely(isOutOfRange, callback);

        return isOutOfRange;
    }

    /// <summary>
    /// Determines whether the string is outside the range from <paramref name="min"/> to <paramref name="max"/> under the
    /// given StringComparison. Both bounds are inclusive.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="min">The minimum bound (inclusive).</param>
    /// <param name="max">The maximum bound (inclusive).</param>
    /// <param name="comparison">The string comparison rule to use.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if <paramref name="value"/> sorts before <paramref name="min"/> or after
    /// <paramref name="max"/>; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">An operand is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="min"/> sorts after <paramref name="max"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool OutOfRange(string value, string min, string max, StringComparison comparison, SGuardCallback? callback = null)
    {
        var isOutOfRange = !Between(value, min, max, comparison);

        SGuard.InvokeCallbackSafely(isOutOfRange, callback);

        return isOutOfRange;
    }
}
