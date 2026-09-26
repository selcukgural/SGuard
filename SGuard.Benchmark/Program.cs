using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using SGuard.Benchmark.All;
using SGuard.Benchmark.Any;
using SGuard.Benchmark.Between;
using SGuard.Benchmark.GreaterThan;
using SGuard.Benchmark.LessThan;
using SGuard.Benchmark.NullOrEmpty;


namespace SGuard.Benchmark;

public class Program
{
    private static readonly Type[] Benchmarks =
    [
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
    ];

    /// <summary>
    /// Runs the benchmarks selected by BenchmarkDotNet's command-line options, or all of them when none are given.
    /// For example: <c>--filter '*NullOrEmpty*' --runtimes net8.0 net10.0 --job short</c>.
    /// </summary>
    public static void Main(string[] args)
    {
        BenchmarkSwitcher.FromTypes(Benchmarks).Run(args.Length == 0 ? ["--filter", "*"] : args);
    }
}

public class Config : ManualConfig
{
    public Config()
    {
        AddExporter(BenchmarkDotNet.Exporters.MarkdownExporter.GitHub);
        AddDiagnoser(MemoryDiagnoser.Default);
    }
}