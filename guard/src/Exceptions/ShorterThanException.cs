using System.Runtime.CompilerServices;

namespace SGuard.Exceptions;

/// <summary>
/// The exception that is thrown when a string is shorter than the minimum length.
/// </summary>
[Serializable]
public sealed class ShorterThanException : ArgumentOutOfRangeException
{
    private readonly string? _paramName;

    /// <summary>
    /// Gets the call-site expression of the checked argument, or <c>null</c> when the exception was created with a message.
    /// </summary>
    public override string? ParamName => _paramName;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShorterThanException"/> class.
    /// </summary>
    public ShorterThanException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShorterThanException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ShorterThanException(string message) : base(null, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShorterThanException"/> class with a specified error message and a reference to the
    /// inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public ShorterThanException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShorterThanException"/> class with a message that names the string and limit
    /// expressions. The actual length is included only when <see cref="SGuardOptions.IncludeValuesInExceptions"/> is
    /// enabled; the string itself never is.
    /// </summary>
    /// <param name="length">The length of the string that failed the guard.</param>
    /// <param name="minLength">The length limit.</param>
    /// <param name="valueExpr">The expression of the checked string, captured at the call site.</param>
    /// <param name="minLengthExpr">The expression passed as <paramref name="minLength"/>, captured by the compiler.</param>
    public ShorterThanException(int length, int minLength, string? valueExpr, [CallerArgumentExpression("minLength")] string? minLengthExpr = null)
        : base(null, BuildMessage(length, valueExpr, minLengthExpr))
    {
        _paramName = valueExpr;
        ExceptionValues.AddTo(Data, "length", length);
        ExceptionValues.AddTo(Data, "minLength", minLength);
        Data["valueExpr"] = valueExpr;
        Data["minLengthExpr"] = minLengthExpr;
    }

    private static string BuildMessage(int length, string? valueExpr, string? limitExpr)
    {
        var message = $"Value '{valueExpr}' is shorter than '{limitExpr}' characters.";
        return ExceptionValues.Included ? $"{message} Actual: length={length}." : message;
    }
}
