using BenchmarkDotNet.Attributes;
using SGuard.Exceptions;

namespace SGuard.Benchmark.All;

[Config(typeof(Config))]
public class ThrowIfAllBenchmark
{
    private List<int> allTrue1000, allTrue5000, allTrue10000, allTrue15000;
    private List<int> oneFalse1000, oneFalse5000, oneFalse10000, oneFalse15000;
    private Func<int, bool> predicate;
    private SGuardCallback callback;

    [GlobalSetup]
    public void Setup()
    {
        predicate = x => x > 0;
        callback = outcome => { };

        allTrue1000 = Enumerable.Repeat(1, 1000).ToList();
        allTrue5000 = Enumerable.Repeat(1, 5000).ToList();
        allTrue10000 = Enumerable.Repeat(1, 10000).ToList();
        allTrue15000 = Enumerable.Repeat(1, 15000).ToList();

        oneFalse1000 = Enumerable.Repeat(1, 999).Concat(new[] { 0 }).ToList();
        oneFalse5000 = Enumerable.Repeat(1, 4999).Concat(new[] { 0 }).ToList();
        oneFalse10000 = Enumerable.Repeat(1, 9999).Concat(new[] { 0 }).ToList();
        oneFalse15000 = Enumerable.Repeat(1, 14999).Concat(new[] { 0 }).ToList();
    }

    [Benchmark] public void All_True_1000() => RunGuard(allTrue1000, predicate);
    [Benchmark] public void All_True_5000() => RunGuard(allTrue5000, predicate);
    [Benchmark] public void All_True_10000() => RunGuard(allTrue10000, predicate);
    [Benchmark] public void All_True_15000() => RunGuard(allTrue15000, predicate);

    [Benchmark] public void All_False_1000() => RunGuard(oneFalse1000, predicate);
    [Benchmark] public void All_False_5000() => RunGuard(oneFalse5000, predicate);
    [Benchmark] public void All_False_10000() => RunGuard(oneFalse10000, predicate);
    [Benchmark] public void All_False_15000() => RunGuard(oneFalse15000, predicate);

    [Benchmark] public void All_True_1000_WithCallback() => RunGuard(allTrue1000, predicate, callback);
    [Benchmark] public void All_True_5000_WithCallback() => RunGuard(allTrue5000, predicate, callback);
    [Benchmark] public void All_True_10000_WithCallback() => RunGuard(allTrue10000, predicate, callback);
    [Benchmark] public void All_True_15000_WithCallback() => RunGuard(allTrue15000, predicate, callback);

    [Benchmark] public void All_False_1000_WithCallback() => RunGuard(oneFalse1000, predicate, callback);
    [Benchmark] public void All_False_5000_WithCallback() => RunGuard(oneFalse5000, predicate, callback);
    [Benchmark] public void All_False_10000_WithCallback() => RunGuard(oneFalse10000, predicate, callback);
    [Benchmark] public void All_False_15000_WithCallback() => RunGuard(oneFalse15000, predicate, callback);

    private static void RunGuard(List<int> source, Func<int, bool> pred, SGuardCallback? cb = null)
    {
        try
        {
            ThrowIf.All(source, pred, cb);
        }
        catch (AllException)
        {
            // Exception expected for all-true cases
        }
    }
}

