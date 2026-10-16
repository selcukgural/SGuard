using System.Numerics;
using System.Runtime.CompilerServices;
using SGuard.Exceptions;

namespace SGuard;

public sealed partial class ThrowIf
{
    /// <summary>
    /// Throws a <see cref="NegativeException"/> if the number is less than zero, and returns the number otherwise.
    /// </summary>
    /// <remarks>
    /// Like the other comparisons, <c>NaN</c> fails. Negative zero (<c>-0.0</c>) equals zero and passes.
    /// </remarks>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <param name="value">The number to check.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="NegativeException"><paramref name="value"/> is less than zero or <c>NaN</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Negative<T>(T value, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null) where T : INumber<T>
    {
        if (SGuard.Fails(value < T.Zero || T.IsNaN(value), callback))
        {
            Throw.NegativeException(value, valueExpression);
        }

        return value;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the number is less than zero, and returns
    /// the number otherwise. The factory runs only when the guard fails.
    /// </summary>
    /// <remarks>Like the other comparisons, <c>NaN</c> fails.</remarks>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="value">The number to check.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Negative<T, TException>(T value, Func<TException> exceptionFactory, SGuardCallback? callback = null)
        where T : INumber<T> where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(value < T.Zero || T.IsNaN(value), callback))
        {
            Throw.From(exceptionFactory);
        }

        return value;
    }

    /// <summary>
    /// Throws a <see cref="NegativeOrZeroException"/> if the number is less than or equal to zero, and returns the number
    /// otherwise.
    /// </summary>
    /// <remarks>Like the other comparisons, <c>NaN</c> fails.</remarks>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <param name="value">The number to check.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="NegativeOrZeroException"><paramref name="value"/> is less than or equal to zero, or <c>NaN</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NegativeOrZero<T>(T value, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null) where T : INumber<T>
    {
        if (SGuard.Fails(value <= T.Zero || T.IsNaN(value), callback))
        {
            Throw.NegativeOrZeroException(value, valueExpression);
        }

        return value;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the number is less than or equal to zero,
    /// and returns the number otherwise. The factory runs only when the guard fails.
    /// </summary>
    /// <remarks>Like the other comparisons, <c>NaN</c> fails.</remarks>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="value">The number to check.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NegativeOrZero<T, TException>(T value, Func<TException> exceptionFactory, SGuardCallback? callback = null)
        where T : INumber<T> where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(value <= T.Zero || T.IsNaN(value), callback))
        {
            Throw.From(exceptionFactory);
        }

        return value;
    }
}
