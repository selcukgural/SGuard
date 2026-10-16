using System.Runtime.CompilerServices;

namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// Determines whether the string has more than <paramref name="maxLength"/> characters, and optionally invokes a
    /// callback with the result.
    /// </summary>
    /// <remarks>
    /// The length is <see cref="string.Length"/>, the number of UTF-16 code units: an emoji outside the Basic Multilingual
    /// Plane counts as two.
    /// </remarks>
    /// <param name="value">The string to check.</param>
    /// <param name="maxLength">The maximum number of characters allowed.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if <paramref name="value"/> is longer than <paramref name="maxLength"/>; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxLength"/> is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LongerThan(string value, int maxLength, SGuardCallback? callback = null)
    {
        SGuard.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegative(maxLength);

        var isLonger = value.Length > maxLength;

        SGuard.InvokeCallbackSafely(isLonger, callback);

        return isLonger;
    }

    /// <summary>
    /// Determines whether the string has fewer than <paramref name="minLength"/> characters, and optionally invokes a
    /// callback with the result.
    /// </summary>
    /// <remarks>
    /// The length is <see cref="string.Length"/>, the number of UTF-16 code units: an emoji outside the Basic Multilingual
    /// Plane counts as two.
    /// </remarks>
    /// <param name="value">The string to check.</param>
    /// <param name="minLength">The minimum number of characters required.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if <paramref name="value"/> is shorter than <paramref name="minLength"/>; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minLength"/> is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ShorterThan(string value, int minLength, SGuardCallback? callback = null)
    {
        SGuard.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegative(minLength);

        var isShorter = value.Length < minLength;

        SGuard.InvokeCallbackSafely(isShorter, callback);

        return isShorter;
    }
}
