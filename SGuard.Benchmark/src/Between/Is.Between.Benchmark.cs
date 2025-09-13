using BenchmarkDotNet.Attributes;

namespace SGuard.Benchmark.Between;

[Config(typeof(Config))]
public class IsBetweenBenchmark
{
    private int min = 10;
    private int max = 10000;
    private string minStr = "a";
    private string maxStr = "z";
    private SGuardCallback callback;

    [GlobalSetup]
    public void Setup()
    {
        callback = outcome => { };
    }

    [Benchmark] public bool Between_Int_True() => Is.Between(5000, min, max);
    [Benchmark] public bool Between_Int_False_Low() => Is.Between(5, min, max);
    [Benchmark] public bool Between_Int_False_High() => Is.Between(20000, min, max);
    [Benchmark] public bool Between_Int_True_WithCallback() => Is.Between(5000, min, max, callback);
    [Benchmark] public bool Between_Int_False_Low_WithCallback() => Is.Between(5, min, max, callback);
    [Benchmark] public bool Between_Int_False_High_WithCallback() => Is.Between(20000, min, max, callback);

    [Benchmark] public bool Between_String_True() => Is.Between("m", minStr, maxStr, StringComparison.Ordinal);
    [Benchmark] public bool Between_String_False_Low() => Is.Between("A", minStr, maxStr, StringComparison.Ordinal);
    [Benchmark] public bool Between_String_False_High() => Is.Between("zz", minStr, maxStr, StringComparison.Ordinal);
    [Benchmark] public bool Between_String_True_WithCallback() => Is.Between("m", minStr, maxStr, StringComparison.Ordinal, callback);
    [Benchmark] public bool Between_String_False_Low_WithCallback() => Is.Between("A", minStr, maxStr, StringComparison.Ordinal, callback);
    [Benchmark] public bool Between_String_False_High_WithCallback() => Is.Between("zz", minStr, maxStr, StringComparison.Ordinal, callback);

    // Edge cases for large numbers
    [Benchmark] public bool Between_Int_Edge_1000000_True() => Is.Between(500000, 10, 1000000);
    [Benchmark] public bool Between_Int_Edge_1000000_False() => Is.Between(1000001, 10, 1000000);
    [Benchmark] public bool Between_Int_Edge_1000000_True_WithCallback() => Is.Between(500000, 10, 1000000, callback);
    [Benchmark] public bool Between_Int_Edge_1000000_False_WithCallback() => Is.Between(1000001, 10, 1000000, callback);
}

