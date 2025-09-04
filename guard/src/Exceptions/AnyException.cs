namespace SGuard.Exceptions;

/// <summary>
/// Represents a specific exception that can be thrown to indicate a particular type of error.
/// </summary>
[Serializable]
public sealed class AnyException : Exception
{
    /// <summary>
    /// Represents a general exception that can be used to signal errors or unexpected conditions within the application.
    /// </summary>
    public AnyException() { }

    /// <summary>
    /// Represents a generic exception thrown by the application.
    /// </summary>
    public AnyException(string message) : base(message) { }
}