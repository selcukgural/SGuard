using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SGuard.Exceptions;

namespace SGuard;

public sealed partial class ThrowIf
{
    /// <summary>
    /// Throws a <see cref="NullException"/> if the value is <c>null</c>, and returns the value otherwise.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="NullOrEmpty{T}(T, SGuardCallback?, string?)"/>, only <c>null</c> fails: <c>""</c>, an empty
    /// collection, <c>0</c> and other default values pass.
    /// </remarks>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    /// <returns><paramref name="value"/>, which is not <c>null</c>.</returns>
    /// <exception cref="NullException"><paramref name="value"/> is <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static T Null<T>([NotNull] T value, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
    {
        if (SGuard.Fails(value is null, callback))
        {
            Throw.NullException(valueExpression);
        }

        // Tells the compiler what the guard established; removed from Release builds.
        Debug.Assert(value is not null);
        return value;
    }

    /// <summary>
    /// Throws a <see cref="NullException"/> if the nullable value has no value, and returns the underlying value otherwise.
    /// </summary>
    /// <typeparam name="T">The underlying value type.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    /// <returns>The value of <paramref name="value"/>.</returns>
    /// <exception cref="NullException"><paramref name="value"/> is <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Null<T>([NotNull] T? value, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null) where T : struct
    {
        if (SGuard.Fails(!value.HasValue, callback))
        {
            Throw.NullException(valueExpression);
        }

        Debug.Assert(value.HasValue);
        return value.GetValueOrDefault();
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the value is <c>null</c>, and returns the
    /// value otherwise. The factory runs only when the guard fails.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="value"/>, which is not <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static T Null<T, TException>([NotNull] T value, Func<TException> exceptionFactory, SGuardCallback? callback = null)
        where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(value is null, callback))
        {
            Throw.From(exceptionFactory);
        }

        // Tells the compiler what the guard established; removed from Release builds.
        Debug.Assert(value is not null);
        return value;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the nullable value has no value, and returns
    /// the underlying value otherwise. The factory runs only when the guard fails.
    /// </summary>
    /// <typeparam name="T">The underlying value type.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns>The value of <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Null<T, TException>([NotNull] T? value, Func<TException> exceptionFactory, SGuardCallback? callback = null)
        where T : struct where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(!value.HasValue, callback))
        {
            Throw.From(exceptionFactory);
        }

        Debug.Assert(value.HasValue);
        return value.GetValueOrDefault();
    }
}
