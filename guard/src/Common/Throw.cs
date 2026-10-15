using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SGuard.Exceptions;

namespace SGuard;

/// <summary>
/// A static class that throws exceptions for various conditions.
/// This class provides a set of static methods to throw specific exceptions
/// based on various conditions, such as range checks, null or empty values,
/// and comparison operations. It is designed to centralize exception throwing
/// logic and improve code readability and maintainability.
/// </summary>
/// <remarks>
/// The methods in this class are marked with the <see cref="System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute"/> attribute,
/// indicating that they always throw an exception and do not return to the caller.
/// </remarks>
// ReSharper disable once ClassNeverInstantiated.Global
public static class Throw
{
    /// <summary>
    /// Throws a <see cref="BetweenException"/> whose message names the value, minimum and maximum expressions; the values themselves are included only when <see cref="SGuardOptions.IncludeValuesInExceptions"/> is enabled.
    /// </summary>
    /// <typeparam name="TValue">The type of the value being checked.</typeparam>
    /// <typeparam name="TMin">The type of the minimum bound.</typeparam>
    /// <typeparam name="TMax">The type of the maximum bound.</typeparam>
    /// <param name="value">The value that caused the exception.</param>
    /// <param name="min">The minimum bound of the range.</param>
    /// <param name="max">The maximum bound of the range.</param>
    /// <exception cref="BetweenException">
    /// Thrown when the value is not within the specified range indicated by the minimum and maximum bounds.
    /// </exception>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    /// <param name="minExpression">The expression passed as <paramref name="min"/>, captured by the compiler.</param>
    /// <param name="maxExpression">The expression passed as <paramref name="max"/>, captured by the compiler.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BetweenException<TValue, TMin, TMax>(TValue value, TMin min, TMax max,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null,
        [CallerArgumentExpression(nameof(min))] string? minExpression = null,
        [CallerArgumentExpression(nameof(max))] string? maxExpression = null)
    {
        throw new BetweenException(value, min, max, valueExpression, minExpression, maxExpression);
    }

    /// <summary>
    /// Throws a <see cref="NullOrEmptyException"/> with a message naming the expression that is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value that is null or empty.</param>
    /// <exception cref="NullOrEmptyException">Always thrown.</exception>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NullOrEmptyException<T>(T value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
    {
        throw new NullOrEmptyException(value, valueExpression);
    }

    /// <summary>
    /// Throws a <see cref="NullOrEmptyException"/> naming the expression, for a value that isn't available to include.
    /// </summary>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void NullOrEmptyExceptionWithoutValue(string? valueExpression)
    {
        throw Exceptions.NullOrEmptyException.ForExpression(valueExpression);
    }

    /// <summary>
    /// Throws a <see cref="GreaterThanException"/> with a message indicating that the left value is greater than the right value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value being compared.</typeparam>
    /// <typeparam name="TRight">The type of the right value being compared.</typeparam>
    /// <param name="lValue">The left value in the comparison.</param>
    /// <param name="rValue">The right value in the comparison.</param>
    /// <exception cref="GreaterThanException">
    /// Always thrown with a message stating that the left value is greater than the right value.
    /// </exception>
    /// <param name="lValueExpression">The expression passed as <paramref name="lValue"/>, captured by the compiler.</param>
    /// <param name="rValueExpression">The expression passed as <paramref name="rValue"/>, captured by the compiler.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GreaterThanException<TLeft, TRight>(TLeft lValue, TRight rValue,
        [CallerArgumentExpression(nameof(lValue))] string? lValueExpression = null,
        [CallerArgumentExpression(nameof(rValue))] string? rValueExpression = null)
    {
        throw new GreaterThanException(lValue, rValue, lValueExpression, rValueExpression);
    }

    /// <summary>
    /// Throws a <see cref="GreaterThanOrEqualException"/> with a message indicating that the left value 
    /// is greater than or equal to the right value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value being compared.</typeparam>
    /// <typeparam name="TRight">The type of the right value being compared.</typeparam>
    /// <param name="lValue">The left value in the comparison.</param>
    /// <param name="rValue">The right value in the comparison.</param>
    /// <exception cref="GreaterThanOrEqualException">
    /// Always thrown with a message stating that the left value is greater than or equal to the right value.
    /// </exception>
    /// <param name="lValueExpression">The expression passed as <paramref name="lValue"/>, captured by the compiler.</param>
    /// <param name="rValueExpression">The expression passed as <paramref name="rValue"/>, captured by the compiler.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GreaterThanOrEqualException<TLeft, TRight>(TLeft lValue, TRight rValue,
        [CallerArgumentExpression(nameof(lValue))] string? lValueExpression = null,
        [CallerArgumentExpression(nameof(rValue))] string? rValueExpression = null)
    {
        throw new GreaterThanOrEqualException(lValue, rValue, lValueExpression, rValueExpression);
    }

    /// <summary>
    /// Throws a <see cref="LessThanException"/> with a message indicating that the left value 
    /// is less than the right value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value being compared.</typeparam>
    /// <typeparam name="TRight">The type of the right value being compared.</typeparam>
    /// <param name="lValue">The left value in the comparison.</param>
    /// <param name="rValue">The right value in the comparison.</param>
    /// <exception cref="LessThanException">
    /// Always thrown with a message stating that the left value is less than the right value.
    /// </exception>
    /// <param name="lValueExpression">The expression passed as <paramref name="lValue"/>, captured by the compiler.</param>
    /// <param name="rValueExpression">The expression passed as <paramref name="rValue"/>, captured by the compiler.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LessThanException<TLeft, TRight>(TLeft lValue, TRight rValue,
        [CallerArgumentExpression(nameof(lValue))] string? lValueExpression = null,
        [CallerArgumentExpression(nameof(rValue))] string? rValueExpression = null)
    {
        throw new LessThanException(lValue, rValue, lValueExpression, rValueExpression);
    }

    /// <summary>
    /// Throws a <see cref="LessThanOrEqualException"/> with a message indicating that the left value
    /// is less than or equal to the right value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value being compared.</typeparam>
    /// <typeparam name="TRight">The type of the right value being compared.</typeparam>
    /// <param name="lValue">The left value in the comparison.</param>
    /// <param name="rValue">The right value in the comparison.</param>
    /// <exception cref="LessThanOrEqualException">
    /// Always thrown with a message stating that the left value is less than or equal to the right value.
    /// </exception>
    /// <param name="lValueExpression">The expression passed as <paramref name="lValue"/>, captured by the compiler.</param>
    /// <param name="rValueExpression">The expression passed as <paramref name="rValue"/>, captured by the compiler.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LessThanOrEqualException<TLeft, TRight>(TLeft lValue, TRight rValue,
        [CallerArgumentExpression(nameof(lValue))] string? lValueExpression = null,
        [CallerArgumentExpression(nameof(rValue))] string? rValueExpression = null)
    {
        throw new LessThanOrEqualException(lValue, rValue, lValueExpression, rValueExpression);
    }

    /// <summary>
    /// Throws the specified exception.
    /// </summary>
    /// <typeparam name="TException">The type of the exception to throw. Must inherit from <see cref="Exception"/>.</typeparam>
    /// <param name="exception">The exception instance to throw.</param>
    /// <exception>Always thrown with the provided exception instance.
    ///     <cref>TException</cref>
    /// </exception>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void That<TException>(TException exception) where TException : Exception
    {
        throw exception;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/>. Kept out of line so that a guard calling it
    /// stays small enough to inline, and the factory runs only when the guard fails.
    /// </summary>
    /// <exception cref="InvalidOperationException">The factory returned <c>null</c>.</exception>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void From<TException>(Func<TException> exceptionFactory) where TException : Exception
    {
        throw exceptionFactory() ?? throw new InvalidOperationException("The exception factory returned null.");
    }

    // The helpers below create the built-in exceptions of the guards added in 1.0. They are kept out of line, like the
    // factory above, so that the exception's construction doesn't stop a guard from being inlined.

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void NullException(string? valueExpression) => throw Exceptions.NullException.ForExpression(valueExpression);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void NullOrWhiteSpaceException(string? value, string? valueExpression)
        => throw new NullOrWhiteSpaceException(value, valueExpression);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void NegativeException<T>(T value, string? valueExpression) => throw new NegativeException(value, valueExpression);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void NegativeOrZeroException<T>(T value, string? valueExpression)
        => throw new NegativeOrZeroException(value, valueExpression);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ZeroException<T>(T value, string? valueExpression) => throw new ZeroException(value, valueExpression);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void OutOfRangeException<TValue, TMin, TMax>(TValue value, TMin min, TMax max, string? valueExpression,
        string? minExpression, string? maxExpression)
        => throw new OutOfRangeException(value, min, max, valueExpression, minExpression, maxExpression);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void EqualException<T>(T lValue, T rValue, string? lValueExpression, string? rValueExpression)
        => throw new EqualException(lValue, rValue, lValueExpression, rValueExpression);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void NotEqualException<T>(T lValue, T rValue, string? lValueExpression, string? rValueExpression)
        => throw new NotEqualException(lValue, rValue, lValueExpression, rValueExpression);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void NotDefinedException<TEnum>(TEnum value, string? valueExpression) where TEnum : struct, Enum
        => throw new NotDefinedException(value, valueExpression);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void LongerThanException(int length, int maxLength, string? valueExpression, string? maxLengthExpression)
        => throw new LongerThanException(length, maxLength, valueExpression, maxLengthExpression);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ShorterThanException(int length, int minLength, string? valueExpression, string? minLengthExpression)
        => throw new ShorterThanException(length, minLength, valueExpression, minLengthExpression);
}