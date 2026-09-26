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
}
