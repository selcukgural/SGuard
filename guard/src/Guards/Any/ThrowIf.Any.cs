using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SGuard.Exceptions;

namespace SGuard;

public sealed partial class ThrowIf
{
    /// <summary>
    /// Throws an exception if any element in the source satisfies the given predicate.
    /// </summary>
    /// <typeparam name="T">The type of elements in the source.</typeparam>
    /// <param name="source">The collection of elements to evaluate.</param>
    /// <param name="predicate">The predicate function to apply to each element.</param>
    /// <param name="callback">An optional callback invoked with the guard's outcome.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Any<T>(IEnumerable<T> source, Func<T, bool> predicate, SGuardCallback? callback = null)
    {
        Any(source, predicate, new AnyException("At least one element satisfied the given predicate."), callback);
    }

    /// <summary>
    /// Throws a specified exception if any element in the provided source sequence satisfies the given predicate.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TException">The type of the exception to be thrown if the condition is met.</typeparam>
    /// <param name="source">The sequence of elements to evaluate.</param>
    /// <param name="predicate">The function that defines the condition to test for each element.</param>
    /// <param name="exception">The exception to be thrown if the condition is met.</param>
    /// <param name="callback">An optional callback that receives the result of the guard.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/>, <paramref name="predicate"/>, or <paramref name="exception"/> is null.</exception>
    /// <exception cref="TException">Thrown if any element satisfies the provided predicate.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Any<T, TException>(IEnumerable<T> source, Func<T, bool> predicate, [NotNull] TException exception, SGuardCallback? callback = null)
        where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(exception);
        
        SGuard.Guard(Is.Any(source, predicate, callback), () => Throw.That(exception), callback);
    }
}