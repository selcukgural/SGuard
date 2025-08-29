namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// Determines whether the left value is less than the right value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value to compare.</typeparam>
    /// <typeparam name="TRight">The type of the right value to compare.</typeparam>
    /// <param name="lValue">The left value to compare. Must not be null.</param>
    /// <param name="rValue">The right value to compare. Must not be null.</param>
    /// <param name="callback">
    /// An optional callback to invoke with the result of the comparison.
    /// The callback is invoked with <c>true</c> if the left value is less than the right value; otherwise, <c>false</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the left value is less than the right value; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="lValue"/> or <paramref name="rValue"/> is null.
    /// </exception>
    public static bool LessThan<TLeft, TRight>(TLeft lValue, TRight rValue, SGuardCallback? callback = null) where TLeft : IComparable<TRight>
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);

        var isLessThan = lValue.CompareTo(rValue) < 0;
        
        SGuard.InvokeCallbackSafely(isLessThan, callback);
        
        return isLessThan;
    }
    
    /// <summary>
    /// Determines whether the left string is less than the right string using the specified StringComparison.
    /// </summary>
    /// <param name="lValue">The left string to compare. Must not be null.</param>
    /// <param name="rValue">The right string to compare. Must not be null.</param>
    /// <param name="comparison">The string comparison rule to use.</param>
    /// <param name="callback">Optional callback invoked with the outcome.</param>
    /// <returns>true if lValue is less than rValue according to the comparison; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown if lValue or rValue is null.</exception>
    public static bool LessThan(string lValue, string rValue, StringComparison comparison, SGuardCallback? callback = null)
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);
        
        var isLessThan = string.Compare(lValue, rValue, comparison) < 0;

        SGuard.InvokeCallbackSafely(isLessThan, callback);
        
        return isLessThan;
    }



    /// <summary>
    /// Determines whether the left value is less than or equal to the right value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value to compare.</typeparam>
    /// <typeparam name="TRight">The type of the right value to compare.</typeparam>
    /// <param name="lValue">The left value to compare. Must not be null.</param>
    /// <param name="rValue">The right value to compare. Must not be null.</param>
    /// <param name="callback">
    /// An optional callback to invoke with the result of the comparison.
    /// The callback is invoked with <c>true</c> if the left value is less than or equal to the right value; otherwise, <c>false</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the left value is less than or equal to the right value; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="lValue"/> or <paramref name="rValue"/> is null.
    /// </exception>
    public static bool LessThanOrEqual<TLeft, TRight>(TLeft lValue, TRight rValue, SGuardCallback? callback = null) where TLeft : IComparable<TRight>
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);
        
        var isLessThanOrEqual = lValue.CompareTo(rValue) <= 0;
        
        SGuard.InvokeCallbackSafely(isLessThanOrEqual, callback);
        
        return isLessThanOrEqual;
    }
    
    /// <summary>
    /// Determines whether the left string is less than or equal to the right string using the specified StringComparison.
    /// </summary>
    /// <param name="lValue">The left string to compare. Must not be null.</param>
    /// <param name="rValue">The right string to compare. Must not be null.</param>
    /// <param name="comparison">The string comparison rule to use.</param>
    /// <param name="callback">Optional callback invoked with the outcome.</param>
    /// <returns>true if lValue is less than or equal to rValue according to the comparison; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown if lValue or rValue is null.</exception>
    public static bool LessThanOrEqual(string lValue, string rValue, StringComparison comparison, SGuardCallback? callback = null)
    {
        ArgumentNullException.ThrowIfNull(lValue);
        ArgumentNullException.ThrowIfNull(rValue);
        
        var isLessThanOrEqual = string.Compare(lValue, rValue, comparison) <= 0;
        
        SGuard.InvokeCallbackSafely(isLessThanOrEqual, callback);
        
        return isLessThanOrEqual;
    }
}