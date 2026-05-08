using SGuard;
using SGuard.Exceptions;

namespace SGuard.Tests;

public class NullOrEmptySpanTests
{
    [Fact]
    public void NullOrEmpty_Span_WithEmptySpan_ThrowsException()
    {
        // Arrange
        var emptyArray = Array.Empty<string>();

        // Act & Assert
        Assert.Throws<NullOrEmptyException>(() => 
        {
             ThrowIf.NullOrEmpty((ReadOnlySpan<string>)emptyArray);
        });
    }

    [Fact]
    public void NullOrEmpty_Span_WithAllNulls_ThrowsException()
    {
        // Arrange
        var array = new string[] { null, null };

        // Act & Assert
        Assert.Throws<NullOrEmptyException>(() => 
        {
            ThrowIf.NullOrEmpty((ReadOnlySpan<string>)array);
        });
    }

    [Fact]
    public void NullOrEmpty_Span_WithItems_DoesNotThrow()
    {
        // Arrange
        var span = (ReadOnlySpan<string>)new string[] { "test" };

        // Act
        ThrowIf.NullOrEmpty(span);

        // Assert
        // No exception
    }
    [Fact]
    public void Is_NullOrEmpty_Span_WithAllNulls_ReturnsTrue()
    {
        // Arrange
        var array = new string[] { null, null };

        // Act
        var result = Is.NullOrEmpty((ReadOnlySpan<string>)array);

        // Assert
        Assert.True(result);
    }
}
