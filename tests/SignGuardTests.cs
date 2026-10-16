using System.Numerics;
using SGuard.Exceptions;

namespace SGuard.Tests;

// Asserts exact messages, so it can't run while another test turns IncludeValuesInExceptions on.
[Collection(nameof(SGuardOptionsCollection))]
public sealed class SignGuardTests
{
    private sealed class DomainException(string message) : Exception(message);

    // ------------------------- Negative -------------------------

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void ThrowIfNegative_NegativeInt_Throws(int value)
    {
        var ex = Assert.Throws<NegativeException>(() => ThrowIf.Negative(value));

        Assert.Equal("value", ex.ParamName);
        Assert.Equal("Value 'value' is negative.", ex.Message);
        Assert.IsAssignableFrom<ArgumentOutOfRangeException>(ex);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void ThrowIfNegative_ZeroOrPositive_ReturnsTheValue(int value)
    {
        Assert.Equal(value, ThrowIf.Negative(value));
    }

    [Fact]
    public void ThrowIfNegative_WorksWithOtherNumericTypes()
    {
        Assert.Equal(1.5m, ThrowIf.Negative(1.5m));
        Assert.Throws<NegativeException>(() => ThrowIf.Negative(-0.01m));
        Assert.Throws<NegativeException>(() => ThrowIf.Negative(-1L));
        Assert.Throws<NegativeException>(() => ThrowIf.Negative((Half)(-1)));
        Assert.Throws<NegativeException>(() => ThrowIf.Negative(BigInteger.MinusOne));
        Assert.Equal(Int128.MaxValue, ThrowIf.Negative(Int128.MaxValue));
    }

    [Fact]
    public void ThrowIfNegative_NaNFails_NegativeZeroPasses()
    {
        Assert.Throws<NegativeException>(() => ThrowIf.Negative(double.NaN));
        Assert.Throws<NegativeException>(() => ThrowIf.Negative(float.NaN));
        Assert.Equal(-0.0, ThrowIf.Negative(-0.0));
    }

    [Fact]
    public void IsNegative_ReturnsTheResult_AndFalseForNaN()
    {
        Assert.True(Is.Negative(-1));
        Assert.False(Is.Negative(0));
        Assert.False(Is.Negative(-0.0));
        Assert.False(Is.Negative(double.NaN));
    }

    [Fact]
    public void ThrowIfNegative_Factory_RunsOnlyOnFailure()
    {
        var calls = 0;

        Assert.Equal(5, ThrowIf.Negative(5, () => { calls++; return new DomainException("negative"); }));
        Assert.Throws<DomainException>(() => ThrowIf.Negative(-5, () => { calls++; return new DomainException("negative"); }));
        Assert.Throws<DomainException>(() => ThrowIf.Negative(double.NaN, () => new DomainException("nan")));

        Assert.Equal(1, calls);
    }

    // ------------------------- NegativeOrZero -------------------------

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ThrowIfNegativeOrZero_Throws(int value)
    {
        var ex = Assert.Throws<NegativeOrZeroException>(() => ThrowIf.NegativeOrZero(value));

        Assert.Equal("Value 'value' is negative or zero.", ex.Message);
        Assert.IsAssignableFrom<ArgumentOutOfRangeException>(ex);
    }

    [Fact]
    public void ThrowIfNegativeOrZero_Positive_ReturnsTheValue()
    {
        Assert.Equal(1, ThrowIf.NegativeOrZero(1));
        Assert.Equal(0.001m, ThrowIf.NegativeOrZero(0.001m));
        Assert.Equal(double.Epsilon, ThrowIf.NegativeOrZero(double.Epsilon));
    }

    [Fact]
    public void ThrowIfNegativeOrZero_NaNAndNegativeZeroFail()
    {
        Assert.Throws<NegativeOrZeroException>(() => ThrowIf.NegativeOrZero(double.NaN));
        Assert.Throws<NegativeOrZeroException>(() => ThrowIf.NegativeOrZero(-0.0));
        Assert.False(Is.NegativeOrZero(double.NaN));
        Assert.True(Is.NegativeOrZero(-0.0));
    }

    [Fact]
    public void ThrowIfNegativeOrZero_Factory()
    {
        Assert.Equal(2, ThrowIf.NegativeOrZero(2, () => new DomainException("x")));
        Assert.Throws<DomainException>(() => ThrowIf.NegativeOrZero(0, () => new DomainException("x")));
    }

    // ------------------------- Zero -------------------------

    [Fact]
    public void ThrowIfZero_Zero_Throws()
    {
        var divisor = 0;

        var ex = Assert.Throws<ZeroException>(() => ThrowIf.Zero(divisor));

        Assert.Equal("divisor", ex.ParamName);
        Assert.Equal("Value 'divisor' is zero.", ex.Message);
        Assert.IsAssignableFrom<ArgumentOutOfRangeException>(ex);
    }

    [Fact]
    public void ThrowIfZero_NegativeZeroIsZero_NaNIsNot()
    {
        Assert.Throws<ZeroException>(() => ThrowIf.Zero(-0.0));
        Assert.True(double.IsNaN(ThrowIf.Zero(double.NaN)));
        Assert.True(Is.Zero(-0.0));
        Assert.False(Is.Zero(double.NaN));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ThrowIfZero_NonZero_ReturnsTheValue(int value)
    {
        Assert.Equal(value, ThrowIf.Zero(value));
    }

    [Fact]
    public void ThrowIfZero_Factory()
    {
        Assert.Equal(1m, ThrowIf.Zero(1m, () => new DomainException("x")));
        Assert.Throws<DomainException>(() => ThrowIf.Zero(0m, () => new DomainException("x")));
    }

    [Fact]
    public void SignGuards_ReportOutcomeToCallback()
    {
        var outcomes = new List<GuardOutcome>();
        SGuardCallback record = outcomes.Add;

        ThrowIf.Negative(1, record);
        Assert.Throws<NegativeException>(() => ThrowIf.Negative(-1, record));
        ThrowIf.NegativeOrZero(1, record);
        Assert.Throws<NegativeOrZeroException>(() => ThrowIf.NegativeOrZero(0, record));
        ThrowIf.Zero(1, record);
        Assert.Throws<ZeroException>(() => ThrowIf.Zero(0, record));

        Assert.Equal(
            [GuardOutcome.Success, GuardOutcome.Failure, GuardOutcome.Success, GuardOutcome.Failure, GuardOutcome.Success, GuardOutcome.Failure],
            outcomes);
    }

    [Fact]
    public void IsSignGuards_ReportTheResultToCallback()
    {
        GuardOutcome? outcome = null;

        Assert.True(Is.Negative(-1, o => outcome = o));
        Assert.Equal(GuardOutcome.Success, outcome);
        Assert.False(Is.NegativeOrZero(1, o => outcome = o));
        Assert.Equal(GuardOutcome.Failure, outcome);
        Assert.True(Is.Zero(0, o => outcome = o));
        Assert.Equal(GuardOutcome.Success, outcome);
    }
}
