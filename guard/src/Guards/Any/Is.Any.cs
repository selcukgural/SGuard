using System.Runtime.CompilerServices;

namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// Determines whether any element of a sequence satisfies a condition,
    /// with an optional callback to handle the result of the evaluation.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to check against the predicate.</param>
    /// <param name="predicate">The function to test each element for a condition.</param>
    /// <param name="callback">
    /// An optional callback that is invoked with a <see cref="GuardOutcome"/> indicating
    /// whether any element satisfied the condition (Success) or if none did (Failure).
    /// </param>
    /// <returns>
    /// true if any elements in the sequence satisfy the condition specified by the predicate; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/> or <paramref name="predicate"/> is null.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any<T>(IEnumerable<T> source, Func<T, bool> predicate, SGuardCallback? callback = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        var result = source.Any(predicate);

        SGuard.InvokeCallbackSafely(result, callback);
        
        return result;
    }
}