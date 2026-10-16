using SGuard.Exceptions;

namespace SGuard.Tests;

// Asserts exact messages, so it can't run while another test turns IncludeValuesInExceptions on.
[Collection(nameof(SGuardOptionsCollection))]
public sealed class NotDefinedTests
{
    private sealed class DomainException(string message) : Exception(message);

    public enum OrderStatus
    {
        Pending = 1,
        Paid = 2,
        Shipped = 3
    }

    [Flags]
    private enum Permissions
    {
        None = 0,
        Read = 1,
        Write = 2,
        ReadWrite = Read | Write,
        Delete = 4
    }

    [Fact]
    public void ThrowIfNotDefined_UndefinedValue_Throws()
    {
        var status = (OrderStatus)42;

        var ex = Assert.Throws<NotDefinedException>(() => ThrowIf.NotDefined(status));

        Assert.Equal("status", ex.ParamName);
        Assert.Equal("Value 'status' is not a defined value of 'OrderStatus'.", ex.Message);
        Assert.IsAssignableFrom<ArgumentOutOfRangeException>(ex);
    }

    [Fact]
    public void ThrowIfNotDefined_DefaultValueThatIsNotAMember_Throws()
    {
        Assert.Throws<NotDefinedException>(() => ThrowIf.NotDefined(default(OrderStatus)));
    }

    [Theory]
    [InlineData(OrderStatus.Pending)]
    [InlineData(OrderStatus.Shipped)]
    public void ThrowIfNotDefined_DefinedValue_ReturnsIt(OrderStatus status)
    {
        Assert.Equal(status, ThrowIf.NotDefined(status));
    }

    [Fact]
    public void ThrowIfNotDefined_Flags_OnlyDeclaredValuesPass()
    {
        Assert.Equal(Permissions.ReadWrite, ThrowIf.NotDefined(Permissions.Read | Permissions.Write));
        Assert.Throws<NotDefinedException>(() => ThrowIf.NotDefined(Permissions.Read | Permissions.Delete));
    }

    [Fact]
    public void IsNotDefined_ReturnsTheResult()
    {
        GuardOutcome? outcome = null;

        Assert.True(Is.NotDefined((OrderStatus)42, o => outcome = o));
        Assert.Equal(GuardOutcome.Success, outcome);
        Assert.False(Is.NotDefined(OrderStatus.Paid, o => outcome = o));
        Assert.Equal(GuardOutcome.Failure, outcome);
    }

    [Fact]
    public void ThrowIfNotDefined_Factory()
    {
        Assert.Equal(OrderStatus.Paid, ThrowIf.NotDefined(OrderStatus.Paid, () => new DomainException("status")));
        Assert.Throws<DomainException>(() => ThrowIf.NotDefined((OrderStatus)42, () => new DomainException("status")));
    }
}
