using SGuard.Exceptions;

namespace SGuard.Tests;

// Asserts exact messages, so it can't run while another test turns IncludeValuesInExceptions on.
[Collection(nameof(SGuardOptionsCollection))]
public sealed class EqualityGuardTests
{
    private sealed class DomainException(string message) : Exception(message);

    private sealed record Money(decimal Amount, string Currency);

    [Fact]
    public void ThrowIfEqual_EqualValues_Throws()
    {
        var sourceAccount = Guid.Parse("8f4f0a51-9e0e-4c41-9f07-56b1d0e7ac22");
        var targetAccount = sourceAccount;

        var ex = Assert.Throws<EqualException>(() => ThrowIf.Equal(targetAccount, sourceAccount));

        Assert.Equal("targetAccount", ex.ParamName);
        Assert.Equal("Left value is equal to right value. Actual: left=targetAccount, right=sourceAccount.", ex.Message);
        Assert.IsAssignableFrom<ArgumentOutOfRangeException>(ex);
    }

    [Fact]
    public void ThrowIfEqual_DifferentValues_ReturnsTheFirst()
    {
        Assert.Equal(1, ThrowIf.Equal(1, 2));
        Assert.Equal(new Money(1, "EUR"), ThrowIf.Equal(new Money(1, "EUR"), new Money(1, "USD")));
    }

    [Fact]
    public void ThrowIfEqual_UsesValueEquality()
    {
        Assert.Throws<EqualException>(() => ThrowIf.Equal(new Money(1, "EUR"), new Money(1, "EUR")));
    }

    [Fact]
    public void ThrowIfEqual_AllowsNull()
    {
        string? a = null;
        string? b = null;

        Assert.Throws<EqualException>(() => ThrowIf.Equal(a, b));
        Assert.Equal("x", ThrowIf.Equal("x", b));
        Assert.Null(ThrowIf.Equal(a, "x"));
    }

    [Fact]
    public void ThrowIfEqual_String_UsesTheComparison()
    {
        Assert.Equal("Admin", ThrowIf.Equal("Admin", "admin", StringComparison.Ordinal));
        Assert.Throws<EqualException>(() => ThrowIf.Equal("Admin", "admin", StringComparison.OrdinalIgnoreCase));
        Assert.True(Is.Equal("Admin", "admin", StringComparison.OrdinalIgnoreCase));
        Assert.False(Is.Equal("Admin", "admin", StringComparison.Ordinal));
        Assert.True(Is.Equal(null, null, StringComparison.Ordinal));
    }

    [Fact]
    public void ThrowIfNotEqual_DifferentValues_Throws()
    {
        const string expected = "EUR";
        var currency = "USD";

        var ex = Assert.Throws<NotEqualException>(() => ThrowIf.NotEqual(currency, expected));

        Assert.Equal("currency", ex.ParamName);
        Assert.Equal("Left value is not equal to right value. Actual: left=currency, right=expected.", ex.Message);
        Assert.IsAssignableFrom<ArgumentOutOfRangeException>(ex);
    }

    [Fact]
    public void ThrowIfNotEqual_EqualValues_ReturnsTheFirst()
    {
        Assert.Equal(3, ThrowIf.NotEqual(3, 3));
        Assert.Equal("eur", ThrowIf.NotEqual("eur", "EUR", StringComparison.OrdinalIgnoreCase));
        Assert.Throws<NotEqualException>(() => ThrowIf.NotEqual("eur", "EUR", StringComparison.Ordinal));
    }

    [Fact]
    public void IsEqualAndNotEqual_AreOpposites()
    {
        Assert.True(Is.Equal(1, 1));
        Assert.False(Is.NotEqual(1, 1));
        Assert.False(Is.Equal(1, 2));
        Assert.True(Is.NotEqual(1, 2));
        Assert.True(Is.NotEqual("a", "A", StringComparison.Ordinal));
        Assert.False(Is.NotEqual("a", "A", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void EqualityGuards_Factory_RunOnlyOnFailure()
    {
        var calls = 0;
        DomainException Create() { calls++; return new DomainException("x"); }

        ThrowIf.Equal(1, 2, Create);
        Assert.Throws<DomainException>(() => ThrowIf.Equal(1, 1, Create));
        ThrowIf.NotEqual(1, 1, Create);
        Assert.Throws<DomainException>(() => ThrowIf.NotEqual(1, 2, Create));
        ThrowIf.Equal("a", "b", StringComparison.Ordinal, Create);
        Assert.Throws<DomainException>(() => ThrowIf.Equal("a", "A", StringComparison.OrdinalIgnoreCase, Create));
        ThrowIf.NotEqual("a", "A", StringComparison.OrdinalIgnoreCase, Create);
        Assert.Throws<DomainException>(() => ThrowIf.NotEqual("a", "b", StringComparison.Ordinal, Create));

        Assert.Equal(4, calls);
    }

    [Fact]
    public void EqualityGuards_ReportOutcomeToCallback()
    {
        var outcomes = new List<GuardOutcome>();

        ThrowIf.Equal(1, 2, outcomes.Add);
        Assert.Throws<EqualException>(() => ThrowIf.Equal(1, 1, outcomes.Add));
        Is.Equal(1, 1, outcomes.Add);
        Is.NotEqual(1, 1, outcomes.Add);

        Assert.Equal([GuardOutcome.Success, GuardOutcome.Failure, GuardOutcome.Success, GuardOutcome.Failure], outcomes);
    }
}
