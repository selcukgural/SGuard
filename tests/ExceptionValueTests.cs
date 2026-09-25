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
    public void IncludeValuesInExceptions_KeepsGuardException_WhenToStringThrows()
    {
        WithValuesIncluded(() =>
        {
            var ex = Assert.Throws<GreaterThanException>(() => ThrowIf.GreaterThan(new ThrowingToString(), new ThrowingToString()));

            Assert.StartsWith("'<ThrowingToString>' is greater than '<ThrowingToString>'.", ex.Message);
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
