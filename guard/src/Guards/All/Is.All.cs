using System.Linq.Expressions;
using System.Runtime.CompilerServices;

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool All<T>(IEnumerable<T> source, Func<T, bool> predicate, SGuardCallback? callback = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        var result = source.All(predicate);

        SGuard.InvokeCallbackSafely(result, callback);
        
        return result;
    }

    /// <summary>
    /// Determines whether all elements in a given span satisfy a specified condition.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="source">The span of elements to be tested.</param>
    /// <param name="predicate">A function that tests each element for a condition.</param>
    /// <param name="callback">
    /// An optional callback that is invoked with the outcome of the guard evaluation.
    /// If all elements satisfy the condition, <see cref="GuardOutcome.Success"/> is passed; otherwise, <see cref="GuardOutcome.Failure"/> is passed.
    /// </param>
    /// <returns>
    /// True if all elements in the span satisfy the condition specified by the predicate; otherwise, false.
    /// </returns>
    public static bool All<T>(ReadOnlySpan<T> source, Func<T, bool> predicate, SGuardCallback? callback = null)
    {
        if (source.IsEmpty)
        {
            SGuard.InvokeCallbackSafely(false, callback);
            return false;
        }
        
        ArgumentNullException.ThrowIfNull(predicate);
        
        foreach (var src in source)
        {
            if (predicate(src))
            {
                continue;
            }
            
            SGuard.InvokeCallbackSafely(false, callback);
            return false;
        }

        SGuard.InvokeCallbackSafely(true, callback);
        return true;
    }
}