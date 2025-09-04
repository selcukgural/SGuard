using SGuard.Exceptions;

namespace SGuard.Tests;

public sealed class ThrowIfGreaterThanTests
{
    private class CustomException : Exception
    {
        public CustomException() { }
        public CustomException(string message) : base(message) { }
    }

    private sealed class CallbackProbe
    {
        public bool Called { get; private set; }
        public GuardOutcome? Outcome { get; private set; }

        public SGuardCallback Create()
        {
            return outcome =>
            {
                Called = true;
                Outcome = outcome;
            };
        }
    }

    // Cross-type comparable
    private struct MyComparable : IComparable<int>
    {
        public int Inner;
        public int CompareTo(int other) => Inner.CompareTo(other);
    }

    // ------------------------- GreaterThan (built-in) -------------------------

    [Theory]
    [InlineData(2, 1)]
    [InlineData(10, -1)]
    [InlineData(0, -1)]
    public void GreaterThan_WhenLeftGreaterThanRight_ThrowsGreaterThanException_AndReportsFailure(int left, int right)
    {
        var probe = new CallbackProbe();

        Assert.Throws<GreaterThanException>(() => ThrowIf.GreaterThan(left, right, probe.Create()));

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(5, 5)]
    public void GreaterThan_WhenLeftNotGreaterThanRight_DoesNotThrow_AndReportsSuccess(int left, int right)
    {
        var probe = new CallbackProbe();

        ThrowIf.GreaterThan(left, right, probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void GreaterThan_WithCustomExceptionInstance_ThrowsGivenException_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var ex = new CustomException("gt");

        var thrown = Assert.Throws<CustomException>(() => ThrowIf.GreaterThan(10, 1, ex, probe.Create()));

        Assert.Same(ex, thrown);
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void GreaterThan_WithCustomExceptionInstance_ValidComparison_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();

        ThrowIf.GreaterThan(1, 10, new CustomException(), probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void GreaterThan_WithGenericExceptionNew_ThrowsCustomException_AndReportsFailure()
    {
        var probe = new CallbackProbe();

        Assert.Throws<CustomException>(() => ThrowIf.GreaterThan<int, int, CustomException>(100, 1, probe.Create()));

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void GreaterThan_WithGenericExceptionNew_ValidComparison_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();

        ThrowIf.GreaterThan<int, int, CustomException>(1, 100, probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void GreaterThan_WithConstructorArgs_ThrowsCustomExceptionWithMessage_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var message = "Left must not be greater than right.";
        object[] args = [message];

        var ex = Assert.Throws<CustomException>(() => ThrowIf.GreaterThan<int, int, CustomException>(5, 1, args, probe.Create()));

        Assert.Equal(message, ex.Message);
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void GreaterThan_WithConstructorArgs_ValidComparison_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();

        ThrowIf.GreaterThan<int, int, CustomException>(1, 5, [], probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    // ------------------------- GreaterThanOrEqual -------------------------

    [Theory]
    [InlineData(2, 1)] // greater
    [InlineData(5, 5)] // equal
    public void GreaterThanOrEqual_WhenLeftGreaterOrEqual_ThrowsGreaterThanOrEqualException_AndReportsFailure(int left, int right)
    {
        var probe = new CallbackProbe();

        Assert.Throws<GreaterThanOrEqualException>(() => ThrowIf.GreaterThanOrEqual(left, right, probe.Create()));

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(-10, 0)]
    public void GreaterThanOrEqual_WhenLeftLess_DoesNotThrow_AndReportsSuccess(int left, int right)
    {
        var probe = new CallbackProbe();

        ThrowIf.GreaterThanOrEqual(left, right, probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void GreaterThanOrEqual_WithCustomExceptionInstance_ThrowsGivenException_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var ex = new CustomException("gte");

        var thrown = Assert.Throws<CustomException>(() => ThrowIf.GreaterThanOrEqual(10, 10, ex, probe.Create()));

        Assert.Same(ex, thrown);
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void GreaterThanOrEqual_WithCustomExceptionInstance_ValidComparison_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();

        ThrowIf.GreaterThanOrEqual(1, 10, new CustomException(), probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void GreaterThanOrEqual_WithGenericExceptionNew_ThrowsCustomException_AndReportsFailure()
    {
        var probe = new CallbackProbe();

        Assert.Throws<CustomException>(() => ThrowIf.GreaterThanOrEqual<int, int, CustomException>(5, 5, probe.Create()));

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void GreaterThanOrEqual_WithGenericExceptionNew_ValidComparison_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();

        ThrowIf.GreaterThanOrEqual<int, int, CustomException>(1, 5, probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void GreaterThanOrEqual_WithConstructorArgs_ThrowsCustomExceptionWithMessage_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var message = "Left must not be >= right.";
        object[] args = [message];

        var ex = Assert.Throws<CustomException>(() => ThrowIf.GreaterThanOrEqual<int, int, CustomException>(5, 5, args, probe.Create()));

        Assert.Equal(message, ex.Message);
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void GreaterThanOrEqual_WithConstructorArgs_ValidComparison_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();

        ThrowIf.GreaterThanOrEqual<int, int, CustomException>(1, 5, [], probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    // ------------------------- ArgumentNull checks -------------------------
    

    [Fact]
    public void GreaterThan_WithExceptionInstanceNull_ThrowsArgumentNullException()
    {
        var probe = new CallbackProbe();

        Assert.Throws<ArgumentNullException>(() => ThrowIf.GreaterThan(10, 1, (Exception)null!, probe.Create()));
        Assert.False(probe.Called);
    }
    

    // ------------------------- Cross-type comparable -------------------------

    [Fact]
    public void GreaterThan_WithCrossComparable_ThrowsWhenGreater_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var left = new MyComparable { Inner = 10 };

        Assert.Throws<GreaterThanException>(() => ThrowIf.GreaterThan(left, 5, probe.Create()));

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void GreaterThanOrEqual_WithCrossComparable_ThrowsWhenGreaterOrEqual_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var left = new MyComparable { Inner = 5 };

        Assert.Throws<GreaterThanOrEqualException>(() => ThrowIf.GreaterThanOrEqual(left, 5, probe.Create()));

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void GreaterThanOrEqual_WithCrossComparable_DoesNotThrowWhenLess_AndReportsSuccess()
    {
        var probe = new CallbackProbe();
        var left = new MyComparable { Inner = 1 };

        ThrowIf.GreaterThanOrEqual(left, 5, probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }
}