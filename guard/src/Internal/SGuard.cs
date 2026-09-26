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
    /// Reports the outcome of a <c>ThrowIf.*</c> guard to the callback and returns <paramref name="condition"/>. The caller
    /// throws when it returns <c>true</c>, so the callback sees <see cref="GuardOutcome.Failure"/> just before the exception.
    /// Exceptions thrown by the callback are ignored.
    /// </summary>
    /// <remarks>
    /// Guards call this in an <c>if</c> and throw from its body rather than passing a throwing lambda, so a passing guard
    /// allocates no closure or delegate and stays small enough to inline.
    /// </remarks>
    /// <param name="condition">Whether the guard fails, i.e. the caller is about to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="condition"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Fails(bool condition, SGuardCallback? callback)
    {
        if (callback is not null)
        {
            Invoke(callback, condition ? GuardOutcome.Failure : GuardOutcome.Success);
        }

        return condition;
    }

    /// <summary>
    /// Invokes the specified callback safely while handling any exceptions that may occur during the invocation.
    /// When occurs an exception during the callback invocation, the exception is ignored.
    /// </summary>
    /// <param name="condition">The condition to evaluate. Determines which outcome will be passed to the callback.</param>
    /// <param name="callback">An optional callback to be invoked with the outcome of the condition evaluation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InvokeCallbackSafely(bool condition, SGuardCallback? callback)
    {
        if (callback is not null)
        {
            Invoke(callback, condition ? GuardOutcome.Success : GuardOutcome.Failure);
        }
    }

    /// <summary>
    /// Invokes the callback and ignores any exception it throws. Kept out of line so that the exception handler doesn't
    /// stop the guards from being inlined.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Invoke(SGuardCallback callback, GuardOutcome outcome)
    {
        try
        {
            callback(outcome);
        }
        catch
        {
            // Ignore
        }
    }

    /// <summary>
    /// Determines whether the value is a floating-point NaN. Comparison guards treat NaN operands as failing: <c>Is.*</c>
    /// returns <c>false</c> and <c>ThrowIf.*</c> throws, so NaN never passes a bound check.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNaN<T>(T value) => value switch
    {
        double d => double.IsNaN(d),
        float f => float.IsNaN(f),
        Half h => Half.IsNaN(h),
        _ => false
    };

    /// <summary>
    /// Determines whether either operand is a floating-point NaN.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AnyNaN<T1, T2>(T1 first, T2 second) => IsNaN(first) || IsNaN(second);

    /// <summary>
    /// Determines whether any operand is a floating-point NaN.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AnyNaN<T1, T2, T3>(T1 first, T2 second, T3 third) => IsNaN(first) || IsNaN(second) || IsNaN(third);

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> when <paramref name="min"/> is greater than <paramref name="max"/>. Bounds of
    /// different types can't be compared and are not checked.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfInvalidRange<TMin, TMax>(TMin min, TMax max)
    {
        if (typeof(TMin) == typeof(TMax) && Comparer<TMin>.Default.Compare(min, Unsafe.As<TMax, TMin>(ref max)) > 0)
        {
            throw new ArgumentException(InvalidRangeMessage, nameof(min));
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> when <paramref name="min"/> is greater than <paramref name="max"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfInvalidRange(string min, string max, StringComparison comparison)
    {
        if (string.Compare(min, max, comparison) > 0)
        {
            throw new ArgumentException(InvalidRangeMessage, nameof(min));
        }
    }

    private const string InvalidRangeMessage = "The minimum must be less than or equal to the maximum.";
}
