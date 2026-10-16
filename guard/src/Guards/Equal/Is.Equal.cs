using System.Runtime.CompilerServices;

namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// Determines whether two values are equal using <see cref="EqualityComparer{T}.Default"/>, and optionally invokes a
    /// callback with the result.
    /// </summary>
    /// <remarks>
    /// <c>null</c> operands are allowed: two <c>null</c> values are equal. Like <see cref="double.Equals(double)"/>,
    /// <c>NaN</c> equals <c>NaN</c>.
    /// </remarks>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="lValue">The first value.</param>
    /// <param name="rValue">The second value.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if the values are equal; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal<T>(T lValue, T rValue, SGuardCallback? callback = null)
    {
        var isEqual = EqualityComparer<T>.Default.Equals(lValue, rValue);

        SGuard.InvokeCallbackSafely(isEqual, callback);

        return isEqual;
    }

    /// <summary>
    /// Determines whether two strings are equal under the given StringComparison, and optionally invokes a callback with
    /// the result. <c>null</c> operands are allowed: two <c>null</c> strings are equal.
    /// </summary>
    /// <param name="lValue">The first string.</param>
    /// <param name="rValue">The second string.</param>
    /// <param name="comparison">The string comparison rule to use.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if the strings are equal; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equal(string? lValue, string? rValue, StringComparison comparison, SGuardCallback? callback = null)
    {
        var isEqual = string.Equals(lValue, rValue, comparison);

        SGuard.InvokeCallbackSafely(isEqual, callback);

        return isEqual;
    }

    /// <summary>
    /// Determines whether two values are not equal using <see cref="EqualityComparer{T}.Default"/>, and optionally invokes
    /// a callback with the result.
    /// </summary>
    /// <remarks><c>null</c> operands are allowed: two <c>null</c> values are equal.</remarks>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="lValue">The first value.</param>
    /// <param name="rValue">The second value.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if the values are not equal; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotEqual<T>(T lValue, T rValue, SGuardCallback? callback = null)
    {
        var isNotEqual = !EqualityComparer<T>.Default.Equals(lValue, rValue);

        SGuard.InvokeCallbackSafely(isNotEqual, callback);

        return isNotEqual;
    }

    /// <summary>
    /// Determines whether two strings are not equal under the given StringComparison, and optionally invokes a callback
    /// with the result. <c>null</c> operands are allowed: two <c>null</c> strings are equal.
    /// </summary>
    /// <param name="lValue">The first string.</param>
    /// <param name="rValue">The second string.</param>
    /// <param name="comparison">The string comparison rule to use.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if the strings are not equal; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotEqual(string? lValue, string? rValue, StringComparison comparison, SGuardCallback? callback = null)
    {
        var isNotEqual = !string.Equals(lValue, rValue, comparison);

        SGuard.InvokeCallbackSafely(isNotEqual, callback);

        return isNotEqual;
    }
}
