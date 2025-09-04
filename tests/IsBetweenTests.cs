namespace SGuard.Tests;

public sealed class IsBetweenTests
{
    #region Integer Tests
        
    [Theory]
    [InlineData(5, 1, 10)]
    [InlineData(3, 1, 10)]
    [InlineData(8, 1, 10)]
    public void Between_WithIntegerInRange_ReturnsTrue(int value, int min, int max)
    {
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.True(result);
    }
        
    [Theory]
    [InlineData(0, 1, 10)]
    [InlineData(11, 1, 10)]
    [InlineData(-5, 1, 10)]
    [InlineData(15, 1, 10)]
    public void Between_WithIntegerOutOfRange_ReturnsFalse(int value, int min, int max)
    {
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.False(result);
    }
        
    [Theory]
    [InlineData(1, 1, 10)]
    [InlineData(10, 1, 10)]
    public void Between_WithIntegerEqualToBoundaries_ReturnsTrue(int value, int min, int max)
    {
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.True(result);
    }
        
    #endregion
        
    #region Double Tests
        
    [Theory]
    [InlineData(5.5, 1.0, 10.0)]
    [InlineData(2.7, 1.0, 10.0)]
    [InlineData(9.1, 1.0, 10.0)]
    public void Between_WithDoubleInRange_ReturnsTrue(double value, double min, double max)
    {
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.True(result);
    }
        
    [Theory]
    [InlineData(0.9, 1.0, 10.0)]
    [InlineData(10.1, 1.0, 10.0)]
    [InlineData(-1.5, 1.0, 10.0)]
    public void Between_WithDoubleOutOfRange_ReturnsFalse(double value, double min, double max)
    {
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.False(result);
    }
        
    [Theory]
    [InlineData(1.0, 1.0, 10.0)]
    [InlineData(10.0, 1.0, 10.0)]
    public void Between_WithDoubleEqualToBoundaries_ReturnsTrue(double value, double min, double max)
    {
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.True(result);
    }
        
    #endregion
        
    #region DateTime Tests
        
    [Fact]
    public void Between_WithDateTimeInRange_ReturnsTrue()
    {
        // Arrange
        var min = new DateTime(2023, 1, 1);
        var max = new DateTime(2023, 12, 31);
        var value = new DateTime(2023, 6, 15);
            
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.True(result);
    }
        
    [Fact]
    public void Between_WithDateTimeOutOfRange_ReturnsFalse()
    {
        // Arrange
        var min = new DateTime(2023, 1, 1);
        var max = new DateTime(2023, 12, 31);
        var value = new DateTime(2024, 1, 1);
            
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.False(result);
    }
        
    [Fact]
    public void Between_WithDateTimeEqualToMinBoundary_ReturnsTrue()
    {
        // Arrange
        var min = new DateTime(2023, 1, 1);
        var max = new DateTime(2023, 12, 31);
        var value = new DateTime(2023, 1, 1);
            
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.True(result);
    }
        
    [Fact]
    public void Between_WithDateTimeEqualToMaxBoundary_ReturnsTrue()
    {
        // Arrange
        var min = new DateTime(2023, 1, 1);
        var max = new DateTime(2023, 12, 31);
        var value = new DateTime(2023, 12, 31);
            
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.True(result);
    }
        
    #endregion
        
    #region String Tests
        
    [Theory]
    [InlineData("charlie", "alpha", "echo")]
    [InlineData("bravo", "alpha", "echo")]
    [InlineData("delta", "alpha", "echo")]
    public void Between_WithStringInRange_ReturnsTrue(string value, string min, string max)
    {
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.True(result);
    }
        
    [Theory]
    [InlineData("foxtrot", "alpha", "echo")]
    [InlineData("apple", "bravo", "echo")]
    public void Between_WithStringOutOfRange_ReturnsFalse(string value, string min, string max)
    {
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.False(result);
    }
        
    [Theory]
    [InlineData("alpha", "alpha", "echo")]
    [InlineData("echo", "alpha", "echo")]
    public void Between_WithStringEqualToBoundaries_ReturnsTrue(string value, string min, string max)
    {
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.True(result);
    }
        
    #endregion
        
    #region Decimal Tests
        
    [Theory]
    [InlineData("0.9", "1.0", "10.0")]
    [InlineData("10.1", "1.0", "10.0")]
    public void Between_WithDecimalOutOfRange_ReturnsFalse(string valueStr, string minStr, string maxStr)
    {
        // Arrange
        var value = decimal.Parse(valueStr);
        var min = decimal.Parse(minStr);
        var max = decimal.Parse(maxStr);
            
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.False(result);
    }
        
    #endregion
        
    #region Callback Tests
        
    [Fact]
    public void Between_WithCallbackAndTrueResult_InvokesCallbackWithSuccess()
    {
        // Arrange
        GuardOutcome? callbackResult = null;
        var value = 5;
        var min = 1;
        var max = 10;
            
        // Act
        var result = Is.Between(value, min, max, outcome => callbackResult = outcome);
            
        // Assert
        Assert.True(result);
        Assert.Equal(GuardOutcome.Success, callbackResult);
    }
        
    [Fact]
    public void Between_WithCallbackAndFalseResult_InvokesCallbackWithFailure()
    {
        // Arrange
        GuardOutcome? callbackResult = null;
        var value = 15;
        var min = 1;
        var max = 10;
            
        // Act
        var result = Is.Between(value, min, max, outcome => callbackResult = outcome);
            
        // Assert
        Assert.False(result);
        Assert.Equal(GuardOutcome.Failure, callbackResult);
    }
        
    [Fact]
    public void Between_WithNullCallback_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => Is.Between(5, 1, 10, null));
        Assert.Null(exception);
    }
        
    #endregion
        
    #region Edge Cases
        
    [Fact]
    public void Between_WithNegativeRange_WorksCorrectly()
    {
        // Arrange
        var value = -5;
        var min = -10;
        var max = -1;
            
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.True(result);
    }
        
    [Fact]
    public void Between_WithSameMinMax_WorksCorrectly()
    {
        // Arrange
        var value = 5;
        var min = 5;
        var max = 5;
            
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.True(result);
    }
        
    [Fact]
    public void Between_WithReversedMinMax_WorksCorrectly()
    {
        // Arrange
        var value = 5;
        var min = 10;
        var max = 1;
            
        // Act
        var result = Is.Between(value, min, max);
            
        // Assert
        Assert.False(result);
    }
        
    #endregion
        
    #region Performance Tests
        
    [Fact]
    public void Between_MultipleCallsPerformance_ExecutesWithinReasonableTime()
    {
        // Arrange
        var iterations = 100000;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
        // Act
        for (int i = 0; i < iterations; i++)
        {
            Is.Between(i % 100, 0, 99);
        }
            
        stopwatch.Stop();
            
        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"Expected execution under 1000ms, actual: {stopwatch.ElapsedMilliseconds}ms");
    }
        
    #endregion
}