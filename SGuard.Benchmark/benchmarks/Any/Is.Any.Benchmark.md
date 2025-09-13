```

BenchmarkDotNet v0.15.2, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD


```
| Method                       | Mean       | Error    | StdDev   |
|----------------------------- |-----------:|---------:|---------:|
| All_False_1000               |   281.1 ns |  1.57 ns |  1.47 ns |
| All_False_5000               | 1,351.9 ns |  4.46 ns |  4.18 ns |
| All_False_10000              | 2,688.8 ns |  7.16 ns |  6.35 ns |
| All_False_15000              | 4,026.8 ns |  8.07 ns |  7.15 ns |
| One_True_1000                |   281.0 ns |  0.50 ns |  0.42 ns |
| One_True_5000                | 1,351.2 ns |  4.61 ns |  4.09 ns |
| One_True_10000               | 2,707.6 ns | 17.36 ns | 16.24 ns |
| One_True_15000               | 4,024.2 ns |  8.85 ns |  7.84 ns |
| All_False_1000_WithCallback  |   281.0 ns |  1.22 ns |  1.14 ns |
| All_False_5000_WithCallback  | 1,351.7 ns |  3.44 ns |  3.05 ns |
| All_False_10000_WithCallback | 2,687.8 ns |  5.18 ns |  4.33 ns |
| All_False_15000_WithCallback | 4,022.9 ns | 10.77 ns |  9.55 ns |
| One_True_1000_WithCallback   |   281.1 ns |  1.30 ns |  1.16 ns |
| One_True_5000_WithCallback   | 1,351.7 ns |  5.83 ns |  5.46 ns |
| One_True_10000_WithCallback  | 2,685.1 ns |  4.73 ns |  3.95 ns |
| One_True_15000_WithCallback  | 4,018.0 ns |  8.71 ns |  7.27 ns |
