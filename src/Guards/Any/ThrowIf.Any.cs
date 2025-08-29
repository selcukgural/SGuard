using System.Diagnostics.CodeAnalysis;
using SGuard.Exceptions;

namespace SGuard;

public sealed partial class ThrowIf
{
    /// <summary>
    /// Throws an exception if all elements in the provided source sequence satisfy the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of elements in the source sequence.</typeparam>
    /// <param name="source">The collection of elements to evaluate.</param>
    /// <param name="predicate">The predicate to test each element of the source against.</param>
    /// <param name="callback">
    /// Optional callback to receive the guard evaluation outcome.
    /// If not provided, no callback will be invoked.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/> or <paramref name="predicate"/> is null.
    /// </exception>
    /// <exception cref="AllException">Thrown if all elements in the source satisfy the given predicate.</exception>
    public static void All<T>(IEnumerable<T> source, Func<T, bool> predicate, SGuardCallback? callback = null)
    {
        All(source, predicate, new AllException("All elements satisfied the given predicate."), callback);
    }

    /// <summary>
    /// Checks if all elements in the specified source satisfy the given predicate.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
    /// <typeparam name="TException">The type of exception to throw if the condition is met.</typeparam>
    /// <param name="source">The source collection to be evaluated.</param>
    /// <param name="predicate">The predicate function to test each element.</param>
    /// <param name="exception">The exception to throw if all elements in the source satisfy the predicate.</param>
    /// <param name="callback">An optional callback invoked with the guard outcome.</param>
    /// <exception cref="ArgumentNullException">Thrown if the source, predicate, or exception is null.</exception>
    /// <exception cref="TException">
    /// Thrown when all elements in the source satisfy the given predicate.
    /// </exception>
    public static void All<T, TException>(IEnumerable<T> source, Func<T, bool> predicate, [NotNull] TException exception, SGuardCallback? callback = null)
        where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(exception);

        SGuard.Guard(Is.All(source, predicate, callback), () => Throw.That(exception), callback);
    }
}