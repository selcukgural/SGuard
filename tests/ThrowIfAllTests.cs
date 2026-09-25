using SGuard.Exceptions;

namespace SGuard.Tests;

public sealed class ThrowIfAllTests
{
    private class CustomException : Exception
    {
        public CustomException() : base("Custom exception") { }
        public CustomException(string message) : base(message) { }
    }

    #region All<T, TException>(Span<T> source, Func<T, bool> predicate, TException exception, SGuardCallback? callback = null)

    [Fact]
    public void All_Span_ThrowsCustomException_WhenAllElementsSatisfyPredicate()
    {
        // Arrange
        int[] source = { 2, 4, 6 };
        Func<int, bool> predicate = x => x % 2 == 0;
        var customException = new CustomException("All are even");

        // Act & Assert
        var exception = Assert.Throws<CustomException>(() => 
        {
            var span = source.AsSpan();
            ThrowIf.All(span, predicate, customException);
        });

        Assert.Equal("All are even", exception.Message);
        Assert.Same(customException, exception);
    }

    [Fact]
    public void All_Span_DoesNotThrow_WhenAnyElementDoesNotSatisfyPredicate()
    {
        // Arrange
        int[] source = { 2, 3, 6 };
        Func<int, bool> predicate = x => x % 2 == 0;
        var customException = new CustomException();

        // Act & Assert - Should not throw
        var span = source.AsSpan();
        ThrowIf.All(span, predicate, customException);
    }

    [Fact]
    public void All_Span_DoesNotThrow_WhenSourceIsEmpty()
    {
        // Arrange
        var source = Span<int>.Empty;
        Func<int, bool> predicate = x => true;
        var customException = new CustomException();

        // Act & Assert - Should not throw
        ThrowIf.All(source, predicate, customException);
    }

    [Fact]
    public void All_Span_ThrowsArgumentNullException_WhenPredicateIsNull()
    {
        // Arrange
        int[] source = { 1, 2, 3 };
        var customException = new CustomException();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
        {
            var span = source.AsSpan();
            ThrowIf.All(span, (Func<int, bool>)null!, customException);
        });
    }

    [Fact]
    public void All_Span_ThrowsArgumentNullException_WhenExceptionIsNull()
    {
        // Arrange
        int[] source = { 1, 2, 3 };
        Func<int, bool> predicate = x => true;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
        {
            var span = source.AsSpan();
            ThrowIf.All(span, predicate, (CustomException)null!);
        });
    }

    [Fact]
    public void All_Span_InvokesCallbackWithFailure_WhenAllElementsSatisfyPredicate()
    {
        // Arrange
        int[] source = { 1, 1, 1 };
        Func<int, bool> predicate = x => x == 1;
        var customException = new CustomException();
        GuardOutcome? outcome = null;

        // Act & Assert
        Assert.Throws<CustomException>(() => 
        {
            var span = source.AsSpan();
            ThrowIf.All(span, predicate, customException, o => outcome = o);
        });

        Assert.Equal(GuardOutcome.Failure, outcome);
    }

    [Fact]
    public void All_Span_InvokesCallbackWithSuccess_WhenAnyElementDoesNotSatisfyPredicate()
    {
        // Arrange
        int[] source = { 1, 2, 1 };
        Func<int, bool> predicate = x => x == 1;
        var customException = new CustomException();
        GuardOutcome? outcome = null;

        // Act
        var span = source.AsSpan();
        ThrowIf.All(span, predicate, customException, o => outcome = o);

        // Assert
        Assert.Equal(GuardOutcome.Success, outcome);
    }

    [Fact]
    public void All_Span_InvokesCallbackWithSuccess_WhenSourceIsEmpty()
    {
        // Arrange
        var source = Span<int>.Empty;
        Func<int, bool> predicate = x => true;
        var customException = new CustomException();
        GuardOutcome? outcome = null;

        // Act
        ThrowIf.All(source, predicate, customException, o => outcome = o);

        // Assert
        Assert.Equal(GuardOutcome.Success, outcome);
    }

    #endregion

    #region All<T, TException>(IEnumerable<T> source, Func<T, bool> predicate, TException exception, SGuardCallback? callback = null)

    [Fact]
    public void All_Enumerable_ThrowsCustomException_WhenAllElementsSatisfyPredicate()
    {
        // Arrange
        var source = new[] { 2, 4, 6 };
        Func<int, bool> predicate = x => x % 2 == 0;
        var customException = new CustomException("All are even");

        // Act & Assert
        var exception = Assert.Throws<CustomException>(() => ThrowIf.All(source, predicate, customException));
        Assert.Same(customException, exception);
    }

    [Fact]
    public void All_Enumerable_DoesNotThrow_WhenAnyElementDoesNotSatisfyPredicate()
    {
        // Arrange
        var source = new[] { 2, 3, 6 };
        Func<int, bool> predicate = x => x % 2 == 0;
        var customException = new CustomException();

        // Act & Assert - Should not throw
        ThrowIf.All(source, predicate, customException);
    }

    [Fact]
    public void All_Enumerable_DoesNotThrow_WhenSourceIsEmpty()
    {
        // Arrange
        var source = Array.Empty<int>();
        Func<int, bool> predicate = x => true;
        var customException = new CustomException();

        // Act & Assert - Should not throw
        // NOTE: In previous session notes, it was mentioned that Is.All returns false for empty source.
        // Even though LINQ's Enumerable.All returns true, SGuard's Is.All might be handling empty differently
        // OR ThrowIf.All has its own logic.
        ThrowIf.All(source, predicate, customException);
    }

    #endregion

    #region All<T>(IEnumerable<T> source, Func<T, bool> predicate, SGuardCallback? callback = null)

    [Fact]
    public void All_ThrowsAllException_WhenAllElementsSatisfyPredicate()
    {
        // Arrange
        var source = new[] { 1, 1, 1 };
        Func<int, bool> predicate = x => x == 1;

        // Act & Assert
        var exception = Assert.Throws<AllException>(() => ThrowIf.All(source, predicate));
        Assert.Equal("All elements satisfied the given predicate.", exception.Message);
    }

    #endregion
}
