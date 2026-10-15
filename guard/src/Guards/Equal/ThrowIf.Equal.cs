using System.Runtime.CompilerServices;
using SGuard.Exceptions;

namespace SGuard;

public sealed partial class ThrowIf
{
    /// <summary>
    /// Throws an <see cref="EqualException"/> if the two values are equal, using <see cref="EqualityComparer{T}.Default"/>,
    /// and returns <paramref name="lValue"/> otherwise.
    /// </summary>
    /// <remarks><c>null</c> operands are allowed: two <c>null</c> values are equal.</remarks>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="lValue">The value to check.</param>
    /// <param name="rValue">The value it must not equal.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <param name="lValueExpression">The expression passed as <paramref name="lValue"/>, captured by the compiler.</param>
    /// <param name="rValueExpression">The expression passed as <paramref name="rValue"/>, captured by the compiler.</param>
    /// <returns><paramref name="lValue"/>.</returns>
    /// <exception cref="EqualException">The values are equal.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Equal<T>(T lValue, T rValue, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(lValue))] string? lValueExpression = null,
        [CallerArgumentExpression(nameof(rValue))] string? rValueExpression = null)
    {
        if (SGuard.Fails(EqualityComparer<T>.Default.Equals(lValue, rValue), callback))
        {
            Throw.EqualException(lValue, rValue, lValueExpression, rValueExpression);
        }

        return lValue;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the two values are equal, using
    /// <see cref="EqualityComparer{T}.Default"/>, and returns <paramref name="lValue"/> otherwise. The factory runs only
    /// when the guard fails.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="lValue">The value to check.</param>
    /// <param name="rValue">The value it must not equal.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="lValue"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Equal<T, TException>(T lValue, T rValue, Func<TException> exceptionFactory, SGuardCallback? callback = null)
        where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(EqualityComparer<T>.Default.Equals(lValue, rValue), callback))
        {
            Throw.From(exceptionFactory);
        }

        return lValue;
    }

    /// <summary>
    /// Throws an <see cref="EqualException"/> if the two strings are equal under the given StringComparison, and returns
    /// <paramref name="lValue"/> otherwise. <c>null</c> operands are allowed: two <c>null</c> strings are equal.
    /// </summary>
    /// <param name="lValue">The string to check.</param>
    /// <param name="rValue">The string it must not equal.</param>
    /// <param name="comparison">The string comparison rule to use.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <param name="lValueExpression">The expression passed as <paramref name="lValue"/>, captured by the compiler.</param>
    /// <param name="rValueExpression">The expression passed as <paramref name="rValue"/>, captured by the compiler.</param>
    /// <returns><paramref name="lValue"/>.</returns>
    /// <exception cref="EqualException">The strings are equal.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Equal(string? lValue, string? rValue, StringComparison comparison, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(lValue))] string? lValueExpression = null,
        [CallerArgumentExpression(nameof(rValue))] string? rValueExpression = null)
    {
        if (SGuard.Fails(string.Equals(lValue, rValue, comparison), callback))
        {
            Throw.EqualException(lValue, rValue, lValueExpression, rValueExpression);
        }

        return lValue;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the two strings are equal under the given
    /// StringComparison, and returns <paramref name="lValue"/> otherwise. The factory runs only when the guard fails.
    /// </summary>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="lValue">The string to check.</param>
    /// <param name="rValue">The string to compare it with.</param>
    /// <param name="comparison">The string comparison rule to use.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="lValue"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Equal<TException>(string? lValue, string? rValue, StringComparison comparison,
        Func<TException> exceptionFactory, SGuardCallback? callback = null) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(string.Equals(lValue, rValue, comparison), callback))
        {
            Throw.From(exceptionFactory);
        }

        return lValue;
    }

    /// <summary>
    /// Throws a <see cref="NotEqualException"/> if the two values are not equal, using
    /// <see cref="EqualityComparer{T}.Default"/>, and returns <paramref name="lValue"/> otherwise.
    /// </summary>
    /// <remarks><c>null</c> operands are allowed: two <c>null</c> values are equal.</remarks>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="lValue">The value to check.</param>
    /// <param name="rValue">The value it must equal.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <param name="lValueExpression">The expression passed as <paramref name="lValue"/>, captured by the compiler.</param>
    /// <param name="rValueExpression">The expression passed as <paramref name="rValue"/>, captured by the compiler.</param>
    /// <returns><paramref name="lValue"/>.</returns>
    /// <exception cref="NotEqualException">The values are not equal.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotEqual<T>(T lValue, T rValue, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(lValue))] string? lValueExpression = null,
        [CallerArgumentExpression(nameof(rValue))] string? rValueExpression = null)
    {
        if (SGuard.Fails(!EqualityComparer<T>.Default.Equals(lValue, rValue), callback))
        {
            Throw.NotEqualException(lValue, rValue, lValueExpression, rValueExpression);
        }

        return lValue;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the two values are not equal, using
    /// <see cref="EqualityComparer{T}.Default"/>, and returns <paramref name="lValue"/> otherwise. The factory runs only
    /// when the guard fails.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="lValue">The value to check.</param>
    /// <param name="rValue">The value it must equal.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="lValue"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotEqual<T, TException>(T lValue, T rValue, Func<TException> exceptionFactory, SGuardCallback? callback = null)
        where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(!EqualityComparer<T>.Default.Equals(lValue, rValue), callback))
        {
            Throw.From(exceptionFactory);
        }

        return lValue;
    }

    /// <summary>
    /// Throws a <see cref="NotEqualException"/> if the two strings are not equal under the given StringComparison, and
    /// returns <paramref name="lValue"/> otherwise. <c>null</c> operands are allowed: two <c>null</c> strings are equal.
    /// </summary>
    /// <param name="lValue">The string to check.</param>
    /// <param name="rValue">The string it must equal.</param>
    /// <param name="comparison">The string comparison rule to use.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <param name="lValueExpression">The expression passed as <paramref name="lValue"/>, captured by the compiler.</param>
    /// <param name="rValueExpression">The expression passed as <paramref name="rValue"/>, captured by the compiler.</param>
    /// <returns><paramref name="lValue"/>.</returns>
    /// <exception cref="NotEqualException">The strings are not equal.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? NotEqual(string? lValue, string? rValue, StringComparison comparison, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(lValue))] string? lValueExpression = null,
        [CallerArgumentExpression(nameof(rValue))] string? rValueExpression = null)
    {
        if (SGuard.Fails(!string.Equals(lValue, rValue, comparison), callback))
        {
            Throw.NotEqualException(lValue, rValue, lValueExpression, rValueExpression);
        }

        return lValue;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the two strings are not equal under the given
    /// StringComparison, and returns <paramref name="lValue"/> otherwise. The factory runs only when the guard fails.
    /// </summary>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="lValue">The string to check.</param>
    /// <param name="rValue">The string to compare it with.</param>
    /// <param name="comparison">The string comparison rule to use.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="lValue"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? NotEqual<TException>(string? lValue, string? rValue, StringComparison comparison,
        Func<TException> exceptionFactory, SGuardCallback? callback = null) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(!string.Equals(lValue, rValue, comparison), callback))
        {
            Throw.From(exceptionFactory);
        }

        return lValue;
    }
}
