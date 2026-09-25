# SGuard.Benchmark

This project contains performance benchmarks for various methods in the SGuard library, using BenchmarkDotNet.

## How to Run

1. Open a terminal in the `SGuard.Benchmark` directory.
2. Run the benchmarks for one target framework (the project targets `net8.0` and `net9.0`, so `-f` is required):
   ```bash
   dotnet run -c Release -f net9.0
   ```
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

The committed results were recorded on .NET 9 before selector caching was added, so they don't reflect the selector
cache (see [Changelog.md](../Changelog.md)).

Summary:
- For small lists, execution time is in nanoseconds; for large lists, it increases to microseconds.
- A passing `ThrowIf.*` guard takes about 10 ns; a failing one (throw plus catch) takes several microseconds.

For more details, see the markdown files for each method in the benchmarks folder.
