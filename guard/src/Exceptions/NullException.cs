namespace SGuard.Exceptions;

/// <summary>
/// The exception that is thrown when a value is null.
/// </summary>
[Serializable]
public sealed class NullException : ArgumentNullException
{
    private readonly string? _paramName;

    /// <summary>
    /// Gets the call-site expression of the checked argument, or <c>null</c> when the exception was created with a message.
    /// </summary>
    public override string? ParamName => _paramName;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullException"/> class.
    /// </summary>
    public NullException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NullException(string message) : base(null, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullException"/> class with a specified error message and a reference
    /// to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public NullException(string message, Exception inner) : base(message, inner) { }

    private NullException(string? valueExpr, string message) : base(null, message)
    {
        _paramName = valueExpr;
        Data["valueExpr"] = valueExpr;
    }

    /// <summary>
    /// Creates the exception with a message that names the expression that is null.
    /// </summary>
    internal static NullException ForExpression(string? valueExpr) => new(valueExpr, $"Value '{valueExpr}' is null.");
}
