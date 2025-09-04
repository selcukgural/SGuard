namespace SGuard.Tests;

public class IsAllTests
{
    [Fact]
    public void All_ReturnsTrue_WhenAllElementsSatisfyPredicate()
    {
        var source = new[] { 2, 4, 6 };
        bool result = Is.All(source, x => x % 2 == 0);
        Assert.True(result);
    }

    [Fact]
    public void All_ReturnsFalse_WhenAnyElementDoesNotSatisfyPredicate()
    {
        var source = new[] { 2, 3, 6 };
        bool result = Is.All(source, x => x % 2 == 0);
        Assert.False(result);
    }

    [Fact]
    public void All_ThrowsArgumentNullException_WhenSourceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => Is.All<int>(null!, x => true));
    }

    [Fact]
    public void All_ThrowsArgumentNullException_WhenPredicateIsNull()
    {
        var source = new[] { 1, 2, 3 };
        Assert.Throws<ArgumentNullException>(() => Is.All(source, null!));
    }

    [Fact]
    public void All_InvokesCallbackWithSuccess_WhenAllElementsSatisfyPredicate()
    {
        var source = new[] { 1, 1, 1 };
        GuardOutcome? outcome = null;
        Is.All(source, x => x == 1, o => outcome = o);
        Assert.Equal(GuardOutcome.Success, outcome);
    }

    [Fact]
    public void All_InvokesCallbackWithFailure_WhenAnyElementDoesNotSatisfyPredicate()
    {
        var source = new[] { 1, 2, 1 };
        GuardOutcome? outcome = null;
        Is.All(source, x => x == 1, o => outcome = o);
        Assert.Equal(GuardOutcome.Failure, outcome);
    }

    [Fact]
    public void All_ReturnsTrue_ForEmptySource()
    {
        var source = Array.Empty<int>();
        bool result = Is.All(source, x => false);
        Assert.True(result);
    }

    [Fact]
    public void All_DoesNotThrow_WhenCallbackIsNull()
    {
        var source = new[] { 1, 2, 3 };
        var exception = Record.Exception(() => Is.All(source, x => true, null));
        Assert.Null(exception);
    }
}