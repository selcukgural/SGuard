using System.Diagnostics;
using System.Reflection;

namespace SGuard.Tests;

/// <summary>
/// A passing guard must not allocate: guards run on every call of the code they protect.
/// </summary>
public sealed class ThrowIfAllocationTests
{
    /// <summary>
    /// Runs only against an optimized SGuard build. Without optimization the JIT boxes value types in generic type checks
    /// such as <c>value is double</c>, which an optimized build removes.
    /// </summary>
    private sealed class OptimizedBuildTheoryAttribute : TheoryAttribute
    {
        public OptimizedBuildTheoryAttribute()
        {
            if (typeof(ThrowIf).Assembly.GetCustomAttribute<DebuggableAttribute>()?.IsJITOptimizerDisabled == true)
            {
                Skip = "Allocations are only meaningful in an optimized (Release) build.";
            }
        }
    }

    private static readonly SGuardCallback Callback = _ => { };
    private static readonly int[] Array = [1, 2, 3];
    private static readonly List<int> List = [1, 2, 3];
    private static readonly Guid Id = Guid.NewGuid();
    private static readonly string Text = "value";
    private static readonly int? NullableInt = 3;

    public static TheoryData<string, Action> PassingGuards() => new()
    {
        { "GreaterThan", () => ThrowIf.GreaterThan(1, 2) },
        { "GreaterThan + callback", () => ThrowIf.GreaterThan(1, 2, Callback) },
        { "GreaterThanOrEqual", () => ThrowIf.GreaterThanOrEqual(1, 2) },
        { "LessThan", () => ThrowIf.LessThan(2, 1) },
        { "LessThan + callback", () => ThrowIf.LessThan(2, 1, Callback) },
        { "LessThanOrEqual", () => ThrowIf.LessThanOrEqual(2, 1) },
        { "Between", () => ThrowIf.Between(20, 1, 10) },
        { "Between string", () => ThrowIf.Between("z", "a", "m", StringComparison.Ordinal) },
        { "NullOrEmpty string", () => ThrowIf.NullOrEmpty(Text) },
        { "NullOrEmpty string + callback", () => ThrowIf.NullOrEmpty(Text, Callback) },
        { "NullOrEmpty int", () => ThrowIf.NullOrEmpty(5) },
        { "NullOrEmpty Guid", () => ThrowIf.NullOrEmpty(Id) },
        { "NullOrEmpty array", () => ThrowIf.NullOrEmpty(Array) },
        { "NullOrEmpty list", () => ThrowIf.NullOrEmpty(List) },
        { "GreaterThan decimal", () => ThrowIf.GreaterThan(1m, 2m) },
        { "Between decimal", () => ThrowIf.Between(20m, 1m, 10m) },
        { "NullOrEmpty decimal", () => ThrowIf.NullOrEmpty(5m) },
        { "Any array", () => ThrowIf.Any(Array, x => x > 10) },
        { "Any list", () => ThrowIf.Any(List, x => x > 10) },
        { "All array", () => ThrowIf.All(Array, x => x > 10) },
        { "All list", () => ThrowIf.All(List, x => x > 10) },
        { "Null", () => ThrowIf.Null(Text) },
        { "Null nullable int", () => ThrowIf.Null(NullableInt) },
        { "Null + callback", () => ThrowIf.Null(Text, Callback) },
        { "Null + factory", () => ThrowIf.Null(Text, () => new InvalidOperationException()) },
        { "NullOrWhiteSpace", () => ThrowIf.NullOrWhiteSpace(Text) },
        { "Negative int", () => ThrowIf.Negative(1) },
        { "Negative decimal", () => ThrowIf.Negative(1m) },
        { "Negative double", () => ThrowIf.Negative(1d) },
        { "Negative + factory", () => ThrowIf.Negative(1, () => new InvalidOperationException()) },
        { "NegativeOrZero", () => ThrowIf.NegativeOrZero(1) },
        { "Zero", () => ThrowIf.Zero(1) },
        { "OutOfRange", () => ThrowIf.OutOfRange(5, 1, 10) },
        { "OutOfRange decimal", () => ThrowIf.OutOfRange(5m, 1m, 10m) },
        { "OutOfRange string", () => ThrowIf.OutOfRange("m", "a", "z", StringComparison.Ordinal) },
        { "Equal", () => ThrowIf.Equal(1, 2) },
        { "Equal Guid", () => ThrowIf.Equal(Id, Guid.Empty) },
        { "NotEqual", () => ThrowIf.NotEqual(1, 1) },
        { "Equal string", () => ThrowIf.Equal(Text, "other", StringComparison.OrdinalIgnoreCase) },
        { "NotDefined", () => ThrowIf.NotDefined(DayOfWeek.Monday) },
        { "LongerThan", () => ThrowIf.LongerThan(Text, 10) },
        { "GreaterThan + factory", () => ThrowIf.GreaterThan(1, 2, () => new InvalidOperationException()) },
        { "Between + factory", () => ThrowIf.Between(20, 1, 10, () => new InvalidOperationException()) },
        { "NullOrEmpty + factory", () => ThrowIf.NullOrEmpty(Text, () => new InvalidOperationException()) },
        { "Any + factory", () => ThrowIf.Any(Array, x => x > 10, () => new InvalidOperationException()) },
        { "ShorterThan", () => ThrowIf.ShorterThan(Text, 1) },
    };

    public static TheoryData<string, Func<bool>> IsChecks() => new()
    {
        { "Is.Null", () => Is.Null(Text) },
        { "Is.NullOrWhiteSpace", () => Is.NullOrWhiteSpace(Text) },
        { "Is.Negative", () => Is.Negative(1) },
        { "Is.Zero", () => Is.Zero(1m) },
        { "Is.OutOfRange", () => Is.OutOfRange(5, 1, 10) },
        { "Is.Equal", () => Is.Equal(1, 2) },
        { "Is.NotDefined", () => Is.NotDefined(DayOfWeek.Monday) },
        { "Is.LongerThan", () => Is.LongerThan(Text, 10) },
        { "Is.CreditCard", () => Is.CreditCard("4111 1111 1111 1111") },
    };

    [OptimizedBuildTheory]
    [MemberData(nameof(PassingGuards))]
    public void PassingGuard_DoesNotAllocate(string name, Action guard)
    {
        // Warm up so that JIT and tiering work doesn't count.
        for (var i = 0; i < 100; i++)
        {
            guard();
        }

        var before = GC.GetAllocatedBytesForCurrentThread();

        for (var i = 0; i < 1000; i++)
        {
            guard();
        }

        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.True(allocated == 0, $"{name} allocated {allocated} bytes over 1000 calls.");
    }

    [OptimizedBuildTheory]
    [MemberData(nameof(IsChecks))]
    public void IsCheck_DoesNotAllocate(string name, Func<bool> check)
    {
        for (var i = 0; i < 100; i++)
        {
            check();
        }

        var before = GC.GetAllocatedBytesForCurrentThread();

        for (var i = 0; i < 1000; i++)
        {
            check();
        }

        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.True(allocated == 0, $"{name} allocated {allocated} bytes over 1000 calls.");
    }
}
