namespace SGuard.Exceptions;

/// <summary>
/// Represents an exception raised when a specific error
/// or general issue occurs within the application logic.
/// This class extends the base <see cref="System.Exception"/> type
/// and provides constructors for default and message-specific scenarios.
/// </summary>
[Serializable]
public sealed class AllException : Exception
{
    /// <summary>
    /// Represents a specific exception that inherits from the base <see cref="Exception"/> class.
    /// Used to indicate errors that relate to a specific "All" category within the application domain.
    /// </summary>
    public AllException() { }

    /// <summary>
    /// Represents an exception that is thrown for general error handling.
    /// </summary>
    public AllException(string message) : base(message) { }
}