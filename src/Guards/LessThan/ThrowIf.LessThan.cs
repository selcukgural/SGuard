using System.Diagnostics.CodeAnalysis;

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
    public static void LessThan<TLeft, TRight>([NotNull] TLeft lValue, [NotNull] TRight rValue, SGuardCallback? callback = null)
        where TLeft : IComparable<TRight>
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);

        SGuard.Guard(Is.LessThan(lValue, rValue), () => Throw.LessThanException(lValue, rValue), callback);
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
    /// <exception cref="TException">Thrown when <paramref name="lValue"/> is less than <paramref name="rValue"/>.</exception>
    public static void LessThan<TLeft, TRight, TException>([NotNull] TLeft lValue, [NotNull] TRight rValue, [NotNull] TException exception,
                                                           SGuardCallback? callback = null)
        where TLeft : IComparable<TRight> where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);
        ArgumentNullException.ThrowIfNull(exception);
        
        SGuard.Guard(Is.LessThan(lValue, rValue), () => Throw.That(exception), callback);
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
    public static void LessThanOrEqual<TLeft, TRight>([NotNull] TLeft lValue, [NotNull] TRight rValue, SGuardCallback? callback = null)
        where TLeft : IComparable<TRight>
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);
        
        SGuard.Guard(Is.LessThanOrEqual(lValue, rValue), () => Throw.LessThanOrEqualException(lValue, rValue), callback);
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
    /// <exception cref="TException">Thrown if <paramref name="lValue"/> is less than or equal to <paramref name="rValue"/>.</exception>
    public static void LessThanOrEqual<TLeft, TRight, TException>([NotNull] TLeft lValue, [NotNull] TRight rValue, [NotNull] TException exception,
                                                                  SGuardCallback? callback = null)
        where TLeft : IComparable<TRight> where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);
        ArgumentNullException.ThrowIfNull(exception);
        
        SGuard.Guard(Is.LessThanOrEqual(lValue, rValue), () => Throw.That(exception), callback);
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
    /// <exception cref="TException">
    /// Thrown if <paramref name="lValue"/> is less than or equal to <paramref name="rValue"/>.
    /// The exception instance is created using the specified constructor arguments.
    /// </exception>
    public static void LessThanOrEqual<TLeft, TRight, TException>([NotNull] TLeft lValue, [NotNull] TRight rValue, SGuardCallback? callback = null)
        where TLeft : IComparable<TRight> where TException : Exception, new()
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);
        
        SGuard.Guard(Is.LessThanOrEqual(lValue, rValue), () => Throw.That(ExceptionActivator.Create<TException>(null)), callback);
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
    /// <exception cref="TException">
    /// Thrown if <paramref name="lValue"/> is less than or equal to <paramref name="rValue"/>.
    /// The exception instance is created using the specified constructor arguments.
    /// </exception>
    public static void LessThanOrEqual<TLeft, TRight, TException>([NotNull] TLeft lValue, [NotNull] TRight rValue, object[]? constructorArgs,
                                                                  SGuardCallback? callback = null)
        where TLeft : IComparable<TRight> where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);
        
        SGuard.Guard(Is.LessThanOrEqual(lValue, rValue), () => Throw.That(ExceptionActivator.Create<TException>(constructorArgs)), callback);
    }
}