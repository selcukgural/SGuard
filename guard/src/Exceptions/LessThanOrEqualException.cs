using System.Runtime.CompilerServices;

namespace SGuard.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a value is less than or equal to a specified value.
/// </summary>
[Serializable]
public sealed class LessThanOrEqualException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanOrEqualException"/> class.
    /// </summary>
    public LessThanOrEqualException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanOrEqualException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public LessThanOrEqualException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanOrEqualException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="inner">The exception that is the cause of the current exception or a null reference if no inner exception is specified.</param>
    public LessThanOrEqualException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// Represents an exception that is thrown when a value is less than or equal to a specified value.
    /// </summary>
    public LessThanOrEqualException(
        object? left,
        object? right,
        [CallerArgumentExpression("left")]  string? leftExpr  = null,
        [CallerArgumentExpression("right")] string? rightExpr = null)
        : base(BuildMessage(left, right, leftExpr, rightExpr))
    {
        Data["left"]      = left;
        Data["right"]     = right;
        Data["leftExpr"]  = leftExpr;
        Data["rightExpr"] = rightExpr;
    }

    /// <summary>
    /// Builds an error message indicating that the left value is less than or equal to the right value, including expressions and actual values.
    /// </summary>
    /// <param name="left">The value on the left-hand side of the comparison.</param>
    /// <param name="right">The value on the right-hand side of the comparison.</param>
    /// <param name="leftExpr">The string representation of the left-hand side expression.</param>
    /// <param name="rightExpr">The string representation of the right-hand side expression.</param>
    /// <returns>A formatted error message describing the comparison failure.</returns>
    private static string BuildMessage(object? left, object? right, string? leftExpr, string? rightExpr)
        => $"'{left}' is less than or equal to '{right}'. Actual: left={leftExpr}, right={rightExpr}.";

}