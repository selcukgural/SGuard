namespace SGuard.Tests;

public sealed class IsLessThanTests
{
    private sealed class CustomComparable : IComparable<int>
    {
        public int Value { get; }
        public CustomComparable(int value) => Value = value;
        public int CompareTo(int other) => Value.CompareTo(other);
    }

    #region LessThan<TLeft, TRight> Tests

    [Fact]
    public void LessThan_Generic_ReturnsTrue_WhenLeftIsLessThanRight()
    {
        // Temel durumlar
        Assert.True(Is.LessThan(3, 5));
        Assert.True(Is.LessThan(-10, -5));
        Assert.True(Is.LessThan(0, 1));
        
        // Sınır değerler
        Assert.True(Is.LessThan(int.MinValue, int.MinValue + 1));
        Assert.True(Is.LessThan(int.MaxValue - 1, int.MaxValue));
    }

    [Fact]
    public void LessThan_Generic_ReturnsFalse_WhenLeftIsEqualOrGreaterThanRight()
    {
        // Eşit değerler
        Assert.False(Is.LessThan(5, 5));
        Assert.False(Is.LessThan(0, 0));
        Assert.False(Is.LessThan(-1, -1));
        
        // Büyük değerler
        Assert.False(Is.LessThan(10, 5));
        Assert.False(Is.LessThan(1, 0));
        Assert.False(Is.LessThan(-5, -10));
    }
    
    [Fact]
    public void LessThan_Generic_InvokesCallback_WithCorrectResult()
    {
        bool? callbackResult = null;
        SGuardCallback callback = result => callbackResult = result == GuardOutcome.Success;

        // True durumu
        var result1 = Is.LessThan(3, 7, callback);
        Assert.True(result1);
        Assert.True(callbackResult);

        // False durumu
        callbackResult = null;
        var result2 = Is.LessThan(7, 3, callback);
        Assert.False(result2);
        Assert.False(callbackResult);

        // Equal durumu
        callbackResult = null;
        var result3 = Is.LessThan(5, 5, callback);
        Assert.False(result3);
        Assert.False(callbackResult);
    }

    [Fact]
    public void LessThan_Generic_DoesNotThrow_WhenCallbackThrowsException()
    {
        SGuardCallback throwingCallback = _ => throw new InvalidOperationException("Test exception");

        // Method should not throw even if callback does
        var result1 = Is.LessThan(1, 5, throwingCallback);
        Assert.True(result1);

        var result2 = Is.LessThan(5, 1, throwingCallback);
        Assert.False(result2);
    }

    [Fact]
    public void LessThan_Generic_WorksWithCrossTypeComparable()
    {
        var customValue = new CustomComparable(10);
        
        Assert.True(Is.LessThan(customValue, 15));
        Assert.False(Is.LessThan(customValue, 10));
        Assert.False(Is.LessThan(customValue, 5));
    }

    [Fact]
    public void LessThan_Generic_WorksWithDifferentNumericTypes()
    {
        // DateTime comparison
        var date1 = new DateTime(2023, 1, 1);
        var date2 = new DateTime(2023, 1, 2);
        
        Assert.True(Is.LessThan(date1, date2));
        Assert.False(Is.LessThan(date2, date1));
        
        // Double comparison
        Assert.True(Is.LessThan(1.5, 2.5));
        Assert.False(Is.LessThan(2.5, 1.5));
    }

    #endregion

    #region LessThan String Tests

    [Fact]
    public void LessThan_String_ReturnsTrue_WhenLeftIsLessThanRight()
    {
        // Ordinal comparison
        Assert.True(Is.LessThan("a", "b", StringComparison.Ordinal));
        Assert.True(Is.LessThan("apple", "banana", StringComparison.Ordinal));
        
        // OrdinalIgnoreCase comparison
        Assert.True(Is.LessThan("A", "b", StringComparison.OrdinalIgnoreCase));
        Assert.True(Is.LessThan("apple", "BANANA", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void LessThan_String_ReturnsFalse_WhenLeftIsEqualOrGreaterThanRight()
    {
        // Equal strings
        Assert.False(Is.LessThan("hello", "hello", StringComparison.Ordinal));
        Assert.False(Is.LessThan("Hello", "hello", StringComparison.OrdinalIgnoreCase));
        
        // Greater strings
        Assert.False(Is.LessThan("z", "a", StringComparison.Ordinal));
        Assert.False(Is.LessThan("banana", "apple", StringComparison.Ordinal));
    }

    [Fact]
    public void LessThan_String_ThrowsArgumentNullException_WhenLeftValueIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => Is.LessThan(null!, "test", StringComparison.Ordinal));
        
        Assert.Equal("lValue", exception.ParamName);
    }

    [Fact]
    public void LessThan_String_ThrowsArgumentNullException_WhenRightValueIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => Is.LessThan("test", null!, StringComparison.Ordinal));
        
        Assert.Equal("rValue", exception.ParamName);
    }

    [Fact]
    public void LessThan_String_InvokesCallback_WithCorrectResult()
    {
        bool? callbackResult = null;
        SGuardCallback callback = result => callbackResult = result == GuardOutcome.Success;

        // True case
        var result1 = Is.LessThan("a", "z", StringComparison.Ordinal, callback);
        Assert.True(result1);
        Assert.True(callbackResult);

        // False case
        callbackResult = null;
        var result2 = Is.LessThan("z", "a", StringComparison.Ordinal, callback);
        Assert.False(result2);
        Assert.False(callbackResult);
    }

    [Fact]
    public void LessThan_String_DoesNotThrow_WhenCallbackThrowsException()
    {
        SGuardCallback throwingCallback = _ => throw new InvalidOperationException("Test exception");

        var result1 = Is.LessThan("a", "z", StringComparison.Ordinal, throwingCallback);
        Assert.True(result1);

        var result2 = Is.LessThan("z", "a", StringComparison.Ordinal, throwingCallback);
        Assert.False(result2);
    }

    [Theory]
    [InlineData(StringComparison.Ordinal)]
    [InlineData(StringComparison.OrdinalIgnoreCase)]
    [InlineData(StringComparison.CurrentCulture)]
    [InlineData(StringComparison.CurrentCultureIgnoreCase)]
    [InlineData(StringComparison.InvariantCulture)]
    [InlineData(StringComparison.InvariantCultureIgnoreCase)]
    public void LessThan_String_WorksWithAllStringComparisonTypes(StringComparison comparison)
    {
        // This test ensures the method works with all StringComparison values
        var result = Is.LessThan("a", "b", comparison);
        // We don't assert the specific result because it depends on the comparison type
        // We just ensure no exception is thrown
        Assert.IsType<bool>(result);
    }

    #endregion

    #region LessThanOrEqual<TLeft, TRight> Tests

    [Fact]
    public void LessThanOrEqual_Generic_ReturnsTrue_WhenLeftIsLessThanOrEqualToRight()
    {
        // Less than
        Assert.True(Is.LessThanOrEqual(3, 5));
        Assert.True(Is.LessThanOrEqual(-10, -5));
        
        // Equal
        Assert.True(Is.LessThanOrEqual(5, 5));
        Assert.True(Is.LessThanOrEqual(0, 0));
        Assert.True(Is.LessThanOrEqual(-1, -1));
    }

    [Fact]
    public void LessThanOrEqual_Generic_ReturnsFalse_WhenLeftIsGreaterThanRight()
    {
        Assert.False(Is.LessThanOrEqual(10, 5));
        Assert.False(Is.LessThanOrEqual(1, 0));
        Assert.False(Is.LessThanOrEqual(-5, -10));
    }

    [Fact]
    public void LessThanOrEqual_Generic_InvokesCallback_WithCorrectResult()
    {
        bool? callbackResult = null;
        SGuardCallback callback = result => callbackResult = result == GuardOutcome.Success;

        // True case (less than)
        var result1 = Is.LessThanOrEqual(3, 7, callback);
        Assert.True(result1);
        Assert.True(callbackResult);

        // True case (equal)
        callbackResult = null;
        var result2 = Is.LessThanOrEqual(5, 5, callback);
        Assert.True(result2);
        Assert.True(callbackResult);

        // False case
        callbackResult = null;
        var result3 = Is.LessThanOrEqual(7, 3, callback);
        Assert.False(result3);
        Assert.False(callbackResult);
    }

    [Fact]
    public void LessThanOrEqual_Generic_DoesNotThrow_WhenCallbackThrowsException()
    {
        SGuardCallback throwingCallback = _ => throw new InvalidOperationException("Test exception");

        var result1 = Is.LessThanOrEqual(1, 5, throwingCallback);
        Assert.True(result1);

        var result2 = Is.LessThanOrEqual(5, 1, throwingCallback);
        Assert.False(result2);
    }

    [Fact]
    public void LessThanOrEqual_Generic_WorksWithCrossTypeComparable()
    {
        var customValue = new CustomComparable(10);
        
        Assert.True(Is.LessThanOrEqual(customValue, 15));
        Assert.True(Is.LessThanOrEqual(customValue, 10)); // Equal case
        Assert.False(Is.LessThanOrEqual(customValue, 5));
    }

    #endregion

    #region LessThanOrEqual String Tests

    [Fact]
    public void LessThanOrEqual_String_ReturnsTrue_WhenLeftIsLessThanOrEqualToRight()
    {
        // Less than
        Assert.True(Is.LessThanOrEqual("a", "b", StringComparison.Ordinal));
        Assert.True(Is.LessThanOrEqual("apple", "banana", StringComparison.Ordinal));
        
        // Equal
        Assert.True(Is.LessThanOrEqual("hello", "hello", StringComparison.Ordinal));
        Assert.True(Is.LessThanOrEqual("Hello", "hello", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void LessThanOrEqual_String_ReturnsFalse_WhenLeftIsGreaterThanRight()
    {
        Assert.False(Is.LessThanOrEqual("z", "a", StringComparison.Ordinal));
        Assert.False(Is.LessThanOrEqual("banana", "apple", StringComparison.Ordinal));
    }

    [Fact]
    public void LessThanOrEqual_String_ThrowsArgumentNullException_WhenLeftValueIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => Is.LessThanOrEqual(null!, "test", StringComparison.Ordinal));
        
        Assert.Equal("lValue", exception.ParamName);
    }

    [Fact]
    public void LessThanOrEqual_String_ThrowsArgumentNullException_WhenRightValueIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => Is.LessThanOrEqual("test", null!, StringComparison.Ordinal));
        
        Assert.Equal("rValue", exception.ParamName);
    }

    [Fact]
    public void LessThanOrEqual_String_InvokesCallback_WithCorrectResult()
    {
        bool? callbackResult = null;
        SGuardCallback callback = result => callbackResult = result == GuardOutcome.Success;

        // True case (less than)
        var result1 = Is.LessThanOrEqual("a", "z", StringComparison.Ordinal, callback);
        Assert.True(result1);
        Assert.True(callbackResult);

        // True case (equal)
        callbackResult = null;
        var result2 = Is.LessThanOrEqual("hello", "hello", StringComparison.Ordinal, callback);
        Assert.True(result2);
        Assert.True(callbackResult);

        // False case
        callbackResult = null;
        var result3 = Is.LessThanOrEqual("z", "a", StringComparison.Ordinal, callback);
        Assert.False(result3);
        Assert.False(callbackResult);
    }

    [Fact]
    public void LessThanOrEqual_String_DoesNotThrow_WhenCallbackThrowsException()
    {
        SGuardCallback throwingCallback = _ => throw new InvalidOperationException("Test exception");

        var result1 = Is.LessThanOrEqual("a", "z", StringComparison.Ordinal, throwingCallback);
        Assert.True(result1);

        var result2 = Is.LessThanOrEqual("z", "a", StringComparison.Ordinal, throwingCallback);
        Assert.False(result2);
    }

    [Theory]
    [InlineData(StringComparison.Ordinal)]
    [InlineData(StringComparison.OrdinalIgnoreCase)]
    [InlineData(StringComparison.CurrentCulture)]
    [InlineData(StringComparison.CurrentCultureIgnoreCase)]
    [InlineData(StringComparison.InvariantCulture)]
    [InlineData(StringComparison.InvariantCultureIgnoreCase)]
    public void LessThanOrEqual_String_WorksWithAllStringComparisonTypes(StringComparison comparison)
    {
        var result = Is.LessThanOrEqual("a", "b", comparison);
        Assert.IsType<bool>(result);
    }

    #endregion

    #region Edge Cases and Integration Tests

    [Fact]
    public void LessThan_Methods_HandleEmptyStrings()
    {
        Assert.False(Is.LessThan("", "", StringComparison.Ordinal));
        Assert.True(Is.LessThan("", "a", StringComparison.Ordinal));
        Assert.False(Is.LessThan("a", "", StringComparison.Ordinal));
        
        Assert.True(Is.LessThanOrEqual("", "", StringComparison.Ordinal));
        Assert.True(Is.LessThanOrEqual("", "a", StringComparison.Ordinal));
        Assert.False(Is.LessThanOrEqual("a", "", StringComparison.Ordinal));
    }

    [Fact]
    public void LessThan_Methods_HandleUnicodeStrings()
    {
        // Unicode characters
        Assert.True(Is.LessThan("café", "cafë", StringComparison.Ordinal));
    }

    [Fact]
    public void LessThan_Methods_WorkConsistentlyWithCompareTo()
    {
        // Verify that our methods work consistently with CompareTo
        var testValues = new[] { 1, 5, 10, 15, 20 };
        
        for (int i = 0; i < testValues.Length; i++)
        {
            for (int j = 0; j < testValues.Length; j++)
            {
                var left = testValues[i];
                var right = testValues[j];
                
                var expectedLessThan = left.CompareTo(right) < 0;
                var actualLessThan = Is.LessThan(left, right);
                Assert.Equal(expectedLessThan, actualLessThan);
                
                var expectedLessThanOrEqual = left.CompareTo(right) <= 0;
                var actualLessThanOrEqual = Is.LessThanOrEqual(left, right);
                Assert.Equal(expectedLessThanOrEqual, actualLessThanOrEqual);
            }
        }
    }

    #endregion
}