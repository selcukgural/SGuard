```

BenchmarkDotNet v0.15.2, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD


```
| Method                       | Mean        | Error    | StdDev   |
|----------------------------- |------------:|---------:|---------:|
| All_False_1000               |    298.7 ns |  0.79 ns |  0.70 ns |
| All_False_5000               |  1,372.2 ns |  2.70 ns |  2.26 ns |
| All_False_10000              |  2,724.0 ns | 17.38 ns | 16.26 ns |
| All_False_15000              |  4,103.5 ns | 28.50 ns | 26.66 ns |
| One_True_1000                |  8,694.6 ns | 51.14 ns | 45.34 ns |
| One_True_5000                |  9,963.3 ns | 57.67 ns | 53.94 ns |
| One_True_10000               | 11,183.0 ns | 90.00 ns | 79.78 ns |
| One_True_15000               | 12,592.2 ns | 35.09 ns | 29.31 ns |
| All_False_1000_WithCallback  |    298.1 ns |  0.72 ns |  0.64 ns |
| All_False_5000_WithCallback  |  1,373.9 ns |  4.09 ns |  3.62 ns |
| All_False_10000_WithCallback |  2,712.0 ns |  5.11 ns |  4.27 ns |
| All_False_15000_WithCallback |  4,055.7 ns |  8.42 ns |  7.03 ns |
| One_True_1000_WithCallback   |  8,769.3 ns | 59.17 ns | 55.35 ns |
| One_True_5000_WithCallback   |  9,757.9 ns | 43.12 ns | 40.33 ns |
| One_True_10000_WithCallback  | 11,301.2 ns | 29.44 ns | 26.10 ns |
| One_True_15000_WithCallback  | 12,587.8 ns | 60.47 ns | 53.61 ns |
