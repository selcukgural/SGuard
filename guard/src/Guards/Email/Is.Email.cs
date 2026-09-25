using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// Validates if the provided string is a valid email address based on a predefined regular expression.
    /// <b>This email validation pattern may not fully validate email addresses, according to RFC standards.
    /// If you require strict RFC-compliant validation, use the method overload that accepts a custom regex pattern.</b>
    /// Only ASCII addresses of at most 254 characters are accepted; line breaks and other trailing characters are rejected.
    /// </summary>
    /// <param name="email">The string to validate as an email address.</param>
    /// <param name="callback">
    /// An optional callback that will be invoked with the outcome of the validation.
    /// The callback receives a <see cref="GuardOutcome"/> indicating whether the validation succeeded or failed.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the provided string is a valid email address.
    /// Returns <c>true</c> if the string matches the email format; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Email(string email, SGuardCallback? callback = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);

        var isEmail = email.Length <= MaxEmailLength && DefaultRegexPattern().IsMatch(email);
        SGuard.InvokeCallbackSafely(isEmail, callback);

        return isEmail;
    }


    /// <summary>
    /// Validates if the provided string is a valid email address based on the given regular expression.
    /// Matching is limited to <see cref="DefaultEmailRegexTimeout"/>; use the overload that accepts a timeout to change it.
    /// </summary>
    /// <param name="email">The string to validate as an email address.</param>
    /// <param name="regex">The custom regular expression to be used for email validation.</param>
    /// <param name="regexOptions">
    /// Optional regular expression options that control the behavior of the regex matching process.
    /// Defaults to <see cref="RegexOptions.None"/> if not provided.
    /// </param>
    /// <param name="callback">
    /// An optional callback that will be invoked with the result of the validation.
    /// The callback receives a <see cref="GuardOutcome"/> indicating whether the validation succeeded or failed.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the provided string is a valid email address, according to the given regex.
    /// Returns <c>true</c> if the string matches the custom email format; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="RegexMatchTimeoutException">
    /// Thrown when matching takes longer than <see cref="DefaultEmailRegexTimeout"/>. The callback is not invoked.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Email(string email, string regex, RegexOptions? regexOptions = null,
        SGuardCallback? callback = null)
        => Email(email, regex, regexOptions ?? RegexOptions.None, DefaultEmailRegexTimeout, callback);

    /// <summary>
    /// Validates if the provided string is a valid email address based on the given regular expression, with a limit on
    /// how long matching may take.
    /// </summary>
    /// <param name="email">The string to validate as an email address.</param>
    /// <param name="regex">The custom regular expression to be used for email validation.</param>
    /// <param name="regexOptions">Regular expression options that control the behavior of the regex matching process.</param>
    /// <param name="matchTimeout">
    /// The maximum time matching may take. Pass <see cref="Regex.InfiniteMatchTimeout"/> only for trusted patterns and input.
    /// </param>
    /// <param name="callback">
    /// An optional callback that will be invoked with the result of the validation.
    /// The callback receives a <see cref="GuardOutcome"/> indicating whether the validation succeeded or failed.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the provided string is a valid email address, according to the given regex.
    /// Returns <c>true</c> if the string matches the custom email format; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="RegexMatchTimeoutException">
    /// Thrown when matching takes longer than <paramref name="matchTimeout"/>. The callback is not invoked.
    /// </exception>
    public static bool Email(string email, string regex, RegexOptions regexOptions, TimeSpan matchTimeout,
        SGuardCallback? callback = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);
        ArgumentException.ThrowIfNullOrEmpty(regex);

        var isEmail = Regex.IsMatch(email, regex, regexOptions, matchTimeout);

        SGuard.InvokeCallbackSafely(isEmail, callback);

        return isEmail;
    }

    /// <summary>
    /// The time limit applied to custom email patterns when no timeout is specified.
    /// </summary>
    public static TimeSpan DefaultEmailRegexTimeout { get; } = TimeSpan.FromSeconds(1);

    private const int MaxEmailLength = 254;

    // \z instead of $ so that a trailing line break is rejected, and no IgnoreCase: the classes already list both cases, and
    // case-insensitive matching would let non-ASCII characters such as the Kelvin sign (U+212A) match [A-Za-z].
    [GeneratedRegex(
        @"^(?=.{1,254}\z)(?=.{1,64}@)(?!.*\.\.)(?!\.)(?!.*\.\z)[A-Za-z0-9._%+-]+@(?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?\.)+[A-Za-z]{2,}\z",
        RegexOptions.CultureInvariant)]
    internal static partial Regex DefaultRegexPattern();
}