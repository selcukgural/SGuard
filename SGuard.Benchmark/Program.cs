using BenchmarkDotNet.Configs;
using SGuard.Benchmark.All;
using SGuard.Benchmark.Any;
using SGuard.Benchmark.Between;
using SGuard.Benchmark.GreaterThan;
using SGuard.Benchmark.LessThan;
using SGuard.Benchmark.NullOrEmpty;


namespace SGuard.Benchmark;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkDotNet.Running.BenchmarkRunner.Run([
            typeof(ThrowIfBetweenBenchmark),
            typeof(ThrowIfNullOrEmptBenchmark),
            typeof(ThrowIfLessThanBenchmark),
            typeof(ThrowIfGreaterThanBenchmark),
            typeof(ThrowIfAnyBenchmark),
            typeof(ThrowIfAllBenchmark),

            typeof(IsNullOrEmptyBenchmark),
            typeof(IsLessThanBenchmark),
            typeof(IsGreaterThanBenchmark),
            typeof(IsBetweenBenchmark),
            typeof(IsAnyBenchmark),
            typeof(IsAllBenchmark)
        ]);
        
    }
}

public class Config : ManualConfig
{
    public Config()
    {
        AddExporter(BenchmarkDotNet.Exporters.MarkdownExporter.GitHub);
    }
}