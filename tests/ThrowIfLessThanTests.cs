using SGuard.Exceptions;

namespace SGuard.Tests;

public sealed class ThrowIfLessThanTests
{
    // Testlerde kullanılacak özel exception türü
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

    // ------------------------- LessThan -------------------------

    [Theory]
    [InlineData(1, 2)]
    [InlineData(-5, 0)]
    [InlineData(-10, -9)]
    public void LessThan_WhenLeftLessThanRight_ThrowsLessThanException_AndReportsFailure(int left, int right)
    {
        var probe = new CallbackProbe();
        Assert.Throws<LessThanException>(() => ThrowIf.LessThan(left, right, probe.Create()));
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Theory]
    [InlineData(2, 1)]
    [InlineData(10, 10)]
    public void LessThan_WhenLeftNotLessThanRight_DoesNotThrow_AndReportsSuccess(int left, int right)
    {
        var probe = new CallbackProbe();
        ThrowIf.LessThan(left, right, probe.Create());
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void LessThan_WithCustomExceptionInstance_ThrowsGivenException_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var ex = new CustomException("lt");
        var thrown = Assert.Throws<CustomException>(() => ThrowIf.LessThan(1, 2, ex, probe.Create()));
        Assert.Same(ex, thrown);
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void LessThan_WithCustomExceptionInstance_ValidComparison_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();
        ThrowIf.LessThan(5, 2, new CustomException(), probe.Create());
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    // --------------------- LessThanOrEqual (built-in) ---------------------

    [Theory]
    [InlineData(1, 2)] // less
    [InlineData(5, 5)] // equal
    public void LessThanOrEqual_WhenLeftLessOrEqual_ThrowsLessThanOrEqualException_AndReportsFailure(int left, int right)
    {
        var probe = new CallbackProbe();
        Assert.Throws<LessThanOrEqualException>(() => ThrowIf.LessThanOrEqual(left, right, probe.Create()));
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Theory]
    [InlineData(6, 5)]
    [InlineData(100, 0)]
    public void LessThanOrEqual_WhenLeftGreater_DoesNotThrow_AndReportsSuccess(int left, int right)
    {
        var probe = new CallbackProbe();
        ThrowIf.LessThanOrEqual(left, right, probe.Create());
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void LessThanOrEqual_WithCustomExceptionInstance_ThrowsGivenException_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var ex = new CustomException("lte");
        var thrown = Assert.Throws<CustomException>(() => ThrowIf.LessThanOrEqual(10, 10, ex, probe.Create()));
        Assert.Same(ex, thrown);
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void LessThanOrEqual_WithCustomExceptionInstance_ValidComparison_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();
        ThrowIf.LessThanOrEqual(20, 10, new CustomException(), probe.Create());
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void LessThanOrEqual_GenericExceptionNew_ThrowsCustomException_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        Assert.Throws<CustomException>(() => ThrowIf.LessThanOrEqual<int, int, CustomException>(1, 2, probe.Create()));
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void LessThanOrEqual_GenericExceptionNew_ValidComparison_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();
        ThrowIf.LessThanOrEqual<int, int, CustomException>(5, 2, probe.Create());
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void LessThanOrEqual_WithConstructorArgs_ThrowsCustomExceptionWithMessage_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var message = "Values must not be less or equal.";
        object[] args = [message];

        var ex = Assert.Throws<CustomException>(() => ThrowIf.LessThanOrEqual<int, int, CustomException>(10, 10, args, probe.Create()));
        Assert.Equal(message, ex.Message);
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void LessThanOrEqual_WithConstructorArgs_ValidComparison_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();
        ThrowIf.LessThanOrEqual<int, int, CustomException>(20, 10, [], probe.Create());
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }
    

    // --------------------- Cross-type IComparable check ---------------------

    private struct MyComparable : IComparable<int>
    {
        public int Inner;
        public int CompareTo(int other) => Inner.CompareTo(other);
    }

    [Fact]
    public void LessThan_WithCrossComparable_Works()
    {
        var probe = new CallbackProbe();
        var left = new MyComparable { Inner = 1 };
        Assert.Throws<LessThanException>(() => ThrowIf.LessThan(left, 5, probe.Create()));
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void LessThanOrEqual_WithCrossComparable_Works()
    {
        var probe = new CallbackProbe();
        var left = new MyComparable { Inner = 5 };
        ThrowIf.LessThanOrEqual(left, 1, probe.Create()); // 5 > 1 => success
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }
}