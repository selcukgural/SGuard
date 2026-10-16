using SGuard.Exceptions;

namespace SGuard.Tests;

// Asserts exact messages, so it can't run while another test turns IncludeValuesInExceptions on.
[Collection(nameof(SGuardOptionsCollection))]
public sealed class NullGuardTests
{
    private sealed class DomainException(string message) : Exception(message);

    private sealed record Customer(string? Email);

    [Fact]
    public void ThrowIfNull_Null_ThrowsNullException_NamingTheExpression()
    {
        var customer = new Customer(null);

        var ex = Assert.Throws<NullException>(() => ThrowIf.Null(customer.Email));

        Assert.Equal("customer.Email", ex.ParamName);
        Assert.Equal("Value 'customer.Email' is null.", ex.Message);
        Assert.IsAssignableFrom<ArgumentNullException>(ex);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("value")]
    public void ThrowIfNull_NonNullString_ReturnsIt(string value)
    {
        Assert.Same(value, ThrowIf.Null(value));
    }

    [Fact]
    public void ThrowIfNull_EmptyAndDefaultValues_Pass()
    {
        // Only null fails, unlike NullOrEmpty.
        Assert.Empty(ThrowIf.Null(Array.Empty<int>()));
        Assert.Equal(0, ThrowIf.Null(0));
        Assert.Equal(Guid.Empty, ThrowIf.Null(Guid.Empty));
        Assert.False(ThrowIf.Null(false));
    }

    [Fact]
    public void ThrowIfNull_NullableStruct_ReturnsTheUnderlyingValue()
    {
        int? count = 0;

        int value = ThrowIf.Null(count);

        Assert.Equal(0, value);
    }

    [Fact]
    public void ThrowIfNull_NullableStructWithoutValue_Throws()
    {
        int? count = null;

        var ex = Assert.Throws<NullException>(() => ThrowIf.Null(count));

        Assert.Equal("count", ex.ParamName);
    }

    [Fact]
    public void ThrowIfNull_Factory_RunsOnlyOnFailure()
    {
        var calls = 0;

        ThrowIf.Null("value", () => { calls++; return new DomainException("missing"); });
        Assert.Equal(0, calls);

        string? missing = null;
        var ex = Assert.Throws<DomainException>(() => ThrowIf.Null(missing, () => { calls++; return new DomainException("missing"); }));

        Assert.Equal(1, calls);
        Assert.Equal("missing", ex.Message);
    }

    [Fact]
    public void ThrowIfNull_NullableStructFactory_ReturnsTheUnderlyingValue()
    {
        int? count = 3;
        int? none = null;

        Assert.Equal(3, ThrowIf.Null(count, () => new DomainException("missing")));
        Assert.Throws<DomainException>(() => ThrowIf.Null(none, () => new DomainException("missing")));
    }

    [Fact]
    public void ThrowIfNull_FactoryReturningNull_ThrowsInvalidOperationException()
    {
        string? missing = null;

        Assert.Throws<InvalidOperationException>(() => ThrowIf.Null<string?, DomainException>(missing, () => null!));
    }

    [Fact]
    public void ThrowIfNull_NullFactory_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ThrowIf.Null<string, DomainException>("value", null!));
    }

    [Fact]
    public void ThrowIfNull_ReportsOutcomeToCallback()
    {
        GuardOutcome? outcome = null;
        string? missing = null;

        ThrowIf.Null("value", o => outcome = o);
        Assert.Equal(GuardOutcome.Success, outcome);

        Assert.Throws<NullException>(() => ThrowIf.Null(missing, o => outcome = o));
        Assert.Equal(GuardOutcome.Failure, outcome);
    }

    [Fact]
    public void ThrowIfNull_ReturnValue_IsNotNullForTheCompiler()
    {
        string? input = "abc";

        // No nullable warning: the guard's return value and the argument are both known to be non-null.
        string guarded = ThrowIf.Null(input);

        Assert.Equal(3, guarded.Length + input.Length - 3);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData("x", false)]
    public void IsNull_ReturnsWhetherTheValueIsNull(string? value, bool expected)
    {
        GuardOutcome? outcome = null;

        Assert.Equal(expected, Is.Null(value, o => outcome = o));
        Assert.Equal(expected ? GuardOutcome.Success : GuardOutcome.Failure, outcome);
    }

    [Fact]
    public void IsNull_NullableStruct()
    {
        int? none = null;
        int? zero = 0;

        Assert.True(Is.Null(none));
        Assert.False(Is.Null(zero));
    }
}
