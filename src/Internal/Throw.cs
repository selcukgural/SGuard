using System.Diagnostics.CodeAnalysis;
using SGuard.Exceptions;

namespace SGuard;

/// <summary>
/// An internal static class that throws exceptions for various conditions.
/// </summary>
internal static class Throw
{
    /// <summary>
    /// Throws a <see cref="BetweenException"/> with a formatted message that includes the value, minimum, and maximum bounds, as well as their respective expressions.
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
    [DoesNotReturn]
    public static void BetweenException<TValue, TMin, TMax>(TValue value, TMin min, TMax max)
    {
        throw new BetweenException(value, min, max);
    }


    /// <summary>
    /// Throws a <see cref="NullOrEmptyException"/> with a predefined message indicating that the value is null or empty.
    /// </summary>
    /// <exception cref="NullOrEmptyException">
    /// Always thrown with a message stating, "Value is null or empty".
    /// </exception>
    [DoesNotReturn]
    public static void NullOrEmptyException<T>(T value)
    {
        throw new NullOrEmptyException(value);
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
    [DoesNotReturn]
    public static void GreaterThanException<TLeft, TRight>(TLeft lValue, TRight rValue)
    {
        throw new GreaterThanException(lValue, rValue);
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
    [DoesNotReturn]
    public static void GreaterThanOrEqualException<TLeft, TRight>(TLeft lValue, TRight rValue)
    {
        throw new GreaterThanOrEqualException(lValue, rValue);
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
    [DoesNotReturn]
    public static void LessThanException<TLeft, TRight>(TLeft lValue, TRight rValue)
    {
        throw new LessThanException(lValue, rValue);
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
    [DoesNotReturn]
    public static void LessThanOrEqualException<TLeft, TRight>(TLeft lValue, TRight rValue)
    {
        throw new LessThanOrEqualException(lValue, rValue);
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
    public static void That<TException>(TException exception) where TException : Exception
    {
        throw exception;
    }
}