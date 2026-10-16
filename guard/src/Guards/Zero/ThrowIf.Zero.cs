using System.Numerics;
using System.Runtime.CompilerServices;
using SGuard.Exceptions;

namespace SGuard;

public sealed partial class ThrowIf
{
    /// <summary>
    /// Throws a <see cref="ZeroException"/> if the number is zero, and returns the number otherwise.
    /// </summary>
    /// <remarks>Negative zero (<c>-0.0</c>) is zero. <c>NaN</c> is not zero and passes.</remarks>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <param name="value">The number to check.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="ZeroException"><paramref name="value"/> is zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Zero<T>(T value, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null) where T : INumberBase<T>
    {
        if (SGuard.Fails(T.IsZero(value), callback))
        {
            Throw.ZeroException(value, valueExpression);
        }

        return value;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the number is zero, and returns the number
    /// otherwise. The factory runs only when the guard fails.
    /// </summary>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="value">The number to check.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Zero<T, TException>(T value, Func<TException> exceptionFactory, SGuardCallback? callback = null)
        where T : INumberBase<T> where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(T.IsZero(value), callback))
        {
            Throw.From(exceptionFactory);
        }

        return value;
    }
}
