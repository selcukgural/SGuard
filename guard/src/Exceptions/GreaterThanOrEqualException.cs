using System.Runtime.CompilerServices;

namespace SGuard.Exceptions;


/// <summary>
/// Represents an exception that thrown when a value is greater than or equal to a specified value.
/// </summary>
[Serializable]
public sealed class GreaterThanOrEqualException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanOrEqualException"/> class.
    /// </summary>
    public GreaterThanOrEqualException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanOrEqualException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public GreaterThanOrEqualException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanOrEqualException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="inner">The exception that is the cause of the current exception or a null reference if no inner exception is specified.</param>
    public GreaterThanOrEqualException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// Represents an exception that is thrown when a value is greater than or equal to a specified value.
    /// </summary>
    public GreaterThanOrEqualException(object? left, object? right, [CallerArgumentExpression("left")] string? leftExpr  = null,
                                       [CallerArgumentExpression("right")] string? rightExpr = null)
        : base(BuildMessage(left, right, leftExpr, rightExpr))
    {
        Data["left"]     = left;
        Data["right"]    = right;
        Data["leftExpr"] = leftExpr;
        Data["rightExpr"] = rightExpr;
    }

    /// <summary>
    /// Constructs an error message for a "greater than or equal" violation using the provided values and expressions.
    /// </summary>
    /// <param name="left">The value on the left side of the comparison.</param>
    /// <param name="right">The value on the right side of the comparison.</param>
    /// <param name="leftExpr">The string representation of the left expression, captured at the call site.</param>
    /// <param name="rightExpr">The string representation of the right expression, captured at the call site.</param>
    /// <returns>A formatted error message string describing the violation.</returns>
    private static string BuildMessage(object? left, object? right, string? leftExpr, string? rightExpr)
    {
        return $"'{left}' is greater than or equal to '{right}'. Actual: left={leftExpr}, right={rightExpr}.";
    }
}