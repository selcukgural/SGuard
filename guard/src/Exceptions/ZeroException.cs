using System.Runtime.CompilerServices;

namespace SGuard.Exceptions;

/// <summary>
/// The exception that is thrown when a value is zero.
/// </summary>
[Serializable]
public sealed class ZeroException : ArgumentOutOfRangeException
{
    private readonly string? _paramName;

    /// <summary>
    /// Gets the call-site expression of the checked argument, or <c>null</c> when the exception was created with a message.
    /// </summary>
    public override string? ParamName => _paramName;

    /// <summary>
    /// Initializes a new instance of the <see cref="ZeroException"/> class.
    /// </summary>
    public ZeroException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZeroException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ZeroException(string message) : base(null, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZeroException"/> class with a specified error message and a reference to the
    /// inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public ZeroException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZeroException"/> class with a message that names the checked expression.
    /// The value is included only when <see cref="SGuardOptions.IncludeValuesInExceptions"/> is enabled.
    /// </summary>
    /// <param name="value">The value that failed the guard.</param>
    /// <param name="valueExpr">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    public ZeroException(object? value, [CallerArgumentExpression("value")] string? valueExpr = null)
        : base(null, BuildMessage(value, valueExpr))
    {
        _paramName = valueExpr;
        ExceptionValues.AddTo(Data, "value", value);
        Data["valueExpr"] = valueExpr;
    }

    private static string BuildMessage(object? value, string? valueExpr)
    {
        var message = $"Value '{valueExpr}' is zero.";
        return ExceptionValues.Included ? $"{message} Actual: value={ExceptionValues.Format(value)}." : message;
    }
}
