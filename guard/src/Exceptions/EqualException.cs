using System.Runtime.CompilerServices;

namespace SGuard.Exceptions;

/// <summary>
/// The exception that is thrown when a value is equal to another value.
/// </summary>
[Serializable]
public sealed class EqualException : ArgumentOutOfRangeException
{
    private readonly string? _paramName;

    /// <summary>
    /// Gets the call-site expression of the checked argument, or <c>null</c> when the exception was created with a message.
    /// </summary>
    public override string? ParamName => _paramName;

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualException"/> class.
    /// </summary>
    public EqualException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public EqualException(string message) : base(null, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualException"/> class with a specified error message and a reference to the
    /// inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public EqualException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualException"/> class with a message that names both expressions. The values
    /// are included only when <see cref="SGuardOptions.IncludeValuesInExceptions"/> is enabled.
    /// </summary>
    public EqualException(object? left, object? right, [CallerArgumentExpression("left")] string? leftExpr = null,
        [CallerArgumentExpression("right")] string? rightExpr = null)
        : base(null, BuildMessage(left, right, leftExpr, rightExpr))
    {
        _paramName = leftExpr;
        ExceptionValues.AddTo(Data, "left", left);
        ExceptionValues.AddTo(Data, "right", right);
        Data["leftExpr"] = leftExpr;
        Data["rightExpr"] = rightExpr;
    }

    private static string BuildMessage(object? left, object? right, string? leftExpr, string? rightExpr)
    {
        return (ExceptionValues.Included
                   ? $"'{ExceptionValues.Format(left)}' is equal to '{ExceptionValues.Format(right)}'. "
                   : "Left value is equal to right value. ") +
               $"Actual: left={leftExpr}, right={rightExpr}.";
    }
}
