using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SGuard;

/// <summary>
/// The <see cref="ThrowIf"/> class provides static methods for validating
/// and asserting conditions to ensure data integrity. It offers methods to
/// throw exceptions when certain conditions are met.
/// </summary>
public sealed partial class ThrowIf
{
    /// <summary>
    /// Throws an exception if the specified value is between the provided minimum and maximum values, inclusive.
    /// </summary>
    /// <remarks>
    /// The guard throws when the value is <b>inside</b> the range. To reject values outside a range, use
    /// <see cref="ThrowIf.LessThan{TLeft, TRight}(TLeft, TRight, SGuardCallback?, string?, string?)"/> and
    /// <see cref="ThrowIf.GreaterThan{TLeft, TRight}(TLeft, TRight, SGuardCallback?, string?, string?)"/>, or
    /// <c>!Is.Between(...)</c>.
    /// </remarks>
    /// <typeparam name="TValue">The type of the value to evaluate.</typeparam>
    /// <typeparam name="TMin">The type of the minimum value.</typeparam>
    /// <typeparam name="TMax">The type of the maximum value.</typeparam>
    /// <param name="value">The value to evaluate.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="callback">An optional callback that will be invoked with the guard evaluation outcome.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/>, <paramref name="min"/>, or <paramref name="max"/> is null.</exception>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    /// <param name="minExpression">The expression passed as <paramref name="min"/>, captured by the compiler.</param>
    /// <param name="maxExpression">The expression passed as <paramref name="max"/>, captured by the compiler.</param>
    /// <returns><paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static TValue Between<TValue, TMin, TMax>([NotNull] TValue value, [NotNull] TMin min, [NotNull] TMax max, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null,
        [CallerArgumentExpression(nameof(min))] string? minExpression = null,
        [CallerArgumentExpression(nameof(max))] string? maxExpression = null)
        where TValue : IComparable<TMin>, IComparable<TMax>
    {
        SGuard.ThrowIfNull(min);
        SGuard.ThrowIfNull(max);
        SGuard.ThrowIfNull(value);
        
        if (SGuard.Fails(Is.Between(value, min, max) || SGuard.AnyNaN(value, min, max), callback))
        {
            Throw.BetweenException(value, min, max, valueExpression, minExpression, maxExpression);
        }

        return value;
    }

    /// <summary>
    /// Throws an exception if the specified value is between the given minimum and maximum bounds.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to evaluate.</typeparam>
    /// <typeparam name="TMin">The type of the minimum bound.</typeparam>
    /// <typeparam name="TMax">The type of the maximum bound.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw if the value is between the bounds.</typeparam>
    /// <param name="value">The value to evaluate.</param>
    /// <param name="min">The minimum bound.</param>
    /// <param name="max">The maximum bound.</param>
    /// <param name="exception">The exception to be thrown if the condition is met.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the evaluation.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/>, <paramref name="min"/>, <paramref name="max"/>, or <paramref name="exception"/> is null.</exception>
    /// <exception cref="Exception">Thrown if <paramref name="value"/> is between <paramref name="min"/> and <paramref name="max"/>.</exception>
    /// <returns><paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static TValue Between<TValue, TMin, TMax, TException>([NotNull] TValue value, [NotNull] TMin min, [NotNull] TMax max,
                                                               [NotNull] TException exception, SGuardCallback? callback = null)
        where TValue : IComparable<TMin>, IComparable<TMax> where TException : Exception
    {
        SGuard.ThrowIfNull(min);
        SGuard.ThrowIfNull(max);
        SGuard.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(exception);
        
        if (SGuard.Fails(Is.Between(value, min, max) || SGuard.AnyNaN(value, min, max), callback))
        {
            Throw.That(exception);
        }

        return value;
    }

    /// <summary>
    /// Throws an exception if the specified string is between the provided bounds using the given StringComparison.
    /// </summary>
    /// <param name="value">The string value to evaluate.</param>
    /// <param name="min">The minimum bound (inclusive).</param>
    /// <param name="max">The maximum bound (inclusive).</param>
    /// <param name="comparison">The string comparison rule to use.</param>
    /// <param name="callback">Optional guard outcome callback.</param>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    /// <param name="minExpression">The expression passed as <paramref name="min"/>, captured by the compiler.</param>
    /// <param name="maxExpression">The expression passed as <paramref name="max"/>, captured by the compiler.</param>
    /// <returns><paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Between(string value, string min, string max, StringComparison comparison, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null,
        [CallerArgumentExpression(nameof(min))] string? minExpression = null,
        [CallerArgumentExpression(nameof(max))] string? maxExpression = null)
    {
        SGuard.ThrowIfNull(min);
        SGuard.ThrowIfNull(max);
        SGuard.ThrowIfNull(value);
        
        if (SGuard.Fails(Is.Between(value, min, max, comparison), callback))
        {
            Throw.BetweenException(value, min, max, valueExpression, minExpression, maxExpression);
        }

        return value;
    }

    /// <summary>
    /// Throws the provided exception if the specified string is between the given bounds using the given StringComparison.
    /// </summary>
    /// <param name="value">The string value to evaluate.</param>
    /// <param name="min">The minimum bound (inclusive).</param>
    /// <param name="max">The maximum bound (inclusive).</param>
    /// <param name="comparison">The string comparison rule to use.</param>
    /// <param name="exception">The exception to throw when the condition is met.</param>
    /// <param name="callback">Optional guard outcome callback.</param>
    /// <returns><paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Between<TException>(string value, string min, string max, StringComparison comparison, [NotNull] TException exception,
                                           SGuardCallback? callback = null) where TException : Exception
    {
        SGuard.ThrowIfNull(min);
        SGuard.ThrowIfNull(max);
        SGuard.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(exception);
        
        if (SGuard.Fails(Is.Between(value, min, max, comparison), callback))
        {
            Throw.That(exception);
        }

        return value;
    }

    /// <summary>
    /// Throws an exception if the specified value is between the provided minimum and maximum values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to evaluate.</typeparam>
    /// <typeparam name="TMin">The type of the minimum value.</typeparam>
    /// <typeparam name="TMax">The type of the maximum value.</typeparam>
    /// <typeparam name="TException">The type of the exception to be thrown if the condition is met.</typeparam>
    /// <param name="value">The value to evaluate.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="callback">An optional callback that will be invoked with the guard evaluation outcome.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/>, <paramref name="min"/>, or <paramref name="max"/> is null.</exception>
    /// <exception cref="Exception">Thrown if <paramref name="value"/> is between <paramref name="min"/> and <paramref name="max"/>.</exception>
    /// <returns><paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static TValue Between<TValue, TMin, TMax, TException>([NotNull] TValue value, [NotNull] TMin min, [NotNull] TMax max,
                                                               SGuardCallback? callback = null)
        where TValue : IComparable<TMin>, IComparable<TMax> where TException : Exception, new()
    {
        SGuard.ThrowIfNull(min);
        SGuard.ThrowIfNull(max);
        SGuard.ThrowIfNull(value);
        
        if (SGuard.Fails(Is.Between(value, min, max) || SGuard.AnyNaN(value, min, max), callback))
        {
            Throw.That(new TException());
        }

        return value;
    }

    /// <summary>
    /// Throws an exception if the specified value is between the provided minimum and maximum values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to evaluate.</typeparam>
    /// <typeparam name="TMin">The type of the minimum value.</typeparam>
    /// <typeparam name="TMax">The type of the maximum value.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw when the guard condition fails.</typeparam>
    /// <param name="value">The value to evaluate.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="constructorArgs">An array of arguments used to construct the exception.</param>
    /// <param name="callback">An optional callback invoked with the guard evaluation result.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/>, <paramref name="min"/>, or <paramref name="max"/> is null.</exception>
    /// <exception cref="Exception">Thrown if <paramref name="value"/> is between <paramref name="min"/> and <paramref name="max"/>.</exception>
    /// <returns><paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete(Obsoletions.ConstructorArgsMessage, DiagnosticId = Obsoletions.ConstructorArgsDiagnosticId, UrlFormat = Obsoletions.UrlFormat)]
    [return: NotNull]
    public static TValue Between<TValue, TMin, TMax, TException>([NotNull] TValue value, [NotNull] TMin min, [NotNull] TMax max,
                                                               object[]? constructorArgs, SGuardCallback? callback = null)
        where TValue : IComparable<TMin>, IComparable<TMax> where TException : Exception
    {
        SGuard.ThrowIfNull(min);
        SGuard.ThrowIfNull(max);
        SGuard.ThrowIfNull(value);
        
        if (SGuard.Fails(Is.Between(value, min, max) || SGuard.AnyNaN(value, min, max), callback))
        {
            Throw.That(ExceptionActivator.Create<TException>(constructorArgs));
        }

        return value;
    }


    /// <summary>
    /// Throws an exception of type <typeparamref name="TException"/> if the specified string is between the given bounds,
    /// inclusive, using the specified StringComparison.
    /// </summary>
    /// <param name="value">The value to evaluate.</param>
    /// <param name="min">The minimum bound (inclusive).</param>
    /// <param name="max">The maximum bound (inclusive).</param>
    /// <param name="comparison">The StringComparison method to use when comparing the string values.</param>
    /// <param name="callback">An optional callback that will be invoked with the guard evaluation outcome.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/>, <paramref name="min"/>, or <paramref name="max"/> is null.</exception>
    /// <exception cref="Exception">Thrown if <paramref name="value"/> is between <paramref name="min"/> and <paramref name="max"/>.</exception>
    /// <returns><paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Between<TException>(string value, string min, string max, StringComparison comparison, SGuardCallback? callback = null)
        where TException : Exception, new()
    {
        SGuard.ThrowIfNull(min);
        SGuard.ThrowIfNull(max);
        SGuard.ThrowIfNull(value);
        
        if (SGuard.Fails(Is.Between(value, min, max, comparison), callback))
        {
            Throw.That(new TException());
        }

        return value;
    }

    /// <summary>
    /// Throws an exception of type <typeparamref name="TException"/> if the specified string is between the given bounds using the specified StringComparison.
    /// </summary>
    /// <typeparam name="TException">The type of exception to throw. Must inherit from Exception and have a parameterless constructor.</typeparam>
    /// <param name="value">The string value to evaluate.</param>
    /// <param name="min">The minimum bound (inclusive).</param>
    /// <param name="max">The maximum bound (inclusive).</param>
    /// <param name="comparison">The string comparison rule to use for evaluation.</param>
    /// <param name="constructorArgs">Optional constructor arguments for creating the exception instance.</param>
    /// <param name="callback">Optional callback invoked with the outcome of the evaluation.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/>, <paramref name="min"/>, or <paramref name="max"/> is null.</exception>
    /// <exception cref="Exception">Thrown if <paramref name="value"/> is between <paramref name="min"/> and <paramref name="max"/>.</exception>
    /// <returns><paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete(Obsoletions.ConstructorArgsMessage, DiagnosticId = Obsoletions.ConstructorArgsDiagnosticId, UrlFormat = Obsoletions.UrlFormat)]
    public static string Between<TException>(string value, string min, string max, StringComparison comparison, object[]? constructorArgs,
                                           SGuardCallback? callback = null) where TException : Exception
    {
        SGuard.ThrowIfNull(min);
        SGuard.ThrowIfNull(max);
        SGuard.ThrowIfNull(value);
        
        if (SGuard.Fails(Is.Between(value, min, max, comparison), callback))
        {
            Throw.That(ExceptionActivator.Create<TException>(constructorArgs));
        }

        return value;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the value is between
    /// <paramref name="min"/> and <paramref name="max"/>, inclusive, and returns the value otherwise. The factory runs
    /// only when the guard fails.
    /// </summary>
    /// <remarks>
    /// The guard throws when the value is <b>inside</b> the range. To reject values outside a range, use
    /// <see cref="OutOfRange{TValue, TMin, TMax, TException}(TValue, TMin, TMax, Func{TException}, SGuardCallback?)"/>.
    /// A <c>NaN</c> operand fails.
    /// </remarks>
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
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static TValue Between<TValue, TMin, TMax, TException>([NotNull] TValue value, [NotNull] TMin min, [NotNull] TMax max,
        Func<TException> exceptionFactory, SGuardCallback? callback = null)
        where TValue : IComparable<TMin>, IComparable<TMax> where TException : Exception
    {
        SGuard.ThrowIfNull(min);
        SGuard.ThrowIfNull(max);
        SGuard.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(Is.Between(value, min, max) || SGuard.AnyNaN(value, min, max), callback))
        {
            Throw.From(exceptionFactory);
        }

        return value;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the string is between the given bounds,
    /// inclusive, under the given StringComparison, and returns the string otherwise. The factory runs only when the guard
    /// fails.
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
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Between<TException>(string value, string min, string max, StringComparison comparison,
        Func<TException> exceptionFactory, SGuardCallback? callback = null) where TException : Exception
    {
        SGuard.ThrowIfNull(min);
        SGuard.ThrowIfNull(max);
        SGuard.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(Is.Between(value, min, max, comparison), callback))
        {
            Throw.From(exceptionFactory);
        }

        return value;
    }
}
