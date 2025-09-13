using SGuard.Exceptions;

namespace SGuard.Tests;

public sealed class ThrowIfCallbackExceptionSwallowTests
{
    private static SGuardCallback ThrowingCallback()
        => _ => throw new InvalidOperationException("callback boom");

    // Failure path: guard throws its own exception; callback throws too, but is swallowed
    [Fact]
    public void LessThan_Failure_CallbackThrows_SwallowsCallbackException_StillThrowsGuardException()
    {
        var cb = ThrowingCallback();

        // 1 < 2 => guard failure => LessThanException expected
        Assert.Throws<LessThanException>(() => ThrowIf.LessThan(1, 2, cb));
    }

    // Success path: guard passes; callback throws, but should be swallowed (no exception escapes)
    [Fact]
    public void LessThan_Success_CallbackThrows_SwallowsCallbackException_NoThrowFromGuard()
    {
        var cb = ThrowingCallback();

        // 5 >= 2 => guard success => no exception should escape even if callback throws
        var ex = Record.Exception(() => ThrowIf.LessThan(5, 2, cb));
        Assert.Null(ex);
    }

    // Failure path for range guard
    [Fact]
    public void Between_Failure_CallbackThrows_SwallowsCallbackException_StillThrowsGuardException()
    {
        var cb = ThrowingCallback();

        // 5 is between 1..10 inclusive => guard failure => BetweenException expected
        Assert.Throws<BetweenException>(() => ThrowIf.Between(5, 1, 10, cb));
    }

    // Failure path for NullOrEmpty (string)
    [Fact]
    public void NullOrEmpty_String_Failure_CallbackThrows_SwallowsCallbackException_StillThrowsGuardException()
    {
        var cb = ThrowingCallback();

        // null string => guard failure => NullOrEmptyException expected
        Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty((string)null!, cb));
    }

    // Success path for NullOrEmpty (non-empty collection)
    [Fact]
    public void NullOrEmpty_Collection_Success_CallbackThrows_SwallowsCallbackException_NoThrowFromGuard()
    {
        var cb = ThrowingCallback();
        var list = new List<int> { 1 };

        // Non-empty list => guard success => no exception should escape even if callback throws
        var ex = Record.Exception(() => ThrowIf.NullOrEmpty(list, cb));
        Assert.Null(ex);
    }
}