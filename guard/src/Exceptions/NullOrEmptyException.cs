using System.Runtime.CompilerServices;

namespace SGuard.Exceptions;

/// <summary>
/// The exception that is thrown when all the objects in an array are null or empty.
/// </summary>
[Serializable]
public sealed class NullOrEmptyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NullOrEmptyException"/> class.
    /// </summary>
    public NullOrEmptyException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullOrEmptyException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NullOrEmptyException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullOrEmptyException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="inner">The exception that is the cause of the current exception or a null reference if no inner exception is specified.</param>
    public NullOrEmptyException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// The exception that is thrown when all the objects in an array are null or empty.
    /// </summary>
    public NullOrEmptyException(object? value, [CallerArgumentExpression("value")] string? valueExpr = null) : base(BuildMessage(value, valueExpr))
    {
        Data["value"] = value;
        Data["valueExpr"] = valueExpr;
    }

    /// <summary>
    /// Builds a detailed error message describing a value that is null or empty and
    /// includes the expression name from the calling site.
    /// </summary>
    /// <param name="value">The value that was evaluated as null or empty.</param>
    /// <param name="valueExpr">The expression name of the value, captured at the call-site.</param>
    /// <returns>A formatted string containing details about the null or empty value.</returns>
    private static string BuildMessage(object? value, string? valueExpr)
    {
        return $"Value '{valueExpr}' is null or empty. Actual: value={value}.";
    }
}