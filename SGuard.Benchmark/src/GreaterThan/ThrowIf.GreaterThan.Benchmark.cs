using BenchmarkDotNet.Attributes;

namespace SGuard.Benchmark.GreaterThan;

[Config(typeof(Config))]
public class ThrowIfGreaterThanBenchmark
{
    private int _small = 1, _large = 1000, _zero = 0, _negative = -100;
    private string _a = "a", _b = "b", _empty = "", _upperA = "A";
    private SGuardCallback? _callback;

    [GlobalSetup]
    public void Setup()
    {
        _callback = _ => { };
    }

    // Int comparisons
    [Benchmark] public void SmallVsLarge() => RunGuard(_small, _large);
    [Benchmark] public void LargeVsSmall() => RunGuard(_large, _small);
    [Benchmark] public void ZeroVsNegative() => RunGuard(_zero, _negative);
    [Benchmark] public void LargeVsLarge() => RunGuard(_large, _large);
    [Benchmark] public void SmallVsLarge_WithCallback() => RunGuard(_small, _large, _callback);
    [Benchmark] public void LargeVsSmall_WithCallback() => RunGuard(_large, _small, _callback);
    [Benchmark] public void ZeroVsNegative_WithCallback() => RunGuard(_zero, _negative, _callback);
    [Benchmark] public void LargeVsLarge_WithCallback() => RunGuard(_large, _large, _callback);

    // String comparisons
    [Benchmark] public void A_vs_B() => RunGuard(_a, _b);
    [Benchmark] public void B_vs_A() => RunGuard(_b, _a);
    [Benchmark] public void Empty_vs_A() => RunGuard(_empty, _a);
    [Benchmark] public void A_vs_UpperA() => RunGuard(_a, _upperA);
    [Benchmark] public void A_vs_B_WithCallback() => RunGuard(_a, _b, _callback);
    [Benchmark] public void B_vs_A_WithCallback() => RunGuard(_b, _a, _callback);
    [Benchmark] public void Empty_vs_A_WithCallback() => RunGuard(_empty, _a, _callback);
    [Benchmark] public void A_vs_UpperA_WithCallback() => RunGuard(_a, _upperA, _callback);

    private static void RunGuard<T>(T l, T r, SGuardCallback? cb = null)
        where T : IComparable<T>
    {
        try { ThrowIf.GreaterThan(l, r, cb); } catch (Exception) { }
    }
}
