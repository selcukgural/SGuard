using System.Collections;

namespace SGuard;

/// <summary>
/// Formats checked values for built-in guard exceptions according to <see cref="SGuardOptions.IncludeValuesInExceptions"/>.
/// </summary>
internal static class ExceptionValues
{
    /// <summary>
    /// Gets whether values are included in exception messages and data.
    /// </summary>
    public static bool Included => SGuardOptions.IncludeValuesInExceptions;

    /// <summary>
    /// Formats a value for an exception message: <c>null</c>, or its <see cref="object.ToString"/> result truncated to
    /// <see cref="SGuardOptions.MaxValueLength"/> characters. A throwing <c>ToString</c> never replaces the guard exception.
    /// </summary>
    public static string Format(object? value)
    {
        if (value is null)
        {
            return "null";
        }

        string? text;

        try
        {
            text = value.ToString();
        }
        catch
        {
            return $"<{value.GetType().Name}>";
        }

        if (text is null)
        {
            return string.Empty;
        }

        return text.Length <= SGuardOptions.MaxValueLength ? text : string.Concat(text.AsSpan(0, SGuardOptions.MaxValueLength), "…");
    }

    /// <summary>
    /// Adds the formatted value to the exception data when values are included.
    /// </summary>
    public static void AddTo(IDictionary data, string key, object? value)
    {
        if (Included)
        {
            data[key] = Format(value);
        }
    }
}
