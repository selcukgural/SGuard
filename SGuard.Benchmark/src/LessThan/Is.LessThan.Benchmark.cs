using BenchmarkDotNet.Attributes;

namespace SGuard.Benchmark.LessThan;

[Config(typeof(Config))]
public class IsLessThanBenchmark
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
    [Benchmark] public bool SmallVsLarge() => Is.LessThan(small, large);
    [Benchmark] public bool LargeVsSmall() => Is.LessThan(large, small);
    [Benchmark] public bool ZeroVsNegative() => Is.LessThan(zero, negative);
    [Benchmark] public bool LargeVsLarge() => Is.LessThan(large, large);
    [Benchmark] public bool SmallVsLarge_WithCallback() => Is.LessThan(small, large, callback);
    [Benchmark] public bool LargeVsSmall_WithCallback() => Is.LessThan(large, small, callback);
    [Benchmark] public bool ZeroVsNegative_WithCallback() => Is.LessThan(zero, negative, callback);
    [Benchmark] public bool LargeVsLarge_WithCallback() => Is.LessThan(large, large, callback);

    // String comparisons
    [Benchmark] public bool A_vs_B() => Is.LessThan(a, b);
    [Benchmark] public bool B_vs_A() => Is.LessThan(b, a);
    [Benchmark] public bool Empty_vs_A() => Is.LessThan(empty, a);
    [Benchmark] public bool A_vs_UpperA() => Is.LessThan(a, upperA);
    [Benchmark] public bool A_vs_B_WithCallback() => Is.LessThan(a, b, callback);
    [Benchmark] public bool B_vs_A_WithCallback() => Is.LessThan(b, a, callback);
    [Benchmark] public bool Empty_vs_A_WithCallback() => Is.LessThan(empty, a, callback);
    [Benchmark] public bool A_vs_UpperA_WithCallback() => Is.LessThan(a, upperA, callback);
}

