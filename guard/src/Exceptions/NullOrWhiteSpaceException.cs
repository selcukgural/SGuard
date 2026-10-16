using System.Runtime.CompilerServices;

namespace SGuard.Exceptions;

/// <summary>
/// The exception that is thrown when a string is null, empty or consists only of white-space characters.
/// </summary>
[Serializable]
public sealed class NullOrWhiteSpaceException : ArgumentException
{
    private readonly string? _paramName;

    /// <summary>
    /// Gets the call-site expression of the checked argument, or <c>null</c> when the exception was created with a message.
    /// </summary>
    public override string? ParamName => _paramName;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullOrWhiteSpaceException"/> class.
    /// </summary>
    public NullOrWhiteSpaceException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullOrWhiteSpaceException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NullOrWhiteSpaceException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullOrWhiteSpaceException"/> class with a specified error message and
    /// a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public NullOrWhiteSpaceException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullOrWhiteSpaceException"/> class with a message that names the
    /// checked expression. The value is included only when <see cref="SGuardOptions.IncludeValuesInExceptions"/> is enabled.
    /// </summary>
    /// <param name="value">The value that failed the guard.</param>
    /// <param name="valueExpr">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    public NullOrWhiteSpaceException(string? value, [CallerArgumentExpression("value")] string? valueExpr = null)
        : base(BuildMessage(value, valueExpr))
    {
        _paramName = valueExpr;
        ExceptionValues.AddTo(Data, "value", value);
        Data["valueExpr"] = valueExpr;
    }

    private static string BuildMessage(string? value, string? valueExpr)
    {
        var message = $"Value '{valueExpr}' is null, empty or white space.";
        return ExceptionValues.Included ? $"{message} Actual: value={ExceptionValues.Format(value)}." : message;
    }
}
