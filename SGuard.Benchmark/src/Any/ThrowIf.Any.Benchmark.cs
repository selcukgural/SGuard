using BenchmarkDotNet.Attributes;
using SGuard.Exceptions;

namespace SGuard.Benchmark.Any;

[Config(typeof(Config))]
public class ThrowIfAnyBenchmark
{
    private List<int> allFalse1000, allFalse5000, allFalse10000, allFalse15000;
    private List<int> oneTrue1000, oneTrue5000, oneTrue10000, oneTrue15000;
    private Func<int, bool> predicate;
    private SGuardCallback callback;

    [GlobalSetup]
    public void Setup()
    {
        predicate = x => x > 0;
        callback = outcome => { };

        allFalse1000 = Enumerable.Repeat(0, 1000).ToList();
        allFalse5000 = Enumerable.Repeat(0, 5000).ToList();
        allFalse10000 = Enumerable.Repeat(0, 10000).ToList();
        allFalse15000 = Enumerable.Repeat(0, 15000).ToList();

        oneTrue1000 = Enumerable.Repeat(0, 999).Concat(new[] { 1 }).ToList();
        oneTrue5000 = Enumerable.Repeat(0, 4999).Concat(new[] { 1 }).ToList();
        oneTrue10000 = Enumerable.Repeat(0, 9999).Concat(new[] { 1 }).ToList();
        oneTrue15000 = Enumerable.Repeat(0, 14999).Concat(new[] { 1 }).ToList();
    }

    [Benchmark] public void All_False_1000() => RunGuard(allFalse1000, predicate);
    [Benchmark] public void All_False_5000() => RunGuard(allFalse5000, predicate);
    [Benchmark] public void All_False_10000() => RunGuard(allFalse10000, predicate);
    [Benchmark] public void All_False_15000() => RunGuard(allFalse15000, predicate);

    [Benchmark] public void One_True_1000() => RunGuard(oneTrue1000, predicate);
    [Benchmark] public void One_True_5000() => RunGuard(oneTrue5000, predicate);
    [Benchmark] public void One_True_10000() => RunGuard(oneTrue10000, predicate);
    [Benchmark] public void One_True_15000() => RunGuard(oneTrue15000, predicate);

    [Benchmark] public void All_False_1000_WithCallback() => RunGuard(allFalse1000, predicate, callback);
    [Benchmark] public void All_False_5000_WithCallback() => RunGuard(allFalse5000, predicate, callback);
    [Benchmark] public void All_False_10000_WithCallback() => RunGuard(allFalse10000, predicate, callback);
    [Benchmark] public void All_False_15000_WithCallback() => RunGuard(allFalse15000, predicate, callback);

    [Benchmark] public void One_True_1000_WithCallback() => RunGuard(oneTrue1000, predicate, callback);
    [Benchmark] public void One_True_5000_WithCallback() => RunGuard(oneTrue5000, predicate, callback);
    [Benchmark] public void One_True_10000_WithCallback() => RunGuard(oneTrue10000, predicate, callback);
    [Benchmark] public void One_True_15000_WithCallback() => RunGuard(oneTrue15000, predicate, callback);

    private static void RunGuard(List<int> source, Func<int, bool> pred, SGuardCallback? cb = null)
    {
        try
        {
            ThrowIf.Any(source, pred, cb);
        }
        catch (AnyException)
        {
            // Exception expected for one-true cases
        }
    }
}

