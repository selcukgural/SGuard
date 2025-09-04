using SGuard.Exceptions;

namespace SGuard.Tests;

public sealed class ThrowIfBetweenTests
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

    // ------------------------- Numeric Between (value,min,max) -------------------------

    [Theory]
    [InlineData(5, 1, 10)]  // strictly inside
    [InlineData(1, 1, 10)]  // at lower bound (inclusive)
    [InlineData(10, 1, 10)] // at upper bound (inclusive)
    public void Between_ValueInsideOrOnBounds_ThrowsBetweenException_AndReportsFailure(int value, int min, int max)
    {
        var probe = new CallbackProbe();

        Assert.Throws<BetweenException>(() => ThrowIf.Between(value, min, max, probe.Create()));

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Theory]
    [InlineData(0, 1, 10)]
    [InlineData(11, 1, 10)]
    public void Between_ValueOutside_DoesNotThrow_AndReportsSuccess(int value, int min, int max)
    {
        var probe = new CallbackProbe();

        ThrowIf.Between(value, min, max, probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    // ------------------------- Between with exception instance -------------------------

    [Fact]
    public void Between_WithExceptionInstance_WhenInside_ThrowsGivenException_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var ex = new CustomException("between");

        var thrown = Assert.Throws<CustomException>(
            () => ThrowIf.Between<int, int, int, CustomException>(5, 1, 10, ex, probe.Create()));

        Assert.Same(ex, thrown);
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void Between_WithExceptionInstance_WhenOutside_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();

        ThrowIf.Between<int, int, int, CustomException>(0, 1, 10, new CustomException(), probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void Between_WithExceptionInstance_NullException_ThrowsArgumentNullException_AndDoesNotCallCallback()
    {
        var probe = new CallbackProbe();

        Assert.Throws<ArgumentNullException>(
            () => ThrowIf.Between(5, 1, 10, (CustomException)null!, probe.Create()));

        Assert.False(probe.Called);
    }

    // ------------------------- Between with TException new() -------------------------

    [Fact]
    public void Between_GenericNew_WhenInside_ThrowsCustomException_AndReportsFailure()
    {
        var probe = new CallbackProbe();

        Assert.Throws<CustomException>(
            () => ThrowIf.Between<int, int, int, CustomException>(5, 1, 10, probe.Create()));

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void Between_GenericNew_WhenOutside_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();

        ThrowIf.Between<int, int, int, CustomException>(0, 1, 10, probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    // ------------------------- Between with constructor args -------------------------

    [Fact]
    public void Between_WithConstructorArgs_WhenInside_ThrowsCustomExceptionWithMessage_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var message = "Value should not be within range.";
        object[] args = [message];

        var ex = Assert.Throws<CustomException>(
            () => ThrowIf.Between<int, int, int, CustomException>(5, 1, 10, args, probe.Create()));

        Assert.Equal(message, ex.Message);
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void Between_WithConstructorArgs_WhenOutside_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();

        ThrowIf.Between<int, int, int, CustomException>(-1, 1, 10, [], probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    // ------------------------- String Between with StringComparison -------------------------

    [Theory]
    [InlineData("b", "a", "c", StringComparison.Ordinal)] // inside
    [InlineData("a", "a", "z", StringComparison.Ordinal)] // at lower bound
    [InlineData("z", "a", "z", StringComparison.Ordinal)] // at upper bound
    public void Between_String_WhenBetweenInclusive_ThrowsBetweenException_AndReportsFailure(
        string value, string min, string max, StringComparison cmp)
    {
        var probe = new CallbackProbe();

        Assert.Throws<BetweenException>(() => ThrowIf.Between(value, min, max, cmp, probe.Create()));

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Theory]
    [InlineData("0", "a", "z", StringComparison.Ordinal)] // below
    [InlineData("{", "a", "z", StringComparison.Ordinal)] // above (ASCII after 'z' is '{')
    public void Between_String_WhenOutside_DoesNotThrow_AndReportsSuccess(
        string value, string min, string max, StringComparison cmp)
    {
        var probe = new CallbackProbe();

        ThrowIf.Between(value, min, max, cmp, probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void Between_String_NullArguments_ThrowArgumentNullException_AndDoNotCallCallback()
    {
        var probe = new CallbackProbe();

        Assert.Throws<ArgumentNullException>(() => ThrowIf.Between(null!, "a", "z", StringComparison.Ordinal, probe.Create()));
        Assert.False(probe.Called);

        Assert.Throws<ArgumentNullException>(() => ThrowIf.Between("b", null!, "z", StringComparison.Ordinal, probe.Create()));
        Assert.False(probe.Called);

        Assert.Throws<ArgumentNullException>(() => ThrowIf.Between("b", "a", null!, StringComparison.Ordinal, probe.Create()));
        Assert.False(probe.Called);
    }

    // ------------------------- String Between with provided exception -------------------------

    [Fact]
    public void Between_String_WithExceptionInstance_WhenBetween_ThrowsGivenException_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var ex = new CustomException("str-between");

        var thrown = Assert.Throws<CustomException>(
            () => ThrowIf.Between("m", "a", "z", StringComparison.Ordinal, ex, probe.Create()));

        Assert.Same(ex, thrown);
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void Between_String_WithExceptionInstance_WhenOutside_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();

        ThrowIf.Between("0", "a", "z", StringComparison.Ordinal, new CustomException(), probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void Between_String_WithExceptionInstance_NullException_ThrowsArgumentNullException_AndDoesNotCallCallback()
    {
        var probe = new CallbackProbe();

        Assert.Throws<ArgumentNullException>(
            () => ThrowIf.Between("m", "a", "z", StringComparison.Ordinal, (CustomException)null!, probe.Create()));

        Assert.False(probe.Called);
    }

    // ------------------------- String Between with TException new() -------------------------

    [Fact]
    public void Between_String_GenericNew_WhenBetween_ThrowsCustomException_AndReportsFailure()
    {
        var probe = new CallbackProbe();

        Assert.Throws<CustomException>(() =>
                                           ThrowIf.Between<CustomException>("m", "a", "z", StringComparison.Ordinal, probe.Create()));

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void Between_String_GenericNew_WhenOutside_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();

        ThrowIf.Between<CustomException>("0", "a", "z", StringComparison.Ordinal, probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    // ------------------------- String Between with constructor args -------------------------

    [Fact]
    public void Between_String_WithConstructorArgs_WhenBetween_ThrowsCustomExceptionWithMessage_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var message = "String must not be between given bounds.";
        object[] args = [message];

        var ex = Assert.Throws<CustomException>(
            () => ThrowIf.Between<CustomException>("m", "a", "z", StringComparison.Ordinal, args, probe.Create()));

        Assert.Equal(message, ex.Message);
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void Between_String_WithConstructorArgs_WhenOutside_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();

        ThrowIf.Between<CustomException>("0", "a", "z", StringComparison.Ordinal, [], probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }
}