using SGuard.Exceptions;

namespace SGuard.Tests;

// Asserts exact messages, so it can't run while another test turns IncludeValuesInExceptions on.
[Collection(nameof(SGuardOptionsCollection))]
public sealed class LengthGuardTests
{
    private sealed class DomainException(string message) : Exception(message);

    private const int MaxNameLength = 5;

    [Fact]
    public void ThrowIfLongerThan_TooLong_Throws_WithoutTheValue()
    {
        var name = "secret-name";

        var ex = Assert.Throws<LongerThanException>(() => ThrowIf.LongerThan(name, MaxNameLength));

        Assert.Equal("name", ex.ParamName);
        Assert.Equal("Value 'name' is longer than 'MaxNameLength' characters.", ex.Message);
        Assert.DoesNotContain("secret", ex.Message);
        Assert.IsAssignableFrom<ArgumentOutOfRangeException>(ex);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abcde")]
    public void ThrowIfLongerThan_AtMostTheLimit_ReturnsTheString(string value)
    {
        Assert.Same(value, ThrowIf.LongerThan(value, MaxNameLength));
    }

    [Fact]
    public void ThrowIfShorterThan_TooShort_Throws()
    {
        var password = "abc";

        var ex = Assert.Throws<ShorterThanException>(() => ThrowIf.ShorterThan(password, 8));

        Assert.Equal("password", ex.ParamName);
        Assert.Equal("Value 'password' is shorter than '8' characters.", ex.Message);
        Assert.DoesNotContain("abc", ex.Message);
    }

    [Theory]
    [InlineData("abcdefgh")]
    [InlineData("abcdefghijk")]
    public void ThrowIfShorterThan_AtLeastTheLimit_ReturnsTheString(string value)
    {
        Assert.Same(value, ThrowIf.ShorterThan(value, 8));
    }

    [Fact]
    public void LengthGuards_CountUtf16CodeUnits()
    {
        const string emoji = "😀"; // one code point, two UTF-16 code units

        Assert.True(Is.LongerThan(emoji, 1));
        Assert.False(Is.ShorterThan(emoji, 2));
    }

    [Fact]
    public void LengthGuards_NullString_ThrowsArgumentNullException_NamingTheExpression()
    {
        string? name = null;

        var ex = Assert.Throws<ArgumentNullException>(() => ThrowIf.LongerThan(name!, 5));

        Assert.Equal("name", ex.ParamName);
        Assert.Throws<ArgumentNullException>(() => ThrowIf.ShorterThan(name!, 5));
        Assert.Throws<ArgumentNullException>(() => Is.LongerThan(name!, 5));
        Assert.Throws<ArgumentNullException>(() => Is.ShorterThan(name!, 5));
    }

    [Fact]
    public void LengthGuards_NegativeLimit_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ThrowIf.LongerThan("a", -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => ThrowIf.ShorterThan("a", -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => Is.LongerThan("a", -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => Is.ShorterThan("a", -1));
    }

    [Fact]
    public void LengthGuards_Factory_RunOnlyOnFailure()
    {
        var calls = 0;
        DomainException Create() { calls++; return new DomainException("length"); }

        ThrowIf.LongerThan("abc", 3, Create);
        Assert.Throws<DomainException>(() => ThrowIf.LongerThan("abcd", 3, Create));
        ThrowIf.ShorterThan("abc", 3, Create);
        Assert.Throws<DomainException>(() => ThrowIf.ShorterThan("ab", 3, Create));

        Assert.Equal(2, calls);
    }

    [Fact]
    public void LengthGuards_ReportOutcomeToCallback()
    {
        var outcomes = new List<GuardOutcome>();

        ThrowIf.LongerThan("abc", 3, outcomes.Add);
        Assert.Throws<LongerThanException>(() => ThrowIf.LongerThan("abcd", 3, outcomes.Add));
        Is.ShorterThan("ab", 3, outcomes.Add);
        Is.ShorterThan("abc", 3, outcomes.Add);

        Assert.Equal([GuardOutcome.Success, GuardOutcome.Failure, GuardOutcome.Success, GuardOutcome.Failure], outcomes);
    }
}
