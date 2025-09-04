using SGuard.Exceptions;

namespace SGuard.Tests;

public sealed class ThrowIfAnyTests
{
    #region Test Helper Classes

    private class TestItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }

        public TestItem(int id, string? name = null, bool isActive = false)
        {
            Id = id;
            Name = name;
            IsActive = isActive;
        }
    }

    private class CustomException : Exception
    {
        public CustomException() : base("Custom exception") { }
        public CustomException(string message) : base(message) { }
    }

    #endregion

    #region Any<T>(IEnumerable<T> source, Func<T, bool> predicate, SGuardCallback? callback = null)

    [Fact]
    public void Any_ThrowsArgumentNullException_WhenSourceIsNull()
    {
        // Arrange
        IEnumerable<int>? nullSource = null;
        Func<int, bool> predicate = x => x > 0;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => ThrowIf.Any(nullSource!, predicate));

        Assert.Equal("source", exception.ParamName);
    }

    [Fact]
    public void Any_ThrowsArgumentNullException_WhenPredicateIsNull()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, bool>? nullPredicate = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => ThrowIf.Any(source, nullPredicate!));

        Assert.Equal("predicate", exception.ParamName);
    }

    [Fact]
    public void Any_ThrowsAnyException_WhenAnyElementSatisfiesPredicate()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4 };
        Func<int, bool> predicate = x => x % 2 == 0;

        // Act & Assert
        var exception = Assert.Throws<AnyException>(() => ThrowIf.Any(source, predicate));

        Assert.Equal("At least one element satisfied the given predicate.", exception.Message);
    }

    [Fact]
    public void Any_DoesNotThrow_WhenNoElementSatisfiesPredicate()
    {
        // Arrange
        var source = new[] { 1, 3, 5 };
        Func<int, bool> predicate = x => x % 2 == 0;

        // Act & Assert - Should not throw
        ThrowIf.Any(source, predicate);
    }

    [Fact]
    public void Any_DoesNotThrow_WhenSourceIsEmpty()
    {
        // Arrange
        var source = new int[0];
        Func<int, bool> predicate = x => x > 0;

        // Act & Assert - Should not throw
        ThrowIf.Any(source, predicate);
    }

    [Fact]
    public void Any_InvokesCallback_WhenAnyElementSatisfiesPredicate()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, bool> predicate = x => x % 2 == 0;
        GuardOutcome? callbackOutcome = null;
        SGuardCallback callback = outcome => callbackOutcome = outcome;

        // Act & Assert
        var exception = Assert.Throws<AnyException>(() => ThrowIf.Any(source, predicate, callback));

        Assert.Equal(GuardOutcome.Failure, callbackOutcome);
    }

    [Fact]
    public void Any_InvokesCallback_WhenNoElementSatisfiesPredicate()
    {
        // Arrange
        var source = new[] { 1, 3, 5 };
        Func<int, bool> predicate = x => x % 2 == 0;
        GuardOutcome? callbackOutcome = null;
        SGuardCallback callback = outcome => callbackOutcome = outcome;

        // Act
        ThrowIf.Any(source, predicate, callback);

        // Assert
        Assert.Equal(GuardOutcome.Success, callbackOutcome);
    }

    [Fact]
    public void Any_DoesNotThrow_WhenCallbackThrowsException()
    {
        // Arrange
        var source = new[] { 2, 4, 6 };
        Func<int, bool> predicate = x => x % 2 == 0;
        SGuardCallback throwingCallback = _ => throw new InvalidOperationException("Test exception");

        // Act & Assert
        var exception = Assert.Throws<AnyException>(() => ThrowIf.Any(source, predicate, throwingCallback));

        // The method should still throw AnyException despite callback exception
        Assert.NotNull(exception);
    }

    #endregion

    #region Any<T, TException>(IEnumerable<T> source, Func<T, bool> predicate, TException exception, SGuardCallback? callback = null)

    [Fact]
    public void Any_Generic_ThrowsArgumentNullException_WhenExceptionIsNull()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, bool> predicate = x => x > 0;
        CustomException? nullException = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => ThrowIf.Any(source, predicate, nullException!));

        Assert.Equal("exception", exception.ParamName);
    }

    [Fact]
    public void Any_Generic_ThrowsCustomException_WhenAnyElementSatisfiesPredicate()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, bool> predicate = x => x % 2 == 0;
        var customException = new CustomException("Found even number");

        // Act & Assert
        var exception = Assert.Throws<CustomException>(() => ThrowIf.Any(source, predicate, customException));

        Assert.Equal("Found even number", exception.Message);
        Assert.Same(customException, exception);
    }

    [Fact]
    public void Any_Generic_DoesNotThrow_WhenNoElementSatisfiesPredicate()
    {
        // Arrange
        var source = new[] { 1, 3, 5 };
        Func<int, bool> predicate = x => x % 2 == 0;
        var customException = new CustomException();

        // Act & Assert - Should not throw
        ThrowIf.Any(source, predicate, customException);
    }

    [Fact]
    public void Any_Generic_WorksWithComplexObjects()
    {
        // Arrange
        var source = new[]
        {
            new TestItem(1, "Item1", false),
            new TestItem(2, "Item2", true),
            new TestItem(3, "Item3", false)
        };
        Func<TestItem, bool> predicate = item => item.IsActive;
        var customException = new CustomException("Found active item");

        // Act & Assert
        var exception = Assert.Throws<CustomException>(() => ThrowIf.Any(source, predicate, customException));

        Assert.Equal("Found active item", exception.Message);
    }

    #endregion

    #region Performance and Short-Circuit Tests

    [Fact]
    public void Any_ShortCircuits_WhenFirstElementMatches()
    {
        // Arrange
        var evaluationCount = 0;
        var source = new[] { 2, 4, 6, 8 }; // First element matches

        Func<int, bool> predicate = x =>
        {
            evaluationCount++;
            return x % 2 == 0;
        };

        // Act & Assert
        Assert.Throws<AnyException>(() => ThrowIf.Any(source, predicate));

        // Should short-circuit after first match
        Assert.Equal(1, evaluationCount);
    }

    [Fact]
    public void Any_EvaluatesAllElements_WhenNoneMatch()
    {
        // Arrange
        var evaluationCount = 0;
        var source = new[] { 1, 3, 5, 7 };

        Func<int, bool> predicate = x =>
        {
            evaluationCount++;
            return x % 2 == 0;
        };

        // Act
        ThrowIf.Any(source, predicate);

        // Assert
        Assert.Equal(4, evaluationCount); // Should evaluate all elements
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Any_WorksWithSingleElementCollection_Match()
    {
        // Arrange
        var source = new[] { 2 };
        Func<int, bool> predicate = x => x % 2 == 0;

        // Act & Assert
        Assert.Throws<AnyException>(() => ThrowIf.Any(source, predicate));
    }

    [Fact]
    public void Any_WorksWithSingleElementCollection_NoMatch()
    {
        // Arrange
        var source = new[] { 1 };
        Func<int, bool> predicate = x => x % 2 == 0;

        // Act & Assert - Should not throw
        ThrowIf.Any(source, predicate);
    }

    [Fact]
    public void Any_WorksWithNullElements()
    {
        // Arrange
        var source = new string?[] { "test", null, "hello" };
        Func<string?, bool> predicate = s => s == null;

        // Act & Assert
        Assert.Throws<AnyException>(() => ThrowIf.Any(source, predicate));
    }

    [Fact]
    public void Any_WorksWithLargeCollection()
    {
        // Arrange
        var source = Enumerable.Range(1, 1000);
        Func<int, bool> predicate = x => x == 500;

        // Act & Assert
        Assert.Throws<AnyException>(() => ThrowIf.Any(source, predicate));
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Any_BehavesConsistentlyWithLinqAny_WhenMatch()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        Func<int, bool> predicate = x => x > 3;

        // Act
        var linqResult = source.Any(predicate);

        // Assert
        if (linqResult)
        {
            Assert.Throws<AnyException>(() => ThrowIf.Any(source, predicate));
        }
        else
        {
            // Should not throw
            ThrowIf.Any(source, predicate);
        }
    }

    [Fact]
    public void Any_BehavesConsistentlyWithLinqAny_WhenNoMatch()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, bool> predicate = x => x > 5;

        // Act
        var linqResult = source.Any(predicate);

        // Assert
        Assert.False(linqResult);
        // Should not throw
        ThrowIf.Any(source, predicate);
    }

    #endregion
}