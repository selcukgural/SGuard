using SGuard.Exceptions;

namespace SGuard.Tests;

/// <summary>
/// From 3.0, each built-in exception derives from the BCL exception that describes it best, so existing
/// <c>catch</c> blocks and exception middleware keep working. All of them are still <see cref="ArgumentException"/>s.
/// </summary>
// Asserts exact messages, so it can't run while another test turns IncludeValuesInExceptions on.
[Collection(nameof(SGuardOptionsCollection))]
public sealed class ExceptionHierarchyTests
{
    public static TheoryData<Type, Type> BaseTypes() => new()
    {
        { typeof(NullException), typeof(ArgumentNullException) },
        { typeof(NullOrEmptyException), typeof(ArgumentException) },
        { typeof(NullOrWhiteSpaceException), typeof(ArgumentException) },
        { typeof(GreaterThanException), typeof(ArgumentOutOfRangeException) },
        { typeof(GreaterThanOrEqualException), typeof(ArgumentOutOfRangeException) },
        { typeof(LessThanException), typeof(ArgumentOutOfRangeException) },
        { typeof(LessThanOrEqualException), typeof(ArgumentOutOfRangeException) },
        { typeof(BetweenException), typeof(ArgumentOutOfRangeException) },
        { typeof(OutOfRangeException), typeof(ArgumentOutOfRangeException) },
        { typeof(NegativeException), typeof(ArgumentOutOfRangeException) },
        { typeof(NegativeOrZeroException), typeof(ArgumentOutOfRangeException) },
        { typeof(ZeroException), typeof(ArgumentOutOfRangeException) },
        { typeof(EqualException), typeof(ArgumentOutOfRangeException) },
        { typeof(NotEqualException), typeof(ArgumentOutOfRangeException) },
        { typeof(NotDefinedException), typeof(ArgumentOutOfRangeException) },
        { typeof(LongerThanException), typeof(ArgumentOutOfRangeException) },
        { typeof(ShorterThanException), typeof(ArgumentOutOfRangeException) },
        { typeof(AnyException), typeof(ArgumentException) },
        { typeof(AllException), typeof(ArgumentException) },
    };

    [Theory]
    [MemberData(nameof(BaseTypes))]
    public void BuiltInException_DerivesFromItsBclType_AndIsSealed(Type exceptionType, Type baseType)
    {
        Assert.Equal(baseType, exceptionType.BaseType);
        Assert.True(typeof(ArgumentException).IsAssignableFrom(exceptionType));
        Assert.True(exceptionType.IsSealed);
    }

    [Fact]
    public void EveryBuiltInExceptionIsListed()
    {
        var listed = BaseTypes().Select(row => (Type)row[0]).ToHashSet();
        var all = typeof(ThrowIf).Assembly.GetTypes()
            .Where(t => t.IsPublic && t.Namespace == "SGuard.Exceptions" && typeof(Exception).IsAssignableFrom(t));

        Assert.All(all, t => Assert.Contains(t, listed));
    }

    [Fact]
    public void ComparisonException_IsCaughtAsArgumentOutOfRangeException()
    {
        var quantity = 0;

        var ex = Assert.Throws<LessThanException>(() => ThrowIf.LessThan(quantity, 1));

        ArgumentOutOfRangeException asOutOfRange = ex;
        Assert.Equal("quantity", asOutOfRange.ParamName);
        Assert.Null(asOutOfRange.ActualValue);
    }

    [Fact]
    public void Messages_DontGainTheBclSuffixes()
    {
        var quantity = 0;

        var ex = Assert.Throws<LessThanException>(() => ThrowIf.LessThan(quantity, 1));

        // ArgumentException appends " (Parameter '...')" and ArgumentOutOfRangeException "Actual value was ..." when
        // their own fields are set; the guards keep the message they had before 3.0.
        Assert.Equal("Left value is less than right value. Actual: left=quantity, right=1.", ex.Message);
    }

    [Fact]
    public void MessageConstructors_TakeAMessage_NotAParameterName()
    {
        // ArgumentOutOfRangeException(string) and ArgumentNullException(string) take a parameter name; the SGuard
        // exceptions' (string) constructors take a message, as they did before 3.0.
        Assert.Equal("custom", new GreaterThanException("custom").Message);
        Assert.Null(new GreaterThanException("custom").ParamName);
        Assert.Equal("custom", new BetweenException("custom").Message);
        Assert.Equal("custom", new NullException("custom").Message);
        Assert.Equal("custom", new OutOfRangeException("custom").Message);
        Assert.Same(
            typeof(InvalidOperationException),
            new LessThanException("custom", new InvalidOperationException()).InnerException!.GetType());
    }
}
