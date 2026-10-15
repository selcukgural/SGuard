using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SGuard;

public sealed partial class ThrowIf
{
    /// <summary>
    /// Throws an exception if the left value is less than the right value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <param name="lValue">The left value to compare. Must not be null.</param>
    /// <param name="rValue">The right value to compare against. Must not be null.</param>
    /// <param name="callback">
    /// An optional delegate that is invoked with the outcome of the guard.
    /// The outcome indicates whether the left value was less than the right value.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="lValue"/> or <paramref name="rValue"/> is null.</exception>
    /// <exception cref="Exception">Thrown if <paramref name="lValue"/> is less than <paramref name="rValue"/>.</exception>
    /// <param name="lValueExpression">The expression passed as <paramref name="lValue"/>, captured by the compiler.</param>
    /// <param name="rValueExpression">The expression passed as <paramref name="rValue"/>, captured by the compiler.</param>
    /// <returns><paramref name="lValue"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static TLeft LessThan<TLeft, TRight>([NotNull] TLeft lValue, [NotNull] TRight rValue, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(lValue))] string? lValueExpression = null,
        [CallerArgumentExpression(nameof(rValue))] string? rValueExpression = null)
        where TLeft : IComparable<TRight>
    {
        SGuard.ThrowIfNull(lValue);
        SGuard.ThrowIfNull(rValue);

        if (SGuard.Fails(Is.LessThan(lValue, rValue) || SGuard.AnyNaN(lValue, rValue), callback))
        {
            Throw.LessThanException(lValue, rValue, lValueExpression, rValueExpression);
        }

        return lValue;
    }


    /// <summary>
    /// Throws an exception if the first value is less than the second value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left-hand value.</typeparam>
    /// <typeparam name="TRight">The type of the right-hand value.</typeparam>
    /// <typeparam name="TException">The type of exception to throw if the condition is true.</typeparam>
    /// <param name="lValue">The left-hand value to compare.</param>
    /// <param name="rValue">The right-hand value to compare against.</param>
    /// <param name="exception">The exception to throw if the condition is met.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard evaluation.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the arguments are null.</exception>
    /// <exception cref="Exception">Thrown when <paramref name="lValue"/> is less than <paramref name="rValue"/>.</exception>
    /// <returns><paramref name="lValue"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static TLeft LessThan<TLeft, TRight, TException>([NotNull] TLeft lValue, [NotNull] TRight rValue, [NotNull] TException exception,
                                                           SGuardCallback? callback = null)
        where TLeft : IComparable<TRight> where TException : Exception
    {
        SGuard.ThrowIfNull(lValue);
        SGuard.ThrowIfNull(rValue);
        ArgumentNullException.ThrowIfNull(exception);
        
        if (SGuard.Fails(Is.LessThan(lValue, rValue) || SGuard.AnyNaN(lValue, rValue), callback))
        {
            Throw.That(exception);
        }

        return lValue;
    }


    /// <summary>
    /// Checks if the left value is less than or equal to the right value and throws an exception if the condition is met.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value. Must implement <see cref="IComparable{TRight}"/>.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <param name="lValue">The left value to compare. Must not be null.</param>
    /// <param name="rValue">The right value to compare against. Must not be null.</param>
    /// <param name="callback">
    /// An optional callback of type <see cref="SGuardCallback"/> that receives the result of the guard check
    /// with <see cref="GuardOutcome.Success"/> for a passed check or <see cref="GuardOutcome.Failure"/> for a failed check.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if either <paramref name="lValue"/> or <paramref name="rValue"/> is null.</exception>
    /// <exception cref="Exception">
    /// Thrown if the left value is less than or equal to the right value. The exception is determined by internal logic.
    /// </exception>
    /// <param name="lValueExpression">The expression passed as <paramref name="lValue"/>, captured by the compiler.</param>
    /// <param name="rValueExpression">The expression passed as <paramref name="rValue"/>, captured by the compiler.</param>
    /// <returns><paramref name="lValue"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static TLeft LessThanOrEqual<TLeft, TRight>([NotNull] TLeft lValue, [NotNull] TRight rValue, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(lValue))] string? lValueExpression = null,
        [CallerArgumentExpression(nameof(rValue))] string? rValueExpression = null)
        where TLeft : IComparable<TRight>
    {
        SGuard.ThrowIfNull(lValue);
        SGuard.ThrowIfNull(rValue);
        
        if (SGuard.Fails(Is.LessThanOrEqual(lValue, rValue) || SGuard.AnyNaN(lValue, rValue), callback))
        {
            Throw.LessThanOrEqualException(lValue, rValue, lValueExpression, rValueExpression);
        }

        return lValue;
    }


    /// <summary>
    /// Ensures that the left value is not less than or equal to the right value. If the condition is met, it allows you to specify an exception to throw.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value. Must implement <see cref="IComparable{TRight}"/>.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TException">The type of the exception to be thrown if the condition is met. Must be derived from <see cref="Exception"/>.</typeparam>
    /// <param name="lValue">The left-hand value to compare. Must not be null.</param>
    /// <param name="rValue">The right-hand value to compare against. Must not be null.</param>
    /// <param name="exception">The exception to throw if the condition is met. Must not be null.</param>
    /// <param name="callback">An optional callback invoked with the guard outcome, specifying whether the validation succeeded or failed.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="lValue"/>, <paramref name="rValue"/>, or <paramref name="exception"/> is null.</exception>
    /// <exception cref="Exception">Thrown if <paramref name="lValue"/> is less than or equal to <paramref name="rValue"/>.</exception>
    /// <returns><paramref name="lValue"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static TLeft LessThanOrEqual<TLeft, TRight, TException>([NotNull] TLeft lValue, [NotNull] TRight rValue, [NotNull] TException exception,
                                                                  SGuardCallback? callback = null)
        where TLeft : IComparable<TRight> where TException : Exception
    {
        SGuard.ThrowIfNull(lValue);
        SGuard.ThrowIfNull(rValue);
        ArgumentNullException.ThrowIfNull(exception);
        
        if (SGuard.Fails(Is.LessThanOrEqual(lValue, rValue) || SGuard.AnyNaN(lValue, rValue), callback))
        {
            Throw.That(exception);
        }

        return lValue;
    }

    /// <summary>
    /// Throws an exception if the left value is less than or equal to the right value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    ///  <typeparam name="TException">The type of the exception to be thrown if the condition is met.</typeparam>
    /// <param name="lValue">The left value to compare. Must not be null.</param>
    /// <param name="rValue">The right value to compare against. Must not be null.</param>
    /// <param name="callback">
    /// An optional delegate that is invoked with the outcome of the guard.
    /// The outcome indicates whether the left value was less than or equal to the right value.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="lValue"/> or <paramref name="rValue"/> is null.</exception>
    /// <exception cref="Exception">
    /// Thrown if <paramref name="lValue"/> is less than or equal to <paramref name="rValue"/>.
    /// The exception instance is created using the specified constructor arguments.
    /// </exception>
    /// <returns><paramref name="lValue"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static TLeft LessThanOrEqual<TLeft, TRight, TException>([NotNull] TLeft lValue, [NotNull] TRight rValue, SGuardCallback? callback = null)
        where TLeft : IComparable<TRight> where TException : Exception, new()
    {
        SGuard.ThrowIfNull(lValue);
        SGuard.ThrowIfNull(rValue);
        
        if (SGuard.Fails(Is.LessThanOrEqual(lValue, rValue) || SGuard.AnyNaN(lValue, rValue), callback))
        {
            Throw.That(new TException());
        }

        return lValue;
    }

    /// <summary>
    /// Throws an exception if the left value is less than or equal to the right value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TException">The type of the exception to be thrown if the condition is met.</typeparam>
    /// <param name="lValue">The left value to compare. Must not be null.</param>
    /// <param name="rValue">The right value to compare against. Must not be null.</param>
    /// <param name="constructorArgs">Optional array of arguments to construct the exception.</param>
    /// <param name="callback">
    /// An optional delegate that is invoked with the outcome of the guard.
    /// The outcome indicates whether the left value was less than or equal to the right value.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="lValue"/> or <paramref name="rValue"/> is null.</exception>
    /// <exception cref="Exception">
    /// Thrown if <paramref name="lValue"/> is less than or equal to <paramref name="rValue"/>.
    /// The exception instance is created using the specified constructor arguments.
    /// </exception>
    /// <returns><paramref name="lValue"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete(Obsoletions.ConstructorArgsMessage, DiagnosticId = Obsoletions.ConstructorArgsDiagnosticId, UrlFormat = Obsoletions.UrlFormat)]
    [return: NotNull]
    public static TLeft LessThanOrEqual<TLeft, TRight, TException>([NotNull] TLeft lValue, [NotNull] TRight rValue, object[]? constructorArgs,
                                                                  SGuardCallback? callback = null)
        where TLeft : IComparable<TRight> where TException : Exception
    {
        SGuard.ThrowIfNull(lValue);
        SGuard.ThrowIfNull(rValue);
        
        if (SGuard.Fails(Is.LessThanOrEqual(lValue, rValue) || SGuard.AnyNaN(lValue, rValue), callback))
        {
            Throw.That(ExceptionActivator.Create<TException>(constructorArgs));
        }

        return lValue;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the left value is less than the right value,
    /// and returns the left value otherwise. The factory runs only when the guard fails, so a passing guard doesn't create
    /// an exception.
    /// </summary>
    /// <remarks>A <c>NaN</c> operand fails.</remarks>
    /// <typeparam name="TLeft">The type of the left value. Must implement <see cref="IComparable{TRight}"/>.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="lValue">The left value to compare.</param>
    /// <param name="rValue">The right value to compare against.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="lValue"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="lValue"/>, <paramref name="rValue"/> or <paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static TLeft LessThan<TLeft, TRight, TException>([NotNull] TLeft lValue, [NotNull] TRight rValue,
        Func<TException> exceptionFactory, SGuardCallback? callback = null)
        where TLeft : IComparable<TRight> where TException : Exception
    {
        SGuard.ThrowIfNull(lValue);
        SGuard.ThrowIfNull(rValue);
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(Is.LessThan(lValue, rValue) || SGuard.AnyNaN(lValue, rValue), callback))
        {
            Throw.From(exceptionFactory);
        }

        return lValue;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the left value is less than or equal to the right value,
    /// and returns the left value otherwise. The factory runs only when the guard fails, so a passing guard doesn't create
    /// an exception.
    /// </summary>
    /// <remarks>A <c>NaN</c> operand fails.</remarks>
    /// <typeparam name="TLeft">The type of the left value. Must implement <see cref="IComparable{TRight}"/>.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="lValue">The left value to compare.</param>
    /// <param name="rValue">The right value to compare against.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="lValue"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="lValue"/>, <paramref name="rValue"/> or <paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNull]
    public static TLeft LessThanOrEqual<TLeft, TRight, TException>([NotNull] TLeft lValue, [NotNull] TRight rValue,
        Func<TException> exceptionFactory, SGuardCallback? callback = null)
        where TLeft : IComparable<TRight> where TException : Exception
    {
        SGuard.ThrowIfNull(lValue);
        SGuard.ThrowIfNull(rValue);
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(Is.LessThanOrEqual(lValue, rValue) || SGuard.AnyNaN(lValue, rValue), callback))
        {
            Throw.From(exceptionFactory);
        }

        return lValue;
    }
}
