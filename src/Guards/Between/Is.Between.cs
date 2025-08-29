using System.Diagnostics.CodeAnalysis;

namespace SGuard;

/// <summary>
/// Provides a set of static methods to evaluate and validate conditions
/// related to comparisons of values, including checks for ranges and
/// relative orderings.
/// </summary>
public sealed partial class Is
{
    /// <summary>
    /// Throws an exception if value is less than or equal to min, or greater than or equal to max.
    /// Boundaries are exclusive.
    /// </summary>
    /// <typeparam name="TValue">The type of the value being checked.</typeparam>
    /// <typeparam name="TMin">The type of the minimum boundary value.</typeparam>
    /// <typeparam name="TMax">The type of the maximum boundary value.</typeparam>
    /// <param name="value">The value to be checked for being within the range.</param>
    /// <param name="min">The minimum value of the range.</param>
    /// <param name="max">The maximum value of the range.</param>
    /// <param name="callback">
    /// An optional callback that will be invoked with the outcome of the evaluation,
    /// indicating success or failure of the check.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if the value is greater than or equal to the minimum
    /// and less than or equal to the maximum; otherwise, <c>false</c>.
    /// </returns>
    public static bool Between<TValue, TMin, TMax>([NotNull] TValue value, [NotNull] TMin min, [NotNull] TMax max, SGuardCallback? callback = null)
        where TValue : IComparable<TMin>, IComparable<TMax>
    {
        ArgumentNullException.ThrowIfNull(min);
        ArgumentNullException.ThrowIfNull(max);
        ArgumentNullException.ThrowIfNull(value);

        var isBetween = value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;

        SGuard.InvokeCallbackSafely(isBetween, callback);

        return isBetween;
    }

    /// <summary>
    /// Throws an exception if value is less than or equal to min, or greater than or equal to max.
    /// Boundaries are exclusive using the given StringComparison.
    /// </summary>
    /// <param name="value">The string value to be checked.</param>
    /// <param name="min">The minimum boundary (inclusive).</param>
    /// <param name="max">The maximum boundary (inclusive).</param>
    /// <param name="comparison">The string comparison rule to use.</param>
    /// <param name="callback">Optional callback invoked with the outcome.</param>
    /// <returns>true if the value is between min and max (inclusive) under the specified comparison; otherwise false.</returns>
    public static bool Between(string value, string min, string max, StringComparison comparison, SGuardCallback? callback = null)
    {
        ArgumentNullException.ThrowIfNull(min);
        ArgumentNullException.ThrowIfNull(max);
        ArgumentNullException.ThrowIfNull(value);

        var isBetween = string.Compare(value, min, comparison) >= 0 && string.Compare(value, max, comparison) <= 0;

        SGuard.InvokeCallbackSafely(isBetween, callback);

        return isBetween;
    }
}