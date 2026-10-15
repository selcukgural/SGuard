using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// Determines whether the string matches the regular expression, and optionally invokes a callback with the result.
    /// </summary>
    /// <remarks>
    /// The pattern is not anchored for you: <c>[0-9]+</c> matches <c>"abc1"</c>. Use <c>^...\z</c> to match the whole
    /// string. The match timeout is the one <paramref name="regex"/> was created with; a <see cref="GeneratedRegexAttribute"/>
    /// or a cached <see cref="Regex"/> instance is the fastest option. <c>null</c> returns <c>false</c>.
    /// </remarks>
    /// <param name="value">The string to check.</param>
    /// <param name="regex">The regular expression to match.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if <paramref name="value"/> matches <paramref name="regex"/>; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="regex"/> is <c>null</c>.</exception>
    /// <exception cref="RegexMatchTimeoutException">Matching timed out. The callback is not invoked.</exception>
    public static bool Match([NotNullWhen(true)] string? value, Regex regex, SGuardCallback? callback = null)
    {
        ArgumentNullException.ThrowIfNull(regex);

        var isMatch = value is not null && regex.IsMatch(value);

        SGuard.InvokeCallbackSafely(isMatch, callback);

        return isMatch;
    }

    /// <summary>
    /// Determines whether the string matches the regular expression pattern, and optionally invokes a callback with the
    /// result. Matching is limited to <see cref="DefaultRegexTimeout"/>.
    /// </summary>
    /// <remarks>
    /// The pattern is not anchored for you: <c>[0-9]+</c> matches <c>"abc1"</c>. Use <c>^...\z</c> to match the whole
    /// string. <c>null</c> returns <c>false</c>. Don't build the pattern from user input.
    /// </remarks>
    /// <param name="value">The string to check.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="options">Options for the regular expression. Defaults to <see cref="RegexOptions.None"/>.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if <paramref name="value"/> matches <paramref name="pattern"/>; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentException"><paramref name="pattern"/> is <c>null</c>, empty or not a valid pattern.</exception>
    /// <exception cref="RegexMatchTimeoutException">Matching took longer than <see cref="DefaultRegexTimeout"/>. The
    /// callback is not invoked.</exception>
    public static bool Match([NotNullWhen(true)] string? value, string pattern, RegexOptions options = RegexOptions.None,
        SGuardCallback? callback = null)
        => Match(value, pattern, options, DefaultRegexTimeout, callback);

    /// <summary>
    /// Determines whether the string matches the regular expression pattern, with a limit on how long matching may take,
    /// and optionally invokes a callback with the result.
    /// </summary>
    /// <remarks>
    /// The pattern is not anchored for you: <c>[0-9]+</c> matches <c>"abc1"</c>. Use <c>^...\z</c> to match the whole
    /// string. <c>null</c> returns <c>false</c>. Don't build the pattern from user input.
    /// </remarks>
    /// <param name="value">The string to check.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="options">Options for the regular expression.</param>
    /// <param name="matchTimeout">
    /// The maximum time matching may take. Pass <see cref="Regex.InfiniteMatchTimeout"/> only for trusted patterns and input.
    /// </param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if <paramref name="value"/> matches <paramref name="pattern"/>; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentException"><paramref name="pattern"/> is <c>null</c>, empty or not a valid pattern.</exception>
    /// <exception cref="RegexMatchTimeoutException">Matching took longer than <paramref name="matchTimeout"/>. The callback
    /// is not invoked.</exception>
    public static bool Match([NotNullWhen(true)] string? value, string pattern, RegexOptions options, TimeSpan matchTimeout,
        SGuardCallback? callback = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(pattern);

        var isMatch = value is not null && Regex.IsMatch(value, pattern, options, matchTimeout);

        SGuard.InvokeCallbackSafely(isMatch, callback);

        return isMatch;
    }

    /// <summary>
    /// The time limit applied to <see cref="Match(string?, string, RegexOptions, SGuardCallback?)"/> when no timeout is
    /// specified.
    /// </summary>
    public static TimeSpan DefaultRegexTimeout { get; } = TimeSpan.FromSeconds(1);
}
