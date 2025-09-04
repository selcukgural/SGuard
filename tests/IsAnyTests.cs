namespace SGuard.Tests;

public sealed class IsAnyTests
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

    private class CustomEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _items;
        
        public CustomEnumerable(IEnumerable<T> items)
        {
            _items = items;
        }
        
        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    #endregion

    #region ArgumentNullException Tests

    [Fact]
    public void Any_ThrowsArgumentNullException_WhenSourceIsNull()
    {
        // Arrange
        IEnumerable<int>? nullSource = null;
        Func<int, bool> predicate = x => x > 0;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => Is.Any(nullSource!, predicate));
        
        Assert.Equal("source", exception.ParamName);
    }

    [Fact]
    public void Any_ThrowsArgumentNullException_WhenPredicateIsNull()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, bool>? nullPredicate = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => Is.Any(source, nullPredicate!));
        
        Assert.Equal("predicate", exception.ParamName);
    }

    [Fact]
    public void Any_ThrowsArgumentNullException_WhenBothSourceAndPredicateAreNull()
    {
        // Arrange
        IEnumerable<int>? nullSource = null;
        Func<int, bool>? nullPredicate = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Is.Any(nullSource!, nullPredicate!));
    }

    #endregion

    #region Basic Functionality Tests

    [Fact]
    public void Any_ReturnsTrue_WhenAtLeastOneElementSatisfiesPredicate()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        Func<int, bool> predicate = x => x > 3;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Any_ReturnsFalse_WhenNoElementSatisfiesPredicate()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, bool> predicate = x => x > 5;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Any_ReturnsFalse_WhenSourceIsEmpty()
    {
        // Arrange
        var source = new int[0];
        Func<int, bool> predicate = x => x > 0;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Any_ReturnsTrue_WhenAllElementsSatisfyPredicate()
    {
        // Arrange
        var source = new[] { 2, 4, 6, 8 };
        Func<int, bool> predicate = x => x % 2 == 0;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Any_ReturnsTrue_WhenOnlyFirstElementSatisfiesPredicate()
    {
        // Arrange
        var source = new[] { 10, 1, 2, 3 };
        Func<int, bool> predicate = x => x > 5;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Any_ReturnsTrue_WhenOnlyLastElementSatisfiesPredicate()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 10 };
        Func<int, bool> predicate = x => x > 5;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Different Collection Types Tests

    [Fact]
    public void Any_WorksWithList()
    {
        // Arrange
        var source = new List<string> { "apple", "banana", "cherry" };
        Func<string, bool> predicate = s => s.StartsWith("b");

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Any_WorksWithArray()
    {
        // Arrange
        var source = new[] { "test", "hello", "world" };
        Func<string, bool> predicate = s => s.Contains("ell");

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Any_WorksWithHashSet()
    {
        // Arrange
        var source = new HashSet<int> { 1, 2, 3, 4, 5 };
        Func<int, bool> predicate = x => x == 3;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Any_WorksWithEnumerableRange()
    {
        // Arrange
        var source = Enumerable.Range(1, 10);
        Func<int, bool> predicate = x => x > 8;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Any_WorksWithCustomEnumerable()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5 };
        var source = new CustomEnumerable<int>(items);
        Func<int, bool> predicate = x => x == 4;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Complex Object Tests

    [Fact]
    public void Any_WorksWithComplexObjects_ReturnsTrue()
    {
        // Arrange
        var source = new[]
        {
            new TestItem(1, "Item1", false),
            new TestItem(2, "Item2", true),
            new TestItem(3, "Item3", false)
        };
        Func<TestItem, bool> predicate = item => item.IsActive;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Any_WorksWithComplexObjects_ReturnsFalse()
    {
        // Arrange
        var source = new[]
        {
            new TestItem(1, "Item1", false),
            new TestItem(2, "Item2", false),
            new TestItem(3, "Item3", false)
        };
        Func<TestItem, bool> predicate = item => item.IsActive;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Any_WorksWithComplexPredicates()
    {
        // Arrange
        var source = new[]
        {
            new TestItem(1, "Apple", true),
            new TestItem(2, "Banana", false),
            new TestItem(3, "Cherry", true)
        };
        
        Func<TestItem, bool> predicate = item => 
            item.Name?.StartsWith("B") == true && item.IsActive;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.False(result); // Banana is not active
    }

    [Fact]
    public void Any_WorksWithNullableProperties()
    {
        // Arrange
        var source = new[]
        {
            new TestItem(1, null, false),
            new TestItem(2, "Test", true),
            new TestItem(3, "", false)
        };
        
        Func<TestItem, bool> predicate = item => !string.IsNullOrEmpty(item.Name);

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.True(result); // "Test" is not null or empty
    }

    #endregion

    #region Callback Tests

    [Fact]
    public void Any_InvokesCallback_WithSuccessOutcome_WhenPredicateMatches()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        Func<int, bool> predicate = x => x > 3;
        GuardOutcome? callbackOutcome = null;
        SGuardCallback callback = outcome => callbackOutcome = outcome;

        // Act
        var result = Is.Any(source, predicate, callback);

        // Assert
        Assert.True(result);
        Assert.Equal(GuardOutcome.Success, callbackOutcome);
    }

    [Fact]
    public void Any_InvokesCallback_WithFailureOutcome_WhenNoPredicateMatches()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, bool> predicate = x => x > 5;
        GuardOutcome? callbackOutcome = null;
        SGuardCallback callback = outcome => callbackOutcome = outcome;

        // Act
        var result = Is.Any(source, predicate, callback);

        // Assert
        Assert.False(result);
        Assert.Equal(GuardOutcome.Failure, callbackOutcome);
    }

    [Fact]
    public void Any_InvokesCallback_WithFailureOutcome_WhenSourceIsEmpty()
    {
        // Arrange
        var source = new int[0];
        Func<int, bool> predicate = x => x > 0;
        GuardOutcome? callbackOutcome = null;
        SGuardCallback callback = outcome => callbackOutcome = outcome;

        // Act
        var result = Is.Any(source, predicate, callback);

        // Assert
        Assert.False(result);
        Assert.Equal(GuardOutcome.Failure, callbackOutcome);
    }

    [Fact]
    public void Any_DoesNotThrow_WhenCallbackIsNull()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, bool> predicate = x => x > 1;

        // Act & Assert - Should not throw
        var result = Is.Any(source, predicate, null);
        Assert.True(result);
    }

    [Fact]
    public void Any_DoesNotThrow_WhenCallbackThrowsException()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        Func<int, bool> predicate = x => x > 3;

        var result = Is.Any(source, predicate);
        Assert.True(result);
    }

    [Fact]
    public void Any_CallbackInvokedOnlyOnce_EvenWithMultipleMatches()
    {
        // Arrange
        var source = new[] { 5, 6, 7, 8, 9 }; // Multiple elements > 3
        Func<int, bool> predicate = x => x > 3;
        var callbackInvocations = 0;
        SGuardCallback callback = _ => callbackInvocations++;

        // Act
        var result = Is.Any(source, predicate, callback);

        // Assert
        Assert.True(result);
        Assert.Equal(1, callbackInvocations);
    }

    #endregion

    #region Edge Cases

    [Theory]
    [InlineData(new int[] { })]
    [InlineData(new int[] { 1 })]
    [InlineData(new int[] { 1, 2 })]
    [InlineData(new int[] { 1, 2, 3, 4, 5 })]
    public void Any_HandlesVaryingCollectionSizes(int[] source)
    {
        // Arrange
        Func<int, bool> predicate = x => x == 3;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        var expected = source.Contains(3);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Any_WorksWithSingleElementCollection_Match()
    {
        // Arrange
        var source = new[] { 42 };
        Func<int, bool> predicate = x => x == 42;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Any_WorksWithSingleElementCollection_NoMatch()
    {
        // Arrange
        var source = new[] { 42 };
        Func<int, bool> predicate = x => x == 100;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Any_WorksWithNullElements()
    {
        // Arrange
        var source = new string?[] { "test", null, "hello" };
        Func<string?, bool> predicate = s => s == null;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Any_WorksWithAllNullElements()
    {
        // Arrange
        var source = new string?[] { null, null, null };
        Func<string?, bool> predicate = s => s != null;

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Performance and Short-Circuit Tests

    [Fact]
    public void Any_ShortCircuits_WhenFirstElementMatches()
    {
        // Arrange
        var evaluationCount = 0;
        var source = new[] { 1, 2, 3, 4, 5 };
        Func<int, bool> predicate = x =>
        {
            evaluationCount++;
            return x == 1; // First element matches
        };

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.True(result);
        Assert.Equal(1, evaluationCount); // Should only evaluate first element
    }

    [Fact]
    public void Any_EvaluatesAllElements_WhenNoneMatch()
    {
        // Arrange
        var evaluationCount = 0;
        var source = new[] { 1, 2, 3, 4, 5 };
        Func<int, bool> predicate = x =>
        {
            evaluationCount++;
            return x == 10; // No element matches
        };

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.False(result);
        Assert.Equal(5, evaluationCount); // Should evaluate all elements
    }

    [Fact]
    public void Any_EvaluatesUntilMatch_WhenMatchInMiddle()
    {
        // Arrange
        var evaluationCount = 0;
        var source = new[] { 1, 2, 3, 4, 5 };
        Func<int, bool> predicate = x =>
        {
            evaluationCount++;
            return x == 3; // Third element matches
        };

        // Act
        var result = Is.Any(source, predicate);

        // Assert
        Assert.True(result);
        Assert.Equal(3, evaluationCount); // Should evaluate first three elements
    }

    #endregion

    #region Integration with LINQ Tests

    [Fact]
    public void Any_BehavesConsistentlyWithLinqAny_WhenMatch()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        Func<int, bool> predicate = x => x > 3;

        // Act
        var ourResult = Is.Any(source, predicate);
        var linqResult = source.Any(predicate);

        // Assert
        Assert.Equal(linqResult, ourResult);
        Assert.True(ourResult);
    }

    [Fact]
    public void Any_BehavesConsistentlyWithLinqAny_WhenNoMatch()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, bool> predicate = x => x > 5;

        // Act
        var ourResult = Is.Any(source, predicate);
        var linqResult = source.Any(predicate);

        // Assert
        Assert.Equal(linqResult, ourResult);
        Assert.False(ourResult);
    }

    [Fact]
    public void Any_BehavesConsistentlyWithLinqAny_EmptySource()
    {
        // Arrange
        var source = new int[0];
        Func<int, bool> predicate = x => x > 0;

        // Act
        var ourResult = Is.Any(source, predicate);
        var linqResult = source.Any(predicate);

        // Assert
        Assert.Equal(linqResult, ourResult);
        Assert.False(ourResult);
    }

    #endregion
}