namespace SGuard;

/// <summary>
/// Provides methods to dynamically create instances of exception types.
/// </summary>
/// <remarks>
/// This static class is designed to simplify the creation of exception instances
/// by dynamically invoking their constructors with the specified arguments.
/// It ensures that the created exception is of the expected type and provides
/// detailed error messages when the creation fails.
/// </remarks>
public static class ExceptionActivator
{
    /// <summary>
    /// Creates an instance of the specified exception type using the provided arguments.
    /// </summary>
    /// <typeparam name="TException">
    /// The type of the exception to create. Must inherit from <see cref="Exception"/>.
    /// </typeparam>
    /// <param name="args">
    /// An array of arguments to pass to the exception's constructor. Can be null or empty.
    /// </param>
    /// <returns>
    /// Returns an instance of the specified exception type <typeparamref name="TException"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the instance could not be created or if no matching constructor is found 
    /// for the provided arguments.
    /// </exception>
    public static TException Create<TException>(object?[]? args) where TException : Exception
    {
        try
        {
            return (TException?)Activator.CreateInstance(typeof(TException), args ?? Array.Empty<object>()) ??
                   throw new InvalidOperationException($"Could not create instance of '{typeof(TException).FullName}'.");
        }
        catch (MissingMethodException)
        {
            throw new InvalidOperationException(
                $"No matching constructor found for '{typeof(TException).FullName}' with the provided argument list.");
        }
    }
}