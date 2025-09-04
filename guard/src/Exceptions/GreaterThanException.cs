using System.Runtime.CompilerServices;

namespace SGuard.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a value is greater than a specified value
/// </summary>
[Serializable]
public sealed class GreaterThanException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanException"/> class.
    /// </summary>
    public GreaterThanException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public GreaterThanException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="inner">The exception that is the cause of the current exception or a null reference if no inner exception is specified.</param>
    public GreaterThanException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// Represents an exception that is thrown when a value is greater than a specified value.
    /// </summary>
    public GreaterThanException(object? left, object? right, [CallerArgumentExpression("left")]  string? leftExpr  = null,
                                [CallerArgumentExpression("right")] string? rightExpr = null)
        : base(BuildMessage(left, right, leftExpr, rightExpr))
    {
        Data["left"]     = left;
        Data["right"]    = right;
        Data["leftExpr"] = leftExpr;
        Data["rightExpr"] = rightExpr;
    }

    /// <summary>
    /// Constructs a formatted error message for a "greater than" exception based on the provided values and their expressions.
    /// </summary>
    /// <param name="left">The left operand of the comparison.</param>
    /// <param name="right">The right operand of the comparison.</param>
    /// <param name="leftExpr">The string representation of the left operand expression.</param>
    /// <param name="rightExpr">The string representation of the right operand expression.</param>
    /// <returns>A formatted string describing the "greater than" violation.</returns>
    private static string BuildMessage(object? left, object? right, string? leftExpr, string? rightExpr)
    {
        return $"'{left}' is greater than '{right}'. Actual: left={leftExpr}, right={rightExpr}.";
    }
}