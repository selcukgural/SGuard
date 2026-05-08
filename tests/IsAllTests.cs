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
        Assert.Throws<ArgumentNullException>(() => Is.All<int>((IEnumerable<int>)null!, x => true));
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
    public void All_ReturnsFalse_ForEmptySource()
    {
        var source = Array.Empty<int>();
        bool result = Is.All(source, x => false);
        Assert.False(result);
    }

    [Fact]
    public void All_Span_ReturnsTrue_WhenAllElementsSatisfyPredicate()
    {
        int[] source = { 2, 4, 6 };
        bool result = Is.All(source.AsSpan(), x => x % 2 == 0);
        Assert.True(result);
    }

    [Fact]
    public void All_Span_ReturnsFalse_WhenAnyElementDoesNotSatisfyPredicate()
    {
        int[] source = { 2, 3, 6 };
        bool result = Is.All(source.AsSpan(), x => x % 2 == 0);
        Assert.False(result);
    }

    [Fact]
    public void All_Span_ReturnsFalse_WhenSourceIsEmpty()
    {
        var source = Span<int>.Empty;
        bool result = Is.All(source, x => true);
        Assert.False(result);
    }

    [Fact]
    public void All_Span_ThrowsArgumentNullException_WhenPredicateIsNull()
    {
        int[] source = { 1, 2, 3 };
        Assert.Throws<ArgumentNullException>(() =>
        {
            var span = source.AsSpan();
            return Is.All(span, null!);
        });
    }

    [Fact]
    public void All_Span_InvokesCallbackWithSuccess_WhenAllElementsSatisfyPredicate()
    {
        int[] source = { 1, 1, 1 };
        GuardOutcome? outcome = null;
        Is.All(source.AsSpan(), x => x == 1, o => outcome = o);
        Assert.Equal(GuardOutcome.Success, outcome);
    }

    [Fact]
    public void All_Span_InvokesCallbackWithFailure_WhenAnyElementDoesNotSatisfyPredicate()
    {
        int[] source = { 1, 2, 1 };
        GuardOutcome? outcome = null;
        Is.All(source.AsSpan(), x => x == 1, o => outcome = o);
        Assert.Equal(GuardOutcome.Failure, outcome);
    }

    [Fact]
    public void All_Span_InvokesCallbackWithFailure_WhenSourceIsEmpty()
    {
        var source = Span<int>.Empty;
        GuardOutcome? outcome = null;
        Is.All(source, x => true, o => outcome = o);
        Assert.Equal(GuardOutcome.Failure, outcome);
    }

    [Fact]
    public void All_DoesNotThrow_WhenCallbackIsNull()
    {
        var source = new[] { 1, 2, 3 };
        var exception = Record.Exception(() => Is.All(source, x => true, null));
        Assert.Null(exception);
    }
}