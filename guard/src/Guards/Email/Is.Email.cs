using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// Validates if the provided string is a valid email address based on a predefined regular expression.
    /// <b>This email validation pattern may not fully validate email addresses, according to RFC standards.
    /// If you require strict RFC-compliant validation, use the method overload that accepts a custom regex pattern.</b>
    /// Default <see cref="RegexOptions"/> is <see cref="RegexOptions.Compiled"/> and <see cref="RegexOptions.IgnoreCase"/>
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
        
        var isEmail = DefaultRegexPattern().IsMatch(email);
        SGuard.InvokeCallbackSafely(isEmail, callback);

        return isEmail;
    }


    /// <summary>
    /// Validates if the provided string is a valid email address based on the given regular expression.
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Email(string email, string regex, RegexOptions? regexOptions = null,
        SGuardCallback? callback = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);
        ArgumentException.ThrowIfNullOrEmpty(regex);

        var isEmail = Regex.IsMatch(email, regex, regexOptions ?? RegexOptions.None);

        SGuard.InvokeCallbackSafely(isEmail, callback);

        return isEmail;
    }

    [GeneratedRegex(
        @"^(?=.{1,254}$)(?=.{1,64}@)(?!.*\.\.)(?!\.)(?!.*\.$)[A-Za-z0-9._%+-]+@(?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?\.)+[A-Za-z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    internal static partial Regex DefaultRegexPattern();
}