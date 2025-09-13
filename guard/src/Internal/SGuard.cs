using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SGuard;

/// <summary>
/// Provides functionality for enforcing guard clauses, ensuring that
/// specified conditions are checked and exceptions are thrown when necessary.
/// This static class is internally used to validate conditions and invoke
/// appropriate actions or callbacks based on the outcome of those validations.
/// </summary>
internal static class SGuard
{
    /// <summary>
    /// Enforces a guard clause that throws an exception if a specified condition is met.
    /// When occur an exception during the callback invocation, the exception is ignored.
    /// </summary>
    /// <param name="condition">The condition to evaluate. If true, the specified exception-throwing action will be invoked.</param>
    /// <param name="throwAction">The action to execute if the condition is met, typically throwing an exception.</param>
    /// <param name="callback">An optional callback invoked with the result of the guard check, indicating success or failure.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Guard([DoesNotReturnIf(true)] bool condition, Action throwAction, SGuardCallback? callback)
    {
        try
        {
            if (condition)
            {
                throwAction();
            }
        }
        finally
        {
            InvokeCallbackSafely(condition, callback, GuardOutcome.Failure, GuardOutcome.Success);
        }
    }

    /// <summary>
    /// Invokes the specified callback safely while handling any exceptions that may occur during the invocation.
    /// When occur an exception during the callback invocation, the exception is ignored.
    /// </summary>
    /// <param name="condition">The condition to evaluate. Determines which outcome will be passed to the callback.</param>
    /// <param name="callback">An optional callback to be invoked with the outcome of the condition evaluation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InvokeCallbackSafely(bool condition, SGuardCallback? callback)=> InvokeCallbackSafely(condition, callback, GuardOutcome.Success, GuardOutcome.Failure);

    /// <summary>
    /// Invokes the specified callback safely while handling any exceptions that may occur during the invocation.
    /// When occur an exception during the callback invocation, the exception is ignored.
    /// </summary>
    /// <param name="condition">The condition used to determine the outcome passed to the callback.</param>
    /// <param name="callback">The callback to be invoked. Receives the outcome based on the provided condition.</param>
    /// <param name="successOutcome">The <see cref="GuardOutcome"/> value to pass to the callback if the condition is true.</param>
    /// <param name="failureOutcome">The <see cref="GuardOutcome"/> value to pass to the callback if the condition is false.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InvokeCallbackSafely(bool condition, SGuardCallback? callback, GuardOutcome successOutcome, GuardOutcome failureOutcome)
    {
        try
        {
            callback?.Invoke(condition ? successOutcome : failureOutcome);
        }
        catch
        {
            // Ignore
        }
    }
}