using BenchmarkDotNet.Attributes;

namespace SGuard.Benchmark.GreaterThan;

[Config(typeof(Config))]
public class IsGreaterThanBenchmark
{
    private int small = 1, large = 1000, zero = 0, negative = -100;
    private string a = "a", b = "b", empty = "", upperA = "A";
    private SGuardCallback callback;

    [GlobalSetup]
    public void Setup()
    {
        callback = outcome => { };
    }

    // Int comparisons
    [Benchmark] public bool SmallVsLarge() => Is.GreaterThan(small, large);
    [Benchmark] public bool LargeVsSmall() => Is.GreaterThan(large, small);
    [Benchmark] public bool ZeroVsNegative() => Is.GreaterThan(zero, negative);
    [Benchmark] public bool LargeVsLarge() => Is.GreaterThan(large, large);
    [Benchmark] public bool SmallVsLarge_WithCallback() => Is.GreaterThan(small, large, callback);
    [Benchmark] public bool LargeVsSmall_WithCallback() => Is.GreaterThan(large, small, callback);
    [Benchmark] public bool ZeroVsNegative_WithCallback() => Is.GreaterThan(zero, negative, callback);
    [Benchmark] public bool LargeVsLarge_WithCallback() => Is.GreaterThan(large, large, callback);

    // String comparisons
    [Benchmark] public bool A_vs_B() => Is.GreaterThan(a, b);
    [Benchmark] public bool B_vs_A() => Is.GreaterThan(b, a);
    [Benchmark] public bool Empty_vs_A() => Is.GreaterThan(empty, a);
    [Benchmark] public bool A_vs_UpperA() => Is.GreaterThan(a, upperA);
    [Benchmark] public bool A_vs_B_WithCallback() => Is.GreaterThan(a, b, callback);
    [Benchmark] public bool B_vs_A_WithCallback() => Is.GreaterThan(b, a, callback);
    [Benchmark] public bool Empty_vs_A_WithCallback() => Is.GreaterThan(empty, a, callback);
    [Benchmark] public bool A_vs_UpperA_WithCallback() => Is.GreaterThan(a, upperA, callback);
}

