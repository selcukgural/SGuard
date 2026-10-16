using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SGuard.Exceptions;

namespace SGuard;

public sealed partial class ThrowIf
{
    /// <summary>
    /// Throws a <see cref="LongerThanException"/> if the string has more than <paramref name="maxLength"/> characters, and
    /// returns the string otherwise.
    /// </summary>
    /// <remarks>
    /// The length is <see cref="string.Length"/>, the number of UTF-16 code units: an emoji outside the Basic Multilingual
    /// Plane counts as two. The exception message never contains the string.
    /// </remarks>
    /// <param name="value">The string to check.</param>
    /// <param name="maxLength">The maximum number of characters allowed.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    /// <param name="maxLengthExpression">The expression passed as <paramref name="maxLength"/>, captured by the compiler.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxLength"/> is negative.</exception>
    /// <exception cref="LongerThanException"><paramref name="value"/> is longer than <paramref name="maxLength"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string LongerThan([NotNull] string value, int maxLength, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null,
        [CallerArgumentExpression(nameof(maxLength))] string? maxLengthExpression = null)
    {
        SGuard.ThrowIfNull(value, valueExpression);
        ArgumentOutOfRangeException.ThrowIfNegative(maxLength);

        if (SGuard.Fails(value.Length > maxLength, callback))
        {
            Throw.LongerThanException(value.Length, maxLength, valueExpression, maxLengthExpression);
        }

        return value;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the string has more than
    /// <paramref name="maxLength"/> characters, and returns the string otherwise. The factory runs only when the guard fails.
    /// </summary>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="value">The string to check.</param>
    /// <param name="maxLength">The maximum number of characters allowed.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> or <paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxLength"/> is negative.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string LongerThan<TException>([NotNull] string value, int maxLength, Func<TException> exceptionFactory,
        SGuardCallback? callback = null) where TException : Exception
    {
        SGuard.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegative(maxLength);
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(value.Length > maxLength, callback))
        {
            Throw.From(exceptionFactory);
        }

        return value;
    }

    /// <summary>
    /// Throws a <see cref="ShorterThanException"/> if the string has fewer than <paramref name="minLength"/> characters,
    /// and returns the string otherwise.
    /// </summary>
    /// <remarks>
    /// The length is <see cref="string.Length"/>, the number of UTF-16 code units: an emoji outside the Basic Multilingual
    /// Plane counts as two. The exception message never contains the string.
    /// </remarks>
    /// <param name="value">The string to check.</param>
    /// <param name="minLength">The minimum number of characters required.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    /// <param name="minLengthExpression">The expression passed as <paramref name="minLength"/>, captured by the compiler.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minLength"/> is negative.</exception>
    /// <exception cref="ShorterThanException"><paramref name="value"/> is shorter than <paramref name="minLength"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ShorterThan([NotNull] string value, int minLength, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null,
        [CallerArgumentExpression(nameof(minLength))] string? minLengthExpression = null)
    {
        SGuard.ThrowIfNull(value, valueExpression);
        ArgumentOutOfRangeException.ThrowIfNegative(minLength);

        if (SGuard.Fails(value.Length < minLength, callback))
        {
            Throw.ShorterThanException(value.Length, minLength, valueExpression, minLengthExpression);
        }

        return value;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the string has fewer than
    /// <paramref name="minLength"/> characters, and returns the string otherwise. The factory runs only when the guard fails.
    /// </summary>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="value">The string to check.</param>
    /// <param name="minLength">The minimum number of characters required.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> or <paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minLength"/> is negative.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ShorterThan<TException>([NotNull] string value, int minLength, Func<TException> exceptionFactory,
        SGuardCallback? callback = null) where TException : Exception
    {
        SGuard.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegative(minLength);
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(value.Length < minLength, callback))
        {
            Throw.From(exceptionFactory);
        }

        return value;
    }
}
