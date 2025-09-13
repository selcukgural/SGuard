```

BenchmarkDotNet v0.15.2, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD


```
| Method                       | Mean        | Error    | StdDev   |
|----------------------------- |------------:|---------:|---------:|
| All_True_1000                |  8,532.4 ns | 58.70 ns | 54.91 ns |
| All_True_5000                |  9,692.1 ns | 42.22 ns | 39.50 ns |
| All_True_10000               | 11,140.9 ns | 48.95 ns | 45.79 ns |
| All_True_15000               | 12,568.8 ns | 59.93 ns | 53.13 ns |
| All_False_1000               |    299.6 ns |  1.57 ns |  1.39 ns |
| All_False_5000               |  1,369.9 ns |  2.17 ns |  1.70 ns |
| All_False_10000              |  2,712.2 ns |  5.17 ns |  4.32 ns |
| All_False_15000              |  4,052.4 ns |  7.21 ns |  6.39 ns |
| All_True_1000_WithCallback   |  8,446.7 ns | 42.11 ns | 37.33 ns |
| All_True_5000_WithCallback   |  9,668.5 ns | 60.90 ns | 56.96 ns |
| All_True_10000_WithCallback  | 11,110.7 ns | 46.96 ns | 39.21 ns |
| All_True_15000_WithCallback  | 12,766.3 ns | 55.36 ns | 51.78 ns |
| All_False_1000_WithCallback  |    300.9 ns |  0.40 ns |  0.33 ns |
| All_False_5000_WithCallback  |  1,370.4 ns |  2.72 ns |  2.41 ns |
| All_False_10000_WithCallback |  2,708.0 ns |  9.08 ns |  7.58 ns |
| All_False_15000_WithCallback |  4,048.9 ns |  9.37 ns |  7.32 ns |
