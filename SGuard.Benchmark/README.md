# SGuard.Benchmark

This project contains performance benchmarks for various methods in the SGuard library, using BenchmarkDotNet.

## How to Run

1. Open a terminal in the `SGuard.Benchmark` directory.
2. Run the benchmarks. The project targets `net8.0`, `net9.0` and `net10.0`, so `-f` is required; it selects the host,
   and `--runtimes` selects the runtimes the benchmarks run on:
   ```bash
   # Everything, on .NET 8 and .NET 10, as the committed results were recorded
   dotnet run -c Release -f net10.0 -- --filter '*' --runtimes net8.0 net10.0 --job short

   # One class, on the host runtime, with BenchmarkDotNet's default (longer, more precise) job
   dotnet run -c Release -f net10.0 -- --filter '*ThrowIfNullOrEmpt*'
   ```
   Without arguments every benchmark runs on the host runtime. Any other
   [BenchmarkDotNet command-line option](https://benchmarkdotnet.org/articles/guides/console-args.html) works too.
   Every benchmark reports allocations (`MemoryDiagnoser`).
3. Results are written to `BenchmarkDotNet.Artifacts/results/`. The markdown files under `benchmarks/` are not updated
   automatically; copy the new results there if you want to commit them.

## Benchmark Results

Detailed benchmark results for each method can be found in the corresponding markdown files in the [benchmarks](benchmarks/) folder:

### All
- [Is.All.Benchmark.md](benchmarks/All/Is.All.Benchmark.md)
- [ThrowIf.All.Benchmark.md](benchmarks/All/ThrowIf.All.Benchmark.md)

### Any
- [Is.Any.Benchmark.md](benchmarks/Any/Is.Any.Benchmark.md)
- [ThrowIf.Any.Benchmark.md](benchmarks/Any/ThrowIf.Any.Benchmark.md)

### Between
- [Is.Between.Benchmark.md](benchmarks/Between/Is.Between.Benchmark.md)
- [ThrowIf.Between.Benchmark.md](benchmarks/Between/ThrowIf.Between.Benchmark.md)

### GreaterThan
- [Is.GreaterThan.Benchmark.md](benchmarks/GreaterThan/Is.GreaterThan.Benchmark.md)
- [ThrowIf.GreaterThan.Benchmark.md](benchmarks/GreaterThan/ThrowIf.GreaterThan.Benchmark.md)

### LessThan
- [Is.LessThan.Benchmark.md](benchmarks/LessThan/Is.LessThan.Benchmark.md)
- [ThrowIf.LessThan.Benchmark.md](benchmarks/LessThan/ThrowIf.LessThan.Benchmark.md)

### NullOrEmpty
- [Is.NullOrEmpty.Benchmark.md](benchmarks/NullOrEmpty/Is.NullOrEmpty.Benchmark.md)
- [ThrowIf.NullOrEmpty.Benchmark.md](benchmarks/NullOrEmpty/ThrowIf.NullOrEmpty.Benchmark.md)

The committed results were recorded in September 2026 on an Apple M3 Max with BenchmarkDotNet 0.15.8, on .NET 8.0.3
and .NET 10.0.5, with the command above (`--job short`: 3 iterations, so treat small differences as noise). Each
table has a row per runtime and an `Allocated` column. A mean of `0.0000 ns` (with a `ZeroMeasurement` warning in the
BenchmarkDotNet log) means the call is indistinguishable from an empty method returning the same value, i.e. it costs
less than BenchmarkDotNet can resolve (a fraction of a nanosecond). The benchmarks return their results, so the calls
are not optimized away.

Summary:
- A passing guard costs about as much as a hand-written `if`: under 1 ns on .NET 10 and 0.6–3 ns on .NET 8, with
  no allocation. A callback adds under a nanosecond.
- A failing `ThrowIf.*` guard (throw plus catch) takes about 2 µs on .NET 10 and 13 µs on .NET 8, and allocates the
  exception (roughly 0.2–0.7 KB).
- `Any`/`All` over 1,000 elements take about 280 ns on .NET 10 (540 ns on .NET 8) and grow linearly.
- A cached selector (`ThrowIf.NullOrEmpty(value, x => x.Member)`) takes about 230 ns (300 ns on .NET 8) and 568 B,
  most of it for building the expression tree at the call site.

For more details, see the markdown files for each method in the benchmarks folder.
