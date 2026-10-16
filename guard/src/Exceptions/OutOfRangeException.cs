using System.Runtime.CompilerServices;

namespace SGuard.Exceptions;

/// <summary>
/// The exception that is thrown when a value is outside the specified range.
/// </summary>
[Serializable]
public sealed class OutOfRangeException : ArgumentOutOfRangeException
{
    private readonly string? _paramName;

    /// <summary>
    /// Gets the call-site expression of the checked argument, or <c>null</c> when the exception was created with a message.
    /// </summary>
    public override string? ParamName => _paramName;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutOfRangeException"/> class.
    /// </summary>
    public OutOfRangeException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OutOfRangeException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public OutOfRangeException(string message) : base(null, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OutOfRangeException"/> class with a specified error message and a
    /// reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public OutOfRangeException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OutOfRangeException"/> class with a message that names the value,
    /// minimum and maximum expressions. The values are included only when
    /// <see cref="SGuardOptions.IncludeValuesInExceptions"/> is enabled.
    /// </summary>
    public OutOfRangeException(
        object? value,
        object? min,
        object? max,
        [CallerArgumentExpression("value")] string? valueExpr = null,
        [CallerArgumentExpression("min")]   string? minExpr   = null,
        [CallerArgumentExpression("max")]   string? maxExpr   = null)
        : base(null, BuildMessage(value, min, max, valueExpr, minExpr, maxExpr))
    {
        _paramName = valueExpr;
        ExceptionValues.AddTo(Data, "value", value);
        ExceptionValues.AddTo(Data, "min", min);
        ExceptionValues.AddTo(Data, "max", max);
        Data["valueExpr"] = valueExpr;
        Data["minExpr"]   = minExpr;
        Data["maxExpr"]   = maxExpr;
    }

    private static string BuildMessage(object? value, object? min, object? max, string? valueExpr, string? minExpr, string? maxExpr)
    {
        return (ExceptionValues.Included
                   ? $"Value '{ExceptionValues.Format(value)}' is not between '{ExceptionValues.Format(min)}' and '{ExceptionValues.Format(max)}'. "
                   : "Value is outside the specified bounds. ") +
               $"Actual: value={valueExpr}, min={minExpr}, max={maxExpr}.";
    }
}
