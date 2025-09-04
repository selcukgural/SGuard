namespace SGuard;


/// <summary>
/// Outcome of a guard evaluation.
/// </summary>
public enum GuardOutcome
{
    Success,
    Failure
}

/// <summary>
/// Signature for guard callbacks. Receives the outcome of the guard.
/// </summary>
public delegate void SGuardCallback(GuardOutcome outcome);

/// <summary>
/// Helper factory methods for building guard callbacks.
/// </summary>
public static class SGuardCallbacks
{
    /// <summary>
    /// Invokes the action only when outcome is Success.
    /// </summary>
    public static SGuardCallback OnSuccess(Action action)
        => outcome => { if (outcome == GuardOutcome.Success) action(); };

    /// <summary>
    /// Invokes the action only when outcome is Failure.
    /// </summary>
    public static SGuardCallback OnFailure(Action action)
        => outcome => { if (outcome == GuardOutcome.Failure) action(); };
}