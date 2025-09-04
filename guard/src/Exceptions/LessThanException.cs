using System.Runtime.CompilerServices;

namespace SGuard.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a value is less than a specified value.
/// </summary>
[Serializable]
public sealed class LessThanException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanException"/> class.
    /// </summary>
    public LessThanException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public LessThanException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="inner">The exception that is the cause of the current exception or a null reference if no inner exception is specified.</param>
    public LessThanException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// Represents an exception that is thrown when a value is less than another specified value.
    /// </summary>
    public LessThanException(
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
    /// Constructs a detailed error message for the <see cref="LessThanException"/> exception.
    /// </summary>
    /// <param name="left">The value of the left operand involved in the comparison.</param>
    /// <param name="right">The value of the right operand involved in the comparison.</param>
    /// <param name="leftExpr">The string representation of the left operand's name or expression.</param>
    /// <param name="rightExpr">The string representation of the right operand's name or expression.</param>
    /// <returns>A formatted string describing the less-than condition that caused the exception.</returns>
    private static string BuildMessage(object? left, object? right, string? leftExpr, string? rightExpr)
        => $"'{left}' is less than '{right}'. Actual: left={leftExpr}, right={rightExpr}.";

}