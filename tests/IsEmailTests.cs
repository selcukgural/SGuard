using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SGuard.Tests;

public sealed class IsEmailTests
{
    [Theory]
    [InlineData("plainaddress")]
    [InlineData("#@%^%#$@#$@#.com")]
    [InlineData("@example.com")]
    [InlineData("Joe Smith <email@example.com>")]
    [InlineData("email.example.com")]
    [InlineData("email@example@example.com")]
    [InlineData(".email@example.com")]
    [InlineData("email..email@example.com")]
    [InlineData("あいうえお@example.com")]
    [InlineData("email@example.com (Joe Smith)")]
    [InlineData("email@example")]
    [InlineData("email@-example.com")]
    [InlineData("email@111.222.333.44444")]
    [InlineData("email@example..com")]
    [InlineData("Abc..123@example.com")]
    [InlineData("email@example.com\n")]
    [InlineData("email@example.com\r\n")]
    [InlineData("email@example.com\nBcc: other@example.com")]
    [InlineData("email@exampl\u212A.com")]
    [InlineData("email@example.co\u212A")]
    [InlineData("\u212Aelvin@example.com")]
    public void Email_ReturnsFalse_ForInvalidEmails(string email)
    {
        Assert.False(Is.Email(email));
    }

    [Fact]
    public void Email_Throws_WhenValueIsNullOrEmpty()
    {
        Assert.Throws<ArgumentNullException>(() => Is.Email(null!));
        Assert.Throws<ArgumentException>(() => Is.Email(string.Empty));
    }

    [Fact]
    public void Email_WithCustomRegex_ReturnsTrue_WhenMatches()
    {
        var email = "test@example.com";
        var regex = @"^.*@.*$"; // Simple regex
        Assert.True(Is.Email(email, regex));
    }

    [Fact]
    public void Email_WithCustomRegex_ReturnsFalse_WhenDoesNotMatch()
    {
        var email = "test_example.com";
        var regex = @"^.*@.*$";
        Assert.False(Is.Email(email, regex));
    }

    [Fact]
    public void Email_WithCustomRegex_Throws_WhenRegexIsNullOrEmpty()
    {
        Assert.Throws<ArgumentException>(() => Is.Email("test@example.com", regex: string.Empty));
        Assert.Throws<ArgumentNullException>(() => Is.Email("test@example.com", regex: null!));
    }

    [Fact]
    public void Email_InvokesCallback()
    {
        bool? observed = null;
        SGuardCallback cb = outcome => observed = outcome == GuardOutcome.Success;

        Is.Email("test@example.com", cb);
        Assert.True(observed);

        Is.Email("invalid", cb);
        Assert.False(observed);
    }

    [Fact]
    public void Email_DoesNotThrow_WhenCallbackThrows()
    {
        SGuardCallback cb = _ => throw new InvalidOperationException("boom");
        
        var result1 = Is.Email("test@example.com", cb);
        Assert.True(result1);

        var result2 = Is.Email("invalid", cb);
        Assert.False(result2);
    }

    [Theory]
    [InlineData("email@example.com")]
    [InlineData("EMAIL@EXAMPLE.COM")]
    [InlineData("first.last+tag@sub.example.co")]
    public void Email_ReturnsTrue_ForValidEmails(string email)
    {
        Assert.True(Is.Email(email));
    }

    [Fact]
    public void Email_ReturnsFalse_WhenLongerThan254Characters()
    {
        var domain = string.Join('.', Enumerable.Repeat(new string('a', 63), 4)) + ".com";
        Assert.False(Is.Email("a@" + domain));
        Assert.False(Is.Email(new string('a', 100_000)));
    }

    [Fact]
    public void Email_WithCustomRegex_ThrowsOnTimeout_WithoutInvokingCallback()
    {
        var invoked = false;
        var input = new string('a', 64) + "!";
        var stopwatch = Stopwatch.StartNew();

        Assert.Throws<RegexMatchTimeoutException>(() =>
            Is.Email(input, "^(a+)+$", RegexOptions.None, TimeSpan.FromMilliseconds(50), _ => invoked = true));

        Assert.False(invoked);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void Email_WithCustomRegex_AppliesDefaultTimeout()
    {
        Assert.Equal(TimeSpan.FromSeconds(1), Is.DefaultEmailRegexTimeout);
        Assert.Throws<RegexMatchTimeoutException>(() => Is.Email(new string('a', 64) + "!", "^(a+)+$"));
    }

    [Fact]
    public void Email_WithCustomRegexAndTimeout_ReturnsResult()
    {
        Assert.True(Is.Email("test@example.com", "^.*@.*$", RegexOptions.None, TimeSpan.FromSeconds(1)));
        Assert.False(Is.Email("test_example.com", "^.*@.*$", RegexOptions.None, TimeSpan.FromSeconds(1)));
    }
}
