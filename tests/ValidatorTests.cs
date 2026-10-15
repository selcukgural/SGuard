using System.Text.RegularExpressions;

namespace SGuard.Tests;

public sealed partial class ValidatorTests
{
    // ------------------------- Url -------------------------

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://example.com/path?q=1#top")]
    [InlineData("HTTPS://EXAMPLE.COM")]
    [InlineData("https://sub.example.co.uk:8443/a/b")]
    [InlineData("https://192.168.0.1")]
    [InlineData("https://[::1]/")]
    [InlineData("https://xn--bcher-kva.example")]
    public void Url_HttpOrHttpsAbsoluteUrl_IsValid(string value)
    {
        Assert.True(Is.Url(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("example.com")]
    [InlineData("/relative/path")]
    [InlineData("//example.com/path")]
    [InlineData(@"\\server\share")]
    [InlineData("javascript:alert(1)")]
    [InlineData("JavaScript:alert(1)")]
    [InlineData("data:text/html,<script>alert(1)</script>")]
    [InlineData("file:///etc/passwd")]
    [InlineData("ftp://example.com")]
    [InlineData("mailto:someone@example.com")]
    [InlineData(" https://example.com")]
    [InlineData("https://example.com ")]
    [InlineData("https://example.com\n")]
    [InlineData("https://")]
    [InlineData("http:/example.com")]
    public void Url_AnythingElse_IsInvalid(string? value)
    {
        Assert.False(Is.Url(value));
    }

    [Fact]
    public void Url_ReportsOutcomeToCallback()
    {
        var outcomes = new List<GuardOutcome>();

        Is.Url("https://example.com", outcomes.Add);
        Is.Url("javascript:alert(1)", outcomes.Add);

        Assert.Equal([GuardOutcome.Success, GuardOutcome.Failure], outcomes);
    }

    // ------------------------- CreditCard -------------------------

    [Theory]
    [InlineData("4111111111111111")]    // Visa test number
    [InlineData("4111 1111 1111 1111")]
    [InlineData("4111-1111-1111-1111")]
    [InlineData("5555555555554444")]    // Mastercard test number
    [InlineData("378282246310005")]     // Amex test number (15 digits)
    [InlineData("6011111111111117")]    // Discover test number
    [InlineData("4222222222222")]       // 13 digits
    [InlineData("6250941006528599")]    // UnionPay
    public void CreditCard_ValidLuhnNumber_IsValid(string value)
    {
        Assert.True(Is.CreditCard(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("4111111111111112")]     // Luhn checksum fails
    [InlineData("41111111111")]          // 11 digits
    [InlineData("41111111111111111111")] // 20 digits
    [InlineData("4111 1111 1111 111a")]
    [InlineData("4111  1111 1111 1111")] // two separators in a row
    [InlineData(" 4111111111111111")]
    [InlineData("4111111111111111 ")]
    [InlineData("-4111111111111111")]
    [InlineData("４１１１１１１１１１１１１１１１")] // full-width digits
    public void CreditCard_Invalid(string? value)
    {
        Assert.False(Is.CreditCard(value));
    }

    [Fact]
    public void CreditCard_IsAFormatCheckOnly()
    {
        // All zeros has a Luhn sum of 0: the checksum can't tell that no card has this number.
        Assert.True(Is.CreditCard("000000000000"));
    }

    [Fact]
    public void CreditCard_VeryLongInput_IsRejectedWithoutScanningIt()
    {
        Assert.False(Is.CreditCard(new string('4', 10_000)));
    }

    // ------------------------- Match -------------------------

    [GeneratedRegex(@"^[A-Z]{3}\z")]
    private static partial Regex CurrencyCode();

    [Fact]
    public void Match_Regex()
    {
        Assert.True(Is.Match("EUR", CurrencyCode()));
        Assert.False(Is.Match("eur", CurrencyCode()));
        Assert.False(Is.Match("EURO", CurrencyCode()));
        Assert.False(Is.Match(null, CurrencyCode()));
    }

    [Fact]
    public void Match_Pattern()
    {
        Assert.True(Is.Match("EUR", @"^[A-Z]{3}\z"));
        Assert.True(Is.Match("eur", @"^[A-Z]{3}\z", RegexOptions.IgnoreCase));
        Assert.False(Is.Match(null, @"^[A-Z]{3}\z"));
    }

    [Fact]
    public void Match_IsNotAnchoredForYou()
    {
        Assert.True(Is.Match("abc1", "[0-9]+"));
    }

    [Fact]
    public void Match_InvalidArguments_Throw()
    {
        Assert.Throws<ArgumentNullException>(() => Is.Match("a", (Regex)null!));
        Assert.Throws<ArgumentException>(() => Is.Match("a", ""));
        Assert.ThrowsAny<ArgumentException>(() => Is.Match("a", "("));
    }

    [Fact]
    public void Match_Timeout_Throws_AndSkipsTheCallback()
    {
        var called = false;
        var input = new string('a', 30) + "!";

        Assert.Throws<RegexMatchTimeoutException>(() =>
            Is.Match(input, "^(a+)+$", RegexOptions.None, TimeSpan.FromMilliseconds(10), _ => called = true));
        Assert.False(called);
    }

    [Fact]
    public void Match_DefaultTimeout_IsOneSecond()
    {
        Assert.Equal(TimeSpan.FromSeconds(1), Is.DefaultRegexTimeout);
    }

    [Fact]
    public void Match_ReportsOutcomeToCallback()
    {
        var outcomes = new List<GuardOutcome>();

        Is.Match("EUR", CurrencyCode(), outcomes.Add);
        Is.Match("eur", CurrencyCode(), outcomes.Add);

        Assert.Equal([GuardOutcome.Success, GuardOutcome.Failure], outcomes);
    }
}
