using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SGuard.Exceptions;

namespace SGuard;

public sealed partial class ThrowIf
{
    /// <summary>
    /// Throws an <see cref="OutOfRangeException"/> if the value is outside the range from <paramref name="min"/> to
    /// <paramref name="max"/>, and returns the value otherwise. Both bounds are inclusive, so <paramref name="min"/> and
    /// <paramref name="max"/> themselves pass.
    /// </summary>
    /// <remarks>
    /// This is the opposite of <see cref="Between{TValue, TMin, TMax}(TValue, TMin, TMax, SGuardCallback?, string?, string?, string?)"/>,
    /// which throws when the value is inside the range. A <c>NaN</c> operand fails.
    /// </remarks>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TMin">The type of the minimum bound.</typeparam>
    /// <typeparam name="TMax">The type of the maximum bound.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum bound (inclusive).</param>
    /// <param name="max">The maximum bound (inclusive).</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    /// <param name="minExpression">The expression passed as <paramref name="min"/>, captured by the compiler.</param>
    /// <param name="maxExpression">The expression passed as <paramref name="max"/>, captured by the compiler.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">An operand is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    /// <exception cref="OutOfRangeException"><paramref name="value"/> is outside the range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue OutOfRange<TValue, TMin, TMax>([NotNull] TValue value, [NotNull] TMin min, [NotNull] TMax max,
        SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null,
        [CallerArgumentExpression(nameof(min))] string? minExpression = null,
        [CallerArgumentExpression(nameof(max))] string? maxExpression = null)
        where TValue : IComparable<TMin>, IComparable<TMax>
    {
        if (SGuard.Fails(!Is.Between(value, min, max), callback))
        {
            Throw.OutOfRangeException(value, min, max, valueExpression, minExpression, maxExpression);
        }

        return value;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the value is outside the range from
    /// <paramref name="min"/> to <paramref name="max"/>, and returns the value otherwise. Both bounds are inclusive. The
    /// factory runs only when the guard fails.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TMin">The type of the minimum bound.</typeparam>
    /// <typeparam name="TMax">The type of the maximum bound.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum bound (inclusive).</param>
    /// <param name="max">The maximum bound (inclusive).</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">An operand or <paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue OutOfRange<TValue, TMin, TMax, TException>([NotNull] TValue value, [NotNull] TMin min, [NotNull] TMax max,
        Func<TException> exceptionFactory, SGuardCallback? callback = null)
        where TValue : IComparable<TMin>, IComparable<TMax> where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(!Is.Between(value, min, max), callback))
        {
            Throw.From(exceptionFactory);
        }

        return value;
    }

    /// <summary>
    /// Throws an <see cref="OutOfRangeException"/> if the string is outside the range from <paramref name="min"/> to
    /// <paramref name="max"/> under the given StringComparison, and returns the string otherwise. Both bounds are inclusive.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="min">The minimum bound (inclusive).</param>
    /// <param name="max">The maximum bound (inclusive).</param>
    /// <param name="comparison">The string comparison rule to use.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    /// <param name="minExpression">The expression passed as <paramref name="min"/>, captured by the compiler.</param>
    /// <param name="maxExpression">The expression passed as <paramref name="max"/>, captured by the compiler.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">An operand is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="min"/> sorts after <paramref name="max"/>.</exception>
    /// <exception cref="OutOfRangeException"><paramref name="value"/> is outside the range.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string OutOfRange(string value, string min, string max, StringComparison comparison, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null,
        [CallerArgumentExpression(nameof(min))] string? minExpression = null,
        [CallerArgumentExpression(nameof(max))] string? maxExpression = null)
    {
        if (SGuard.Fails(!Is.Between(value, min, max, comparison), callback))
        {
            Throw.OutOfRangeException(value, min, max, valueExpression, minExpression, maxExpression);
        }

        return value;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the string is outside the range from
    /// <paramref name="min"/> to <paramref name="max"/> under the given StringComparison, and returns the string otherwise.
    /// Both bounds are inclusive. The factory runs only when the guard fails.
    /// </summary>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="value">The string to check.</param>
    /// <param name="min">The minimum bound (inclusive).</param>
    /// <param name="max">The maximum bound (inclusive).</param>
    /// <param name="comparison">The string comparison rule to use.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">An operand or <paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="min"/> sorts after <paramref name="max"/>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string OutOfRange<TException>(string value, string min, string max, StringComparison comparison,
        Func<TException> exceptionFactory, SGuardCallback? callback = null) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(!Is.Between(value, min, max, comparison), callback))
        {
            Throw.From(exceptionFactory);
        }

        return value;
    }
}
