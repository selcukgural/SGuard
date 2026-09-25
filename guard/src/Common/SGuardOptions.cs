namespace SGuard;

/// <summary>
/// Global settings for SGuard. Set them once at application startup.
/// </summary>
public static class SGuardOptions
{
    /// <summary>
    /// Gets or sets whether built-in guard exceptions include the checked values in <see cref="Exception.Message"/> and
    /// <see cref="Exception.Data"/>. Defaults to <c>false</c>.
    /// </summary>
    /// <remarks>
    /// Exception messages usually end up in logs and error trackers, so values are left out by default: they may be
    /// passwords, tokens or personal data. Messages always contain the argument expressions captured at the call site
    /// (for example <c>request.Age</c>). When enabled, each value is written with <see cref="object.ToString"/>, truncated to
    /// <see cref="MaxValueLength"/> characters, and <see cref="Exception.Data"/> holds these strings rather than the values.
    /// </remarks>
    public static bool IncludeValuesInExceptions { get; set; }

    /// <summary>
    /// The maximum number of characters written for each value when <see cref="IncludeValuesInExceptions"/> is enabled.
    /// </summary>
    public const int MaxValueLength = 64;
}
