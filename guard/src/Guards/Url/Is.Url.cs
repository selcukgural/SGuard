using System.Diagnostics.CodeAnalysis;

namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// Determines whether the string is an absolute <c>http</c> or <c>https</c> URL with a host, and optionally invokes a
    /// callback with the result.
    /// </summary>
    /// <remarks>
    /// Only <c>http</c> and <c>https</c> pass, so a URL that will be rendered as a link can't carry <c>javascript:</c>,
    /// <c>data:</c>, <c>file:</c> or another scheme. Relative and protocol-relative URLs (<c>//example.com</c>) fail, and so
    /// do strings that contain white space, control characters or invisible formatting characters. <c>null</c> and
    /// <c>""</c> return <c>false</c>. The check parses the string with
    /// <see cref="Uri.TryCreate(string?, UriKind, out Uri?)"/>, so unlike most guards it allocates.
    /// </remarks>
    /// <param name="value">The string to check.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if <paramref name="value"/> is an absolute http or https URL; otherwise, <c>false</c>.</returns>
    public static bool Url([NotNullWhen(true)] string? value, SGuardCallback? callback = null)
    {
        var isUrl = IsHttpUrl(value);

        SGuard.InvokeCallbackSafely(isUrl, callback);

        return isUrl;
    }

    private static bool IsHttpUrl([NotNullWhen(true)] string? value)
    {
        // Uri.TryCreate trims white space, turns "http:\\host" into "http://host" and accepts "//host" as a UNC path on
        // some platforms, so the string must start with the scheme and its slashes, and may not contain white space,
        // control or invisible formatting characters (such as a zero-width space).
        if (string.IsNullOrEmpty(value) ||
            !(value.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        foreach (var c in value)
        {
            if (char.IsWhiteSpace(c) || char.IsControl(c) || char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.Format)
            {
                return false;
            }
        }

        return Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp) &&
               uri.Host.Length > 0;
    }
}
