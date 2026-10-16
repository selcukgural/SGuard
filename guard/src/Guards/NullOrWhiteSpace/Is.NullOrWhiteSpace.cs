using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// Determines whether the string is <c>null</c>, empty or consists only of white-space characters, and optionally
    /// invokes a callback with the result.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if <paramref name="value"/> is <c>null</c>, empty or white space; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NullOrWhiteSpace([NotNullWhen(false)] string? value, SGuardCallback? callback = null)
    {
        var result = string.IsNullOrWhiteSpace(value);

        SGuard.InvokeCallbackSafely(result, callback);

        return result;
    }
}
