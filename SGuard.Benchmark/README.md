# SGuard.Benchmark

This project contains performance benchmarks for various methods in the SGuard library, using BenchmarkDotNet.

## How to Run

1. Open a terminal in the project root directory.
2. Run the following command:
   ```bash
   dotnet run -c Release
   ```
3. Results will be available in the `BenchmarkDotNet.Artifacts/results/` folder and in the relevant markdown files under the `benchmarks/` directory.

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

Summary:
- For small lists, execution time is in nanoseconds; for large lists, it increases to microseconds.
- Adding a callback introduces minimal overhead.
- Performance is consistent and efficient across all scenarios.

For more details, see the markdown files for each method in the benchmarks folder.
