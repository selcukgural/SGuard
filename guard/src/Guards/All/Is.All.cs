namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// Determines whether all elements in a given sequence satisfy a specified condition.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The sequence of elements to be tested.</param>
    /// <param name="predicate">A function that tests each element for a condition.</param>
    /// <param name="callback">
    /// An optional callback that is invoked with the outcome of the guard evaluation.
    /// If all elements satisfy the condition, <see cref="GuardOutcome.Success"/> is passed; otherwise, <see cref="GuardOutcome.Failure"/> is passed.
    /// </param>
    /// <returns>
    /// True if all elements in the sequence satisfy the condition specified by the predicate; otherwise, false.
    /// </returns>
    public static bool All<T>(IEnumerable<T> source, Func<T, bool> predicate, SGuardCallback? callback = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        var result = source.All(predicate);

        SGuard.InvokeCallbackSafely(result, callback);
        
        return result;
    }
}