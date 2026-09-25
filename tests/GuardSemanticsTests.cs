using SGuard.Exceptions;

namespace SGuard.Tests;

public sealed class GuardSemanticsTests
{
    [Fact]
    public void Is_Comparisons_ReturnFalse_WhenAnOperandIsNaN()
    {
        Assert.False(Is.GreaterThan(double.NaN, 100.0));
        Assert.False(Is.GreaterThanOrEqual(double.NaN, 100.0));
        Assert.False(Is.LessThan(double.NaN, 0.0));
        Assert.False(Is.LessThanOrEqual(double.NaN, 0.0));
        Assert.False(Is.LessThan(0.0, double.NaN));
        Assert.False(Is.Between(double.NaN, 0.0, 100.0));
        Assert.False(Is.Between(50.0, double.NaN, 100.0));
        Assert.False(Is.GreaterThan(float.NaN, 1f));
        Assert.False(Is.LessThan(Half.NaN, (Half)1));
    }

    [Fact]
    public void ThrowIf_Comparisons_Throw_WhenAnOperandIsNaN()
    {
        var amount = double.NaN;
        GuardOutcome? outcome = null;

        Assert.Throws<GreaterThanException>(() => ThrowIf.GreaterThan(amount, 100.0, o => outcome = o));
        Assert.Equal(GuardOutcome.Failure, outcome);
        Assert.Throws<GreaterThanOrEqualException>(() => ThrowIf.GreaterThanOrEqual(amount, 100.0));
        Assert.Throws<LessThanException>(() => ThrowIf.LessThan(amount, 0.0));
        Assert.Throws<LessThanOrEqualException>(() => ThrowIf.LessThanOrEqual(0.0, amount));
        Assert.Throws<BetweenException>(() => ThrowIf.Between(amount, 0.0, 100.0));
        Assert.Throws<InvalidOperationException>(() => ThrowIf.GreaterThan(float.NaN, 1f, new InvalidOperationException()));
        Assert.Throws<GreaterThanException>(() => ThrowIf.GreaterThan(Half.NaN, (Half)1));
    }

    [Fact]
    public void ThrowIf_Comparisons_StillPass_ForOrdinaryFloatingPointValues()
    {
        ThrowIf.GreaterThan(99.5, 100.0);
        ThrowIf.LessThan(double.PositiveInfinity, 0.0);
        ThrowIf.Between(-1.0, 0.0, 100.0);
    }

    [Fact]
    public void Between_WithReversedStringBounds_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Is.Between("m", "z", "a", StringComparison.Ordinal));
        Assert.Throws<ArgumentException>(() => ThrowIf.Between("m", "z", "a", StringComparison.Ordinal));
        Assert.True(Is.Between("M", "a", "z", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Between_WithEqualBounds_IsValid()
    {
        Assert.True(Is.Between(5, 5, 5));
        Assert.False(Is.Between(4, 5, 5));
    }

    [Fact]
    public void BuiltInExceptions_AreArgumentExceptions_NamingTheCallerExpression()
    {
        var request = new { Name = "", Age = 200 };

        var nullOrEmpty = Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty(request.Name));
        var greater = Assert.Throws<GreaterThanException>(() => ThrowIf.GreaterThan(request.Age, 130));
        var between = Assert.Throws<BetweenException>(() => ThrowIf.Between(request.Age, 150, 250));

        Assert.IsAssignableFrom<ArgumentException>(nullOrEmpty);
        Assert.Equal("request.Name", nullOrEmpty.ParamName);
        Assert.Equal("Value 'request.Name' is null or empty.", nullOrEmpty.Message);
        Assert.Equal("request.Age", greater.ParamName);
        Assert.DoesNotContain("(Parameter", greater.Message);
        Assert.Equal("request.Age", between.ParamName);
        Assert.IsAssignableFrom<ArgumentException>(Assert.Throws<AllException>(() => ThrowIf.All(new[] { 1 }, x => x > 0)));
        Assert.IsAssignableFrom<ArgumentException>(Assert.Throws<AnyException>(() => ThrowIf.Any(new[] { 1 }, x => x > 0)));
        Assert.Null(new LessThanException("custom").ParamName);
    }

    [Fact]
    public void SpanGuards_ValidateArguments_EvenWhenSourceIsEmpty()
    {
        Assert.Throws<ArgumentNullException>(() => Is.All(ReadOnlySpan<int>.Empty, null!));
        Assert.Throws<ArgumentNullException>(() => Is.Any(ReadOnlySpan<int>.Empty, null!));
        Assert.Throws<ArgumentNullException>(() => ThrowIf.Any(ReadOnlySpan<int>.Empty, x => true, (Exception)null!));
        Assert.False(Is.Any(ReadOnlySpan<int>.Empty, x => true));
        ThrowIf.Any(ReadOnlySpan<int>.Empty, x => true, new InvalidOperationException());
    }
}
