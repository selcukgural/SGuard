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
    public void NullOrEmpty_Span_WithAllNulls_DoesNotThrow()
    {
        // Arrange: like an array or a collection, a span with elements is not empty
        var array = new string?[] { null, null };

        // Act & Assert
        ThrowIf.NullOrEmpty((ReadOnlySpan<string?>)array);
        ThrowIf.NullOrEmpty(array);
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
    public void Is_NullOrEmpty_Span_WithAllNulls_ReturnsFalse_LikeTheArray()
    {
        // Arrange
        var array = new string?[] { null, null };

        // Act & Assert
        Assert.False(Is.NullOrEmpty((ReadOnlySpan<string?>)array));
        Assert.False(Is.NullOrEmpty(array));
        Assert.True(Is.NullOrEmpty(ReadOnlySpan<string?>.Empty));
        Assert.True(Is.NullOrEmpty(Array.Empty<string?>()));
    }
}
