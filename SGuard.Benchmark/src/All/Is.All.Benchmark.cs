using BenchmarkDotNet.Attributes;

namespace SGuard.Benchmark.All;

[Config(typeof(Config))]
public class IsAllBenchmark
{
    private List<int> allTrueList;
    private List<int> oneFalseList;
    private Func<int, bool> predicate;
    private SGuardCallback callback;

    private List<int> allTrue1000, allTrue5000, allTrue10000, allTrue15000;
    private List<int> oneFalse1000, oneFalse5000, oneFalse10000, oneFalse15000;

    [GlobalSetup]
    public void Setup()
    {
        allTrueList = new List<int> { 1, 2, 3, 4, 5 };
        oneFalseList = new List<int> { 1, 2, 3, 4, 0 };
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

    [Benchmark]
    public bool All_True_NoCallback() => Is.All(allTrueList, predicate);

    [Benchmark]
    public bool All_True_WithCallback() => Is.All(allTrueList, predicate, callback);

    [Benchmark]
    public bool All_False_NoCallback() => Is.All(oneFalseList, predicate);

    [Benchmark]
    public bool All_False_WithCallback() => Is.All(oneFalseList, predicate, callback);

    [Benchmark]
    public bool All_True_1000() => Is.All(allTrue1000, predicate);

    [Benchmark]
    public bool All_True_5000() => Is.All(allTrue5000, predicate);

    [Benchmark]
    public bool All_True_10000() => Is.All(allTrue10000, predicate);

    [Benchmark]
    public bool All_True_15000() => Is.All(allTrue15000, predicate);

    [Benchmark]
    public bool All_False_1000() => Is.All(oneFalse1000, predicate);

    [Benchmark]
    public bool All_False_5000() => Is.All(oneFalse5000, predicate);

    [Benchmark]
    public bool All_False_10000() => Is.All(oneFalse10000, predicate);

    [Benchmark]
    public bool All_False_15000() => Is.All(oneFalse15000, predicate);

    [Benchmark]
    public bool All_True_1000_WithCallback() => Is.All(allTrue1000, predicate, callback);

    [Benchmark]
    public bool All_True_5000_WithCallback() => Is.All(allTrue5000, predicate, callback);

    [Benchmark]
    public bool All_True_10000_WithCallback() => Is.All(allTrue10000, predicate, callback);

    [Benchmark]
    public bool All_True_15000_WithCallback() => Is.All(allTrue15000, predicate, callback);

    [Benchmark]
    public bool All_False_1000_WithCallback() => Is.All(oneFalse1000, predicate, callback);

    [Benchmark]
    public bool All_False_5000_WithCallback() => Is.All(oneFalse5000, predicate, callback);

    [Benchmark]
    public bool All_False_10000_WithCallback() => Is.All(oneFalse10000, predicate, callback);

    [Benchmark]
    public bool All_False_15000_WithCallback() => Is.All(oneFalse15000, predicate, callback);
}