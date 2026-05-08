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
}
