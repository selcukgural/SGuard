using SGuard.Exceptions;

namespace SGuard.Tests;

public sealed class ThrowIfNullOrEmptyTests
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

    private class TestObject
    {
        public string? Name { get; set; }
        public List<int>? Numbers { get; set; }
        public object? Any { get; set; }
    }

    // ------------------------- NullOrEmpty<T>(value) -------------------------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NullOrEmpty_Value_NullOrEmpty_ThrowsNullOrEmptyException_AndReportsFailure(string? value)
    {
        var probe = new CallbackProbe();

        Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty(value, probe.Create()));

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("0")]
    public void NullOrEmpty_Value_Valid_DoesNotThrow_AndReportsSuccess(string value)
    {
        var probe = new CallbackProbe();

        ThrowIf.NullOrEmpty(value, probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void NullOrEmpty_Value_EmptyCollection_Throws_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var list = new List<int>();

        Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty(list, probe.Create()));

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void NullOrEmpty_Value_NonEmptyCollection_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();
        var list = new List<int> { 1 };

        ThrowIf.NullOrEmpty(list, probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    // ------------- NullOrEmpty<T, TException>(value, exception instance) -------------

    [Fact]
    public void NullOrEmpty_WithExceptionInstance_Null_ThrowsGivenException_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var ex = new CustomException("null");

        var thrown = Assert.Throws<CustomException>(() => ThrowIf.NullOrEmpty<string, CustomException>(null!, ex, probe.Create()));

        Assert.Same(ex, thrown);
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void NullOrEmpty_WithExceptionInstance_Empty_ThrowsGivenException_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var ex = new CustomException("empty");

        var thrown = Assert.Throws<CustomException>(() => ThrowIf.NullOrEmpty<string, CustomException>(string.Empty, ex, probe.Create()));

        Assert.Same(ex, thrown);
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void NullOrEmpty_WithExceptionInstance_Valid_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();

        ThrowIf.NullOrEmpty("ok", new CustomException(), probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void NullOrEmpty_WithExceptionInstance_ExceptionNull_ThrowsArgumentNullException_AndDoesNotCallCallback()
    {
        var probe = new CallbackProbe();

        Assert.Throws<ArgumentNullException>(() => ThrowIf.NullOrEmpty("x", (CustomException)null!, probe.Create()));

        Assert.False(probe.Called);
    }

    // ------------- NullOrEmpty<T, TException>(value, constructorArgs) -------------

    [Fact]
    public void NullOrEmpty_WithConstructorArgs_Null_ThrowsCustomExceptionWithMessage_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var message = "Value is invalid";
        object[] args = { message };

        var ex = Assert.Throws<CustomException>(() =>
                                                    ThrowIf.NullOrEmpty<string, CustomException>(null!, args, probe.Create()));

        Assert.Equal(message, ex.Message);
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void NullOrEmpty_WithConstructorArgs_Valid_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();

        ThrowIf.NullOrEmpty<string, CustomException>("ok", Array.Empty<object>(), probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    // ------------------------- NullOrEmpty(value, selector) -> default exception -------------------------

    [Fact]
    public void NullOrEmpty_Selector_DefaultException_NullProperty_Throws_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var obj = new TestObject { Name = null };

        Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty(obj, o => o.Name, probe.Create()));

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void NullOrEmpty_Selector_DefaultException_EmptyList_Throws_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var obj = new TestObject { Numbers = new List<int>() };

        Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty(obj, o => o.Numbers, probe.Create()));

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void NullOrEmpty_Selector_DefaultException_Valid_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();
        var obj = new TestObject { Name = "John", Numbers = new List<int> { 1 } };

        ThrowIf.NullOrEmpty(obj, o => o.Name, probe.Create());
        ThrowIf.NullOrEmpty(obj, o => o.Numbers, probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void NullOrEmpty_Selector_DefaultException_SelectorNull_ThrowsArgumentNullException_AndDoesNotCallCallback()
    {
        var probe = new CallbackProbe();
        var obj = new TestObject { Name = "John" };

        Assert.Throws<ArgumentNullException>(() => ThrowIf.NullOrEmpty<TestObject, NullOrEmptyException>(obj, selector: null!, callback: probe.Create()));

        Assert.False(probe.Called);
    }

    // ------------------------- NullOrEmpty(value, selector, exception instance) -------------------------

    [Fact]
    public void NullOrEmpty_Selector_WithExceptionInstance_NullProperty_ThrowsGivenException_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var obj = new TestObject { Any = null };
        var ex = new CustomException("selector");

        var thrown = Assert.Throws<CustomException>(() => ThrowIf.NullOrEmpty(obj, o => o.Any, ex, probe.Create()));

        Assert.Same(ex, thrown);
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void NullOrEmpty_Selector_WithExceptionInstance_Valid_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();
        var obj = new TestObject { Any = new object() };

        ThrowIf.NullOrEmpty(obj, o => o.Any, new CustomException(), probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    [Fact]
    public void NullOrEmpty_Selector_WithExceptionInstance_NullArguments_ThrowArgumentNullException_AndDoNotCallCallback()
    {
        var probe = new CallbackProbe();
        var obj = new TestObject { Name = null };

        Assert.Throws<ArgumentNullException>(() => ThrowIf.NullOrEmpty(obj, null!, new CustomException(), probe.Create()));
        Assert.False(probe.Called);

        Assert.Throws<ArgumentNullException>(() => ThrowIf.NullOrEmpty(obj, o => o.Name, (CustomException)null!, probe.Create()));
        Assert.False(probe.Called);
    }

    // ------------------------- NullOrEmpty(value, selector) with TException new() -------------------------

    [Fact]
    public void NullOrEmpty_Selector_GenericNew_NullProperty_ThrowsCustomException_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var obj = new TestObject { Name = null };

        Assert.Throws<CustomException>(() => ThrowIf.NullOrEmpty<TestObject, CustomException>(obj, o => o.Name, probe.Create()));

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void NullOrEmpty_Selector_GenericNew_Valid_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();
        var obj = new TestObject { Name = "OK" };

        ThrowIf.NullOrEmpty<TestObject, CustomException>(obj, o => o.Name, probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    // ------------- NullOrEmpty(value, selector, constructorArgs) -------------

    [Fact]
    public void NullOrEmpty_Selector_WithConstructorArgs_NullProperty_ThrowsCustomExceptionWithMessage_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var obj = new TestObject { Numbers = null };
        var message = "Member cannot be null or empty.";
        object[] args = { message };

        var ex = Assert.Throws<CustomException>(() =>
                                                    ThrowIf.NullOrEmpty<TestObject, CustomException>(obj, o => o.Numbers, args, probe.Create()));

        Assert.Equal(message, ex.Message);
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void NullOrEmpty_Selector_WithConstructorArgs_Valid_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();
        var obj = new TestObject { Numbers = new List<int> { 42 } };

        ThrowIf.NullOrEmpty<TestObject, CustomException>(obj, o => o.Numbers, Array.Empty<object>(), probe.Create());

        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }

    // ------------------------- Additional collection/object tests -------------------------

    [Fact]
    public void NullOrEmpty_Array_Empty_Throws_AndReportsFailure()
    {
        var probe = new CallbackProbe();
        var arr = Array.Empty<int>();

        Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty(arr, probe.Create()));
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Failure, probe.Outcome);
    }

    [Fact]
    public void NullOrEmpty_Array_NonEmpty_DoesNotThrow_AndReportsSuccess()
    {
        var probe = new CallbackProbe();
        var arr = new[] { 1 };

        ThrowIf.NullOrEmpty(arr, probe.Create());
        Assert.True(probe.Called);
        Assert.Equal(GuardOutcome.Success, probe.Outcome);
    }
}