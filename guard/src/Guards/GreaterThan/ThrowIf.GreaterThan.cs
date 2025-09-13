using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SGuard;

public sealed partial class ThrowIf
{
    /// <summary>
    /// Throws an exception if the left value is greater than the right value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value, which must implement IComparable with the right value type.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <param name="lValue">The left value to compare. Must not be null.</param>
    /// <param name="rValue">The right value to compare to. Must not be null.</param>
    /// <param name="callback">
    /// Optional callback that receives the outcome of the guard evaluation as a <see cref="GuardOutcome"/> value.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if the left value or the right value is null.</exception>
    /// <exception cref="Exception">Thrown if the left value is greater than the right value, with specific exception handling logic.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GreaterThan<TLeft, TRight>([NotNull] TLeft lValue, [NotNull] TRight rValue, SGuardCallback? callback = null)
        where TLeft : IComparable<TRight>
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);
        
        SGuard.Guard(Is.GreaterThan(lValue, rValue), () => Throw.GreaterThanException(lValue, rValue), callback);
    }

    /// <summary>
    /// Throws an exception of type <typeparamref name="TException"/> if the left value is greater than the right value.
    /// The exception is created using its default constructor.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value, which implements IComparable.</typeparam>
    /// <typeparam name="TRight">The type of the right value, which is comparable to TLeft.</typeparam>
    /// <typeparam name="TException">The type of the exception to be thrown if the condition is met. Must have a parameterless constructor.</typeparam>
    /// <param name="lValue">The left value to evaluate.</param>
    /// <param name="rValue">The right value to evaluate against.</param>
    /// <param name="callback">Optional guard callback that receives the outcome of the evaluation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GreaterThan<TLeft, TRight, TException>([NotNull] TLeft lValue, [NotNull] TRight rValue, SGuardCallback? callback = null)
        where TLeft : IComparable<TRight> where TException : Exception, new()
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);
        
        SGuard.Guard(Is.GreaterThan(lValue, rValue), () => Throw.That(ExceptionActivator.Create<TException>(null)), callback);
    }

    /// <summary>
    /// Throws an exception of type <typeparamref name="TException"/> if the left value is greater than the right value.
    /// The exception is created using the provided constructor arguments.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value, which implements IComparable.</typeparam>
    /// <typeparam name="TRight">The type of the right value, which is comparable to TLeft.</typeparam>
    /// <typeparam name="TException">The type of the exception to be thrown if the condition is met.</typeparam>
    /// <param name="lValue">The left value to evaluate.</param>
    /// <param name="rValue">The right value to evaluate against.</param>
    /// <param name="constructorArgs">The arguments to pass to the constructor of the exception.</param>
    /// <param name="callback">Optional guard callback that receives the outcome of the evaluation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GreaterThan<TLeft, TRight, TException>([NotNull] TLeft lValue, [NotNull] TRight rValue, object[]? constructorArgs,
                                                              SGuardCallback? callback = null)
        where TLeft : IComparable<TRight> where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);
        
        SGuard.Guard(Is.GreaterThan(lValue, rValue), () => Throw.That(ExceptionActivator.Create<TException>(constructorArgs)), callback);
    }

    /// <summary>
    /// Throws an exception if the specified left value is greater than the specified right value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value. Must implement <see cref="System.IComparable{TRight}"/>.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw if the condition is met. Must derive from <see cref="System.Exception"/>.</typeparam>
    /// <param name="lValue">The left value to compare.</param>
    /// <param name="rValue">The right value to compare against.</param>
    /// <param name="exception">The exception to throw if the condition is met.</param>
    /// <param name="callback">Optional. A callback to invoke indicating the outcome of the guard evaluation.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="lValue"/>, <paramref name="rValue"/>, or <paramref name="exception"/> is null.</exception>
    /// <exception cref="TException">Thrown when <paramref name="lValue"/> is greater than <paramref name="rValue"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GreaterThan<TLeft, TRight, TException>([NotNull] TLeft lValue, [NotNull] TRight rValue, [NotNull] TException exception,
                                                              SGuardCallback? callback = null) where TLeft : IComparable<TRight>
                                                                                               where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);
        ArgumentNullException.ThrowIfNull(exception);
        
        SGuard.Guard(Is.GreaterThan(lValue, rValue), () => Throw.That(exception), callback);
    }

    /// <summary>
    /// Throws an exception if the left value is greater than or equal to the right value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value. Must implement <see cref="IComparable{TRight}"/>.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <param name="lValue">The left value to be compared. Must not be null.</param>
    /// <param name="rValue">The right value to be compared. Must not be null.</param>
    /// <param name="callback">An optional callback invoked with the evaluation outcome.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="lValue"/> or <paramref name="rValue"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="lValue"/> is greater than or equal to <paramref name="rValue"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GreaterThanOrEqual<TLeft, TRight>([NotNull] TLeft lValue, [NotNull] TRight rValue, SGuardCallback? callback = null)
        where TLeft : IComparable<TRight>
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);
        
        SGuard.Guard(Is.GreaterThanOrEqual(lValue, rValue), () => Throw.GreaterThanOrEqualException(lValue, rValue), callback);
    }


    /// <summary>
    /// Throws an exception if the left value is greater than or equal to the right value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value, which implements IComparable.</typeparam>
    /// <typeparam name="TRight">The type of the right value, which is comparable to TLeft.</typeparam>
    /// <typeparam name="TException">The type of the exception to be thrown if the condition is met.</typeparam>
    /// <param name="lValue">The left value to evaluate.</param>
    /// <param name="rValue">The right value to evaluate against.</param>
    /// <param name="exception">The exception to throw if the left value is greater than or equal to the right value.</param>
    /// <param name="callback">Optional guard callback that receives the outcome of the evaluation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GreaterThanOrEqual<TLeft, TRight, TException>([NotNull] TLeft lValue, [NotNull] TRight rValue, [NotNull] TException exception,
                                                                     SGuardCallback? callback = null)
        where TLeft : IComparable<TRight> where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);
        ArgumentNullException.ThrowIfNull(exception);
        
        SGuard.Guard(Is.GreaterThanOrEqual(lValue, rValue), () => Throw.That(exception), callback);
    }


    /// <summary>
    /// Throws an exception of type <typeparamref name="TException"/> if the left value is greater than or equal to the right value.
    /// The exception is created using its default constructor.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value, which implements IComparable.</typeparam>
    /// <typeparam name="TRight">The type of the right value, which is comparable to TLeft.</typeparam>
    /// <typeparam name="TException">The type of the exception to be thrown if the condition is met. Must have a parameterless constructor.</typeparam>
    /// <param name="lValue">The left value to evaluate.</param>
    /// <param name="rValue">The right value to evaluate against.</param>
    /// <param name="callback">Optional guard callback that receives the outcome of the evaluation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GreaterThanOrEqual<TLeft, TRight, TException>([NotNull] TLeft lValue, [NotNull] TRight rValue, SGuardCallback? callback = null)
        where TLeft : IComparable<TRight> where TException : Exception, new()
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);
        
        SGuard.Guard(Is.GreaterThanOrEqual(lValue, rValue), () => Throw.That(ExceptionActivator.Create<TException>(null)), callback);
    }

    /// <summary>
    /// Throws an exception of type <typeparamref name="TException"/> if the left value is greater than or equal to the right value.
    /// The exception is created using the provided constructor arguments.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value, which implements IComparable.</typeparam>
    /// <typeparam name="TRight">The type of the right value, which is comparable to TLeft.</typeparam>
    /// <typeparam name="TException">The type of the exception to be thrown if the condition is met.</typeparam>
    /// <param name="lValue">The left value to evaluate.</param>
    /// <param name="rValue">The right value to evaluate against.</param>
    /// <param name="constructorArgs">The arguments to pass to the constructor of the exception.</param>
    /// <param name="callback">Optional guard callback that receives the outcome of the evaluation.</param>
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GreaterThanOrEqual<TLeft, TRight, TException>([NotNull] TLeft lValue, [NotNull] TRight rValue, object[]? constructorArgs,
                                                                     SGuardCallback? callback = null)
        where TLeft : IComparable<TRight> where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);
        
        SGuard.Guard(Is.GreaterThanOrEqual(lValue, rValue), () => Throw.That(ExceptionActivator.Create<TException>(constructorArgs)), callback);
    }
}