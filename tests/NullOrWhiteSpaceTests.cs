using SGuard.Exceptions;

namespace SGuard.Tests;

// Asserts exact messages, so it can't run while another test turns IncludeValuesInExceptions on.
[Collection(nameof(SGuardOptionsCollection))]
public sealed class NullOrWhiteSpaceTests
{
    private sealed class DomainException(string message) : Exception(message);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t\r\n")]
    [InlineData(" ")] // no-break space
    public void ThrowIfNullOrWhiteSpace_Fails(string? value)
    {
        var ex = Assert.Throws<NullOrWhiteSpaceException>(() => ThrowIf.NullOrWhiteSpace(value));

        Assert.Equal("value", ex.ParamName);
        Assert.Equal("Value 'value' is null, empty or white space.", ex.Message);
        Assert.IsAssignableFrom<ArgumentException>(ex);
    }

    [Theory]
    [InlineData("a")]
    [InlineData(" a ")]
    public void ThrowIfNullOrWhiteSpace_Text_ReturnsIt(string value)
    {
        Assert.Same(value, ThrowIf.NullOrWhiteSpace(value));
    }

    [Fact]
    public void ThrowIfNullOrWhiteSpace_Factory_RunsOnlyOnFailure()
    {
        var calls = 0;

        Assert.Equal("a", ThrowIf.NullOrWhiteSpace("a", () => { calls++; return new DomainException("blank"); }));
        Assert.Throws<DomainException>(() => ThrowIf.NullOrWhiteSpace("  ", () => { calls++; return new DomainException("blank"); }));

        Assert.Equal(1, calls);
    }

    [Fact]
    public void ThrowIfNullOrWhiteSpace_ReportsOutcomeToCallback()
    {
        GuardOutcome? outcome = null;

        ThrowIf.NullOrWhiteSpace("a", o => outcome = o);
        Assert.Equal(GuardOutcome.Success, outcome);

        Assert.Throws<NullOrWhiteSpaceException>(() => ThrowIf.NullOrWhiteSpace(" ", o => outcome = o));
        Assert.Equal(GuardOutcome.Failure, outcome);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    [InlineData("a", false)]
    public void IsNullOrWhiteSpace_ReturnsTheResult(string? value, bool expected)
    {
        GuardOutcome? outcome = null;

        Assert.Equal(expected, Is.NullOrWhiteSpace(value, o => outcome = o));
        Assert.Equal(expected ? GuardOutcome.Success : GuardOutcome.Failure, outcome);
    }
}
