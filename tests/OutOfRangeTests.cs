using SGuard.Exceptions;

namespace SGuard.Tests;

// Asserts exact messages, so it can't run while another test turns IncludeValuesInExceptions on.
[Collection(nameof(SGuardOptionsCollection))]
public sealed class OutOfRangeTests
{
    private sealed class DomainException(string message) : Exception(message);

    [Theory]
    [InlineData(0)]
    [InlineData(65536)]
    [InlineData(int.MinValue)]
    public void ThrowIfOutOfRange_OutsideTheRange_Throws(int port)
    {
        var ex = Assert.Throws<OutOfRangeException>(() => ThrowIf.OutOfRange(port, 1, 65535));

        Assert.Equal("port", ex.ParamName);
        Assert.Equal("Value is outside the specified bounds. Actual: value=port, min=1, max=65535.", ex.Message);
        Assert.IsAssignableFrom<ArgumentOutOfRangeException>(ex);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(80)]
    [InlineData(65535)]
    public void ThrowIfOutOfRange_InsideTheRangeOrOnABound_ReturnsTheValue(int port)
    {
        Assert.Equal(port, ThrowIf.OutOfRange(port, 1, 65535));
    }

    [Fact]
    public void ThrowIfOutOfRange_IsTheOppositeOfBetween()
    {
        for (var value = -2; value <= 12; value++)
        {
            var between = Is.Between(value, 0, 10);

            Assert.Equal(!between, Is.OutOfRange(value, 0, 10));

            var outOfRangeThrew = Record.Exception(() => ThrowIf.OutOfRange(value, 0, 10)) is not null;
            var betweenThrew = Record.Exception(() => ThrowIf.Between(value, 0, 10)) is not null;
            Assert.NotEqual(outOfRangeThrew, betweenThrew);
        }
    }

    [Fact]
    public void ThrowIfOutOfRange_NaN_Fails()
    {
        Assert.Throws<OutOfRangeException>(() => ThrowIf.OutOfRange(double.NaN, 0d, 1d));
        Assert.Throws<OutOfRangeException>(() => ThrowIf.OutOfRange(0.5, double.NaN, 1d));
        Assert.Throws<OutOfRangeException>(() => ThrowIf.OutOfRange(0.5, 0d, double.NaN));
        Assert.True(Is.OutOfRange(double.NaN, 0d, 1d));
    }

    [Fact]
    public void ThrowIfOutOfRange_InvalidRange_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => ThrowIf.OutOfRange(5, 10, 1));

        Assert.Equal("min", ex.ParamName);
    }

    [Fact]
    public void ThrowIfOutOfRange_NullOperand_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ThrowIf.OutOfRange<string, string, string>(null!, "a", "z"));
        Assert.Throws<ArgumentNullException>(() => Is.OutOfRange<string, string, string>("m", null!, "z"));
    }

    [Fact]
    public void ThrowIfOutOfRange_OtherComparableTypes()
    {
        var timeout = TimeSpan.FromMinutes(5);

        Assert.Throws<OutOfRangeException>(() => ThrowIf.OutOfRange(timeout, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(2)));
        Assert.Equal(new DateOnly(2026, 1, 1), ThrowIf.OutOfRange(new DateOnly(2026, 1, 1), DateOnly.MinValue, DateOnly.MaxValue));
        Assert.Equal(2.5m, ThrowIf.OutOfRange(2.5m, 1m, 3m));
    }

    [Fact]
    public void ThrowIfOutOfRange_String_UsesTheComparison()
    {
        Assert.Equal("m", ThrowIf.OutOfRange("m", "a", "z", StringComparison.Ordinal));
        Assert.Throws<OutOfRangeException>(() => ThrowIf.OutOfRange("M", "a", "z", StringComparison.Ordinal));
        Assert.Equal("M", ThrowIf.OutOfRange("M", "a", "z", StringComparison.OrdinalIgnoreCase));
        Assert.True(Is.OutOfRange("M", "a", "z", StringComparison.Ordinal));
        Assert.False(Is.OutOfRange("M", "a", "z", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ThrowIfOutOfRange_String_InvalidRange_Throws()
    {
        Assert.Throws<ArgumentException>(() => ThrowIf.OutOfRange("m", "z", "a", StringComparison.Ordinal));
    }

    [Fact]
    public void ThrowIfOutOfRange_Factory_RunsOnlyOnFailure()
    {
        var calls = 0;

        Assert.Equal(5, ThrowIf.OutOfRange(5, 1, 10, () => { calls++; return new DomainException("range"); }));
        Assert.Throws<DomainException>(() => ThrowIf.OutOfRange(50, 1, 10, () => { calls++; return new DomainException("range"); }));
        Assert.Equal("m", ThrowIf.OutOfRange("m", "a", "z", StringComparison.Ordinal, () => { calls++; return new DomainException("range"); }));
        Assert.Throws<DomainException>(() =>
            ThrowIf.OutOfRange("M", "a", "z", StringComparison.Ordinal, () => { calls++; return new DomainException("range"); }));

        Assert.Equal(2, calls);
    }

    [Fact]
    public void ThrowIfOutOfRange_ReportsOutcomeToCallback()
    {
        var outcomes = new List<GuardOutcome>();

        ThrowIf.OutOfRange(5, 1, 10, outcomes.Add);
        Assert.Throws<OutOfRangeException>(() => ThrowIf.OutOfRange(0, 1, 10, outcomes.Add));
        Is.OutOfRange(0, 1, 10, outcomes.Add);
        Is.OutOfRange(5, 1, 10, outcomes.Add);

        Assert.Equal([GuardOutcome.Success, GuardOutcome.Failure, GuardOutcome.Success, GuardOutcome.Failure], outcomes);
    }
}
