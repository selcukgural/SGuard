using SGuard.Exceptions;

namespace SGuard.Tests;

[CollectionDefinition(nameof(SGuardOptionsCollection), DisableParallelization = true)]
public sealed class SGuardOptionsCollection;

[Collection(nameof(SGuardOptionsCollection))]
public sealed class ExceptionValueTests
{
    private sealed record Request(string? Name, int Age);

    private sealed class ThrowingToString : IComparable<ThrowingToString>
    {
        public int CompareTo(ThrowingToString? other) => 1;
        public override string ToString() => throw new InvalidOperationException("ToString failed");
    }

    [Fact]
    public void Between_LeavesValuesOut_AndNamesCallerExpressions_ByDefault()
    {
        const string apiToken = "s3cr3t-token";

        var ex = Assert.Throws<BetweenException>(() => ThrowIf.Between(apiToken, "a", "z", StringComparison.Ordinal));

        Assert.DoesNotContain("s3cr3t", ex.Message);
        Assert.Contains("value=apiToken", ex.Message);
        Assert.Contains("min=\"a\"", ex.Message);
        Assert.False(ex.Data.Contains("value"));
        Assert.False(ex.Data.Contains("min"));
        Assert.False(ex.Data.Contains("max"));
        Assert.Equal("apiToken", ex.Data["valueExpr"]);
    }

    [Fact]
    public void ComparisonGuards_LeaveValuesOut_AndNameCallerExpressions_ByDefault()
    {
        var request = new Request("x", 4217);
        const int limit = 1000;

        var greater = Assert.Throws<GreaterThanException>(() => ThrowIf.GreaterThan(request.Age, limit));
        var greaterOrEqual = Assert.Throws<GreaterThanOrEqualException>(() => ThrowIf.GreaterThanOrEqual(request.Age, limit));
        var less = Assert.Throws<LessThanException>(() => ThrowIf.LessThan(limit, request.Age));
        var lessOrEqual = Assert.Throws<LessThanOrEqualException>(() => ThrowIf.LessThanOrEqual(limit, request.Age));

        Assert.All(new Exception[] { greater, greaterOrEqual }, ex =>
        {
            Assert.DoesNotContain("4217", ex.Message);
            Assert.Contains("left=request.Age, right=limit", ex.Message);
            Assert.False(ex.Data.Contains("left"));
            Assert.False(ex.Data.Contains("right"));
        });

        Assert.All(new Exception[] { less, lessOrEqual }, ex =>
        {
            Assert.DoesNotContain("4217", ex.Message);
            Assert.Contains("left=limit, right=request.Age", ex.Message);
            Assert.False(ex.Data.Contains("left"));
            Assert.False(ex.Data.Contains("right"));
        });
    }

    [Fact]
    public void NullOrEmpty_NamesCallerExpression()
    {
        var request = new Request("", 1);
        ReadOnlySpan<string?> names = [];

        var ex = Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty(request.Name));
        var spanException = CaptureSpan(names);

        Assert.Equal("Value 'request.Name' is null or empty.", ex.Message);
        Assert.False(ex.Data.Contains("value"));
        Assert.Equal("Value 'names' is null or empty.", spanException.Message);

        static NullOrEmptyException CaptureSpan(ReadOnlySpan<string?> names)
        {
            try
            {
                ThrowIf.NullOrEmpty(names);
            }
            catch (NullOrEmptyException e)
            {
                return e;
            }

            throw new InvalidOperationException("Expected NullOrEmptyException.");
        }
    }

    [Fact]
    public void IncludeValuesInExceptions_WritesFormattedValues()
    {
        var request = new Request("x", 4217);

        WithValuesIncluded(() =>
        {
            var ex = Assert.Throws<GreaterThanException>(() => ThrowIf.GreaterThan(request.Age, 1000));

            Assert.StartsWith("'4217' is greater than '1000'.", ex.Message);
            Assert.Contains("left=request.Age, right=1000", ex.Message);
            Assert.Equal("4217", ex.Data["left"]);
            Assert.Equal("1000", ex.Data["right"]);
        });
    }

    [Fact]
    public void IncludeValuesInExceptions_TruncatesLongValues()
    {
        var value = new string('a', 500);

        WithValuesIncluded(() =>
        {
            var ex = Assert.Throws<BetweenException>(() => ThrowIf.Between(value, "a", "b", StringComparison.Ordinal));
            var expected = new string('a', SGuardOptions.MaxValueLength) + "…";

            Assert.Equal(expected, ex.Data["value"]);
            Assert.Contains($"Value '{expected}' is between 'a' and 'b'.", ex.Message);
        });
    }

    [Fact]
    public void IncludeValuesInExceptions_DoesNotSplitSurrogatePairs()
    {
        // The emoji takes two chars, and the limit falls between them.
        var value = new string('a', SGuardOptions.MaxValueLength - 1) + "😀tail";

        WithValuesIncluded(() =>
        {
            var ex = Assert.Throws<BetweenException>(() => ThrowIf.Between(value, "a", "z", StringComparison.Ordinal));

            Assert.Equal(new string('a', SGuardOptions.MaxValueLength - 1) + "…", ex.Data["value"]);
        });
    }

    [Fact]
    public void IncludeValuesInExceptions_KeepsGuardException_WhenToStringThrows()
    {
        WithValuesIncluded(() =>
        {
            var ex = Assert.Throws<GreaterThanException>(() => ThrowIf.GreaterThan(new ThrowingToString(), new ThrowingToString()));

            Assert.StartsWith("'<ThrowingToString>' is greater than '<ThrowingToString>'.", ex.Message);
        });
    }

    [Fact]
    public void IncludeValuesInExceptions_SelectorAndSpanGuards_DontReportANullValue()
    {
        var request = new Request("", 1);

        WithValuesIncluded(() =>
        {
            var selector = Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty(request, r => r.Name));
            var span = Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty(ReadOnlySpan<int>.Empty));

            Assert.Equal("Value 'r => r.Name' is null or empty.", selector.Message);
            Assert.Equal("r => r.Name", selector.ParamName);
            Assert.False(selector.Data.Contains("value"));
            Assert.Equal("Value 'ReadOnlySpan<int>.Empty' is null or empty.", span.Message);
        });
    }

    [Fact]
    public void NewGuards_LeaveValuesOut_ByDefault()
    {
        var balance = -4217;
        var sameBalance = balance;
        var day = (DayOfWeek)4217;
        var password = "hunter2";

        Assert.All(
            new Exception[]
            {
                Assert.Throws<NegativeException>(() => ThrowIf.Negative(balance)),
                Assert.Throws<NegativeOrZeroException>(() => ThrowIf.NegativeOrZero(balance)),
                Assert.Throws<OutOfRangeException>(() => ThrowIf.OutOfRange(balance, 0, 10)),
                Assert.Throws<EqualException>(() => ThrowIf.Equal(balance, sameBalance)),
                Assert.Throws<NotEqualException>(() => ThrowIf.NotEqual(balance, 0)),
                Assert.Throws<NotDefinedException>(() => ThrowIf.NotDefined(day)),
                Assert.Throws<NullOrWhiteSpaceException>(() => ThrowIf.NullOrWhiteSpace(" ")),
                Assert.Throws<ShorterThanException>(() => ThrowIf.ShorterThan(password, 8)),
                Assert.Throws<LongerThanException>(() => ThrowIf.LongerThan(password, 3)),
            },
            ex =>
            {
                Assert.DoesNotContain("4217", ex.Message);
                Assert.DoesNotContain("hunter2", ex.Message);
                Assert.DoesNotContain("length=", ex.Message);
                Assert.False(ex.Data.Contains("value"));
                Assert.False(ex.Data.Contains("left"));
                Assert.False(ex.Data.Contains("length"));
            });
    }

    [Fact]
    public void IncludeValuesInExceptions_NewGuards_WriteFormattedValues()
    {
        var balance = -5;
        var name = "abcdef";

        WithValuesIncluded(() =>
        {
            var negative = Assert.Throws<NegativeException>(() => ThrowIf.Negative(balance));
            var outOfRange = Assert.Throws<OutOfRangeException>(() => ThrowIf.OutOfRange(balance, 0, 10));
            var equal = Assert.Throws<EqualException>(() => ThrowIf.Equal(balance, -5));
            var notDefined = Assert.Throws<NotDefinedException>(() => ThrowIf.NotDefined((DayOfWeek)9));
            var longer = Assert.Throws<LongerThanException>(() => ThrowIf.LongerThan(name, 3));

            Assert.Equal("Value 'balance' is negative. Actual: value=-5.", negative.Message);
            Assert.Equal("-5", negative.Data["value"]);
            Assert.Equal("Value '-5' is not between '0' and '10'. Actual: value=balance, min=0, max=10.", outOfRange.Message);
            Assert.Equal("'-5' is equal to '-5'. Actual: left=balance, right=-5.", equal.Message);
            Assert.Equal("Value '(DayOfWeek)9' is not a defined value of 'DayOfWeek'. Actual: value=9.", notDefined.Message);
            Assert.Equal("Value 'name' is longer than '3' characters. Actual: length=6.", longer.Message);
            Assert.DoesNotContain("abcdef", longer.Message);
            Assert.Equal("6", longer.Data["length"]);
        });
    }

    private static void WithValuesIncluded(Action assertions)
    {
        SGuardOptions.IncludeValuesInExceptions = true;

        try
        {
            assertions();
        }
        finally
        {
            SGuardOptions.IncludeValuesInExceptions = false;
        }
    }
}
