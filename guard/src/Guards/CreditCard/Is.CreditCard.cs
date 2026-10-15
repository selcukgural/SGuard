using System.Diagnostics.CodeAnalysis;

namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// Determines whether the string looks like a payment card number: 12 to 19 digits that pass the Luhn checksum, and
    /// optionally invokes a callback with the result.
    /// </summary>
    /// <remarks>
    /// Single spaces or hyphens between digits are allowed (<c>4111 1111 1111 1111</c>, <c>4111-1111-1111-1111</c>); any
    /// other character fails. The check doesn't tell whether the card exists, which network issued it, or whether it can
    /// be charged. <c>null</c> and <c>""</c> return <c>false</c>. Card numbers are sensitive: don't log them.
    /// </remarks>
    /// <param name="value">The string to check.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if <paramref name="value"/> has 12 to 19 digits that pass the Luhn checksum; otherwise,
    /// <c>false</c>.</returns>
    public static bool CreditCard([NotNullWhen(true)] string? value, SGuardCallback? callback = null)
    {
        var isCreditCard = IsLuhnCardNumber(value);

        SGuard.InvokeCallbackSafely(isCreditCard, callback);

        return isCreditCard;
    }

    private const int MinCardDigits = 12;
    private const int MaxCardDigits = 19;

    private static bool IsLuhnCardNumber([NotNullWhen(true)] string? value)
    {
        // 19 digits with a separator between each pair is the longest string that can pass.
        if (string.IsNullOrEmpty(value) || value.Length > MaxCardDigits * 2 - 1)
        {
            return false;
        }

        var digits = 0;
        var sum = 0;
        var previousWasSeparator = true; // a separator can't come first

        // Walk from the right: every second digit, starting with the one left of the check digit, is doubled.
        for (var i = value.Length - 1; i >= 0; i--)
        {
            var c = value[i];

            if (c is ' ' or '-')
            {
                if (previousWasSeparator)
                {
                    return false;
                }

                previousWasSeparator = true;
                continue;
            }

            if (!char.IsAsciiDigit(c))
            {
                return false;
            }

            var digit = c - '0';

            if (digits % 2 == 1)
            {
                digit *= 2;

                if (digit > 9)
                {
                    digit -= 9;
                }
            }

            sum += digit;
            digits++;
            previousWasSeparator = false;
        }

        return !previousWasSeparator && digits is >= MinCardDigits and <= MaxCardDigits && sum % 10 == 0;
    }
}
