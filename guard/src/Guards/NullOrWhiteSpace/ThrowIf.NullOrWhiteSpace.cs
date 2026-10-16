using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SGuard.Exceptions;

namespace SGuard;

public sealed partial class ThrowIf
{
    /// <summary>
    /// Throws a <see cref="NullOrWhiteSpaceException"/> if the string is <c>null</c>, empty or consists only of white-space
    /// characters, and returns the string otherwise.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    /// <returns><paramref name="value"/>, which is not <c>null</c>.</returns>
    /// <exception cref="NullOrWhiteSpaceException"><paramref name="value"/> is <c>null</c>, empty or white space.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NullOrWhiteSpace([NotNull] string? value, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
    {
        if (SGuard.Fails(string.IsNullOrWhiteSpace(value), callback))
        {
            Throw.NullOrWhiteSpaceException(value, valueExpression);
        }

        // Tells the compiler what the guard established; removed from Release builds.
        Debug.Assert(value is not null);
        return value;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the string is <c>null</c>, empty or consists
    /// only of white-space characters, and returns the string otherwise. The factory runs only when the guard fails.
    /// </summary>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="value">The string to check.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="value"/>, which is not <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NullOrWhiteSpace<TException>([NotNull] string? value, Func<TException> exceptionFactory,
        SGuardCallback? callback = null) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(string.IsNullOrWhiteSpace(value), callback))
        {
            Throw.From(exceptionFactory);
        }

        // Tells the compiler what the guard established; removed from Release builds.
        Debug.Assert(value is not null);
        return value;
    }
}
