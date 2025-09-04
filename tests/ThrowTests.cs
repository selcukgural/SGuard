using System.Reflection;
using SGuard.Exceptions;

namespace SGuard.Tests;

public sealed class ThrowTests
{
    private static Type GetThrowType()
    {
        var asm = typeof(Is).Assembly;
        var t = asm.GetType("SGuard.Throw", throwOnError: true);
        return t!;
    }

    [Fact]
    public void BetweenException_ShouldThrow_TypeAndNonEmptyMessage()
    {
        // Arrange
        const int test = 5;
        const int min = 1;
        const int max = 10;

        // Act
        var ex = InvokeAndCapture(
            methodName: "BetweenException",
            genericArgs: [typeof(int), typeof(int), typeof(int)],
            parameters: [test, min, max]);

        // Assert
        var typed = Assert.IsType<BetweenException>(ex);
        Assert.False(string.IsNullOrWhiteSpace(typed.Message));
    }

    [Fact]
    public void NullOrEmptyException_ShouldThrow_TypeAndNonEmptyMessage()
    {
        // Arrange
        const string value = "not-empty";

        // Act
        var ex = InvokeAndCapture(
            methodName: "NullOrEmptyException",
            genericArgs: [typeof(string)],
            parameters: [value]);

        // Assert
        var typed = Assert.IsType<NullOrEmptyException>(ex);
        Assert.False(string.IsNullOrWhiteSpace(typed.Message));
    }

    [Fact]
    public void GreaterThanException_ShouldThrow_TypeAndNonEmptyMessage()
    {
        // Arrange
        const int l = 5;
        const int r = 3;

        // Act
        var ex = InvokeAndCapture(
            methodName: "GreaterThanException",
            genericArgs: [typeof(int), typeof(int)],
            parameters: [l, r]);

        // Assert
        var typed = Assert.IsType<GreaterThanException>(ex);
        Assert.False(string.IsNullOrWhiteSpace(typed.Message));
    }

    [Fact]
    public void GreaterThanOrEqualException_ShouldThrow_TypeMessageAndData()
    {
        // Arrange
        const int l = 5;
        const int r = 5;

        // Act
        var ex = InvokeAndCapture(
            methodName: "GreaterThanOrEqualException",
            genericArgs: [typeof(int), typeof(int)],
            parameters: [l, r]);

        // Assert
        var typed = Assert.IsType<GreaterThanOrEqualException>(ex);

        Assert.False(string.IsNullOrWhiteSpace(typed.Message));

        if (typed.Data.Contains("left"))  Assert.Equal(l, typed.Data["left"]);
        if (typed.Data.Contains("right")) Assert.Equal(r, typed.Data["right"]);
        if (typed.Data.Contains("leftExpr"))  Assert.False(string.IsNullOrWhiteSpace(typed.Data["leftExpr"]?.ToString()));
        if (typed.Data.Contains("rightExpr")) Assert.False(string.IsNullOrWhiteSpace(typed.Data["rightExpr"]?.ToString()));
    }

    [Fact]
    public void LessThanException_ShouldThrow_TypeAndNonEmptyMessage()
    {
        // Arrange
        const int l = 3;
        const int r = 5;

        // Act
        var ex = InvokeAndCapture(
            methodName: "LessThanException",
            genericArgs: [typeof(int), typeof(int)],
            parameters: [l, r]);

        // Assert
        var typed = Assert.IsType<LessThanException>(ex);
        Assert.False(string.IsNullOrWhiteSpace(typed.Message));
    }

    [Fact]
    public void LessThanOrEqualException_ShouldThrow_TypeAndNonEmptyMessage()
    {
        // Arrange
        const int l = 5;
        const int r = 5;

        // Act
        var ex = InvokeAndCapture(
            methodName: "LessThanOrEqualException",
            genericArgs: [typeof(int), typeof(int)],
            parameters: [l, r]);

        // Assert
        var typed = Assert.IsType<LessThanOrEqualException>(ex);
        Assert.False(string.IsNullOrWhiteSpace(typed.Message));
    }

    [Fact]
    public void That_ShouldThrow_TheSameInstance()
    {
        // Arrange
        var original = new InvalidOperationException("boom");

        // Act
        var ex = InvokeAndCapture(
            methodName: "That",
            genericArgs: [typeof(InvalidOperationException)],
            parameters: [original]);

        // Assert
        Assert.Same(original, ex);
        Assert.Equal("boom", ex.Message);
    }

    private static Exception InvokeAndCapture(string methodName, Type[] genericArgs, object?[]? parameters)
    {
        var throwType = GetThrowType();
        const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        var candidates = throwType.GetMethods(flags)
                                  .Where(m => m.Name == methodName && m.IsGenericMethodDefinition == (genericArgs.Length > 0))
                                  .Where(m => genericArgs.Length == 0 || m.GetGenericArguments().Length == genericArgs.Length)
                                  .Where(m => m.GetParameters().Length == (parameters?.Length ?? 0))
                                  .ToArray();

        var candidate = candidates.FirstOrDefault();
        if (candidate == null)
        {
            throw new InvalidOperationException($"Method not found: {methodName} with {genericArgs.Length} generic args and {parameters?.Length ?? 0} parameters");
        }

        var method = genericArgs.Length > 0 ? candidate.MakeGenericMethod(genericArgs) : candidate;

        try
        {
            method.Invoke(null, parameters);
            throw new InvalidOperationException("Expected an exception to be thrown, but none was thrown.");
        }
        catch (TargetInvocationException ex)
        {
            return ex.InnerException!;
        }
    }
}