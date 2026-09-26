```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 10.0.105
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 8.0  : .NET 8.0.3 (8.0.3, 8.0.324.11423), Arm64 RyuJIT armv8.0-a

IterationCount=3  LaunchCount=1  WarmupCount=3  

```
| Method                       | Job       | Runtime   | Mean        | Error       | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|----------------------------- |---------- |---------- |------------:|------------:|----------:|------:|--------:|-------:|----------:|------------:|
| All_True_1000                | .NET 10.0 | .NET 10.0 |  2,319.7 ns |   444.00 ns |  24.34 ns |  0.18 |    0.00 | 0.0267 |     224 B |        0.97 |
| All_True_1000                | .NET 8.0  | .NET 8.0  | 13,147.7 ns |    40.59 ns |   2.22 ns |  1.00 |    0.00 | 0.0153 |     232 B |        1.00 |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_True_5000                | .NET 10.0 | .NET 10.0 |  3,431.3 ns |    68.82 ns |   3.77 ns |  0.22 |    0.00 | 0.0267 |     224 B |        0.97 |
| All_True_5000                | .NET 8.0  | .NET 8.0  | 15,372.4 ns | 1,419.09 ns |  77.79 ns |  1.00 |    0.01 |      - |     232 B |        1.00 |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_True_10000               | .NET 10.0 | .NET 10.0 |  4,871.3 ns |   552.17 ns |  30.27 ns |  0.27 |    0.00 | 0.0229 |     224 B |        0.97 |
| All_True_10000               | .NET 8.0  | .NET 8.0  | 18,247.1 ns | 1,137.31 ns |  62.34 ns |  1.00 |    0.00 |      - |     232 B |        1.00 |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_True_15000               | .NET 10.0 | .NET 10.0 |  6,313.5 ns | 1,487.36 ns |  81.53 ns |  0.30 |    0.00 | 0.0229 |     224 B |        0.97 |
| All_True_15000               | .NET 8.0  | .NET 8.0  | 20,803.1 ns | 1,843.53 ns | 101.05 ns |  1.00 |    0.01 |      - |     232 B |        1.00 |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_False_1000               | .NET 10.0 | .NET 10.0 |    275.2 ns |    18.69 ns |   1.02 ns |  0.52 |    0.00 |      - |         - |          NA |
| All_False_1000               | .NET 8.0  | .NET 8.0  |    527.5 ns |    78.24 ns |   4.29 ns |  1.00 |    0.01 |      - |         - |          NA |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_False_5000               | .NET 10.0 | .NET 10.0 |  1,328.9 ns |   416.89 ns |  22.85 ns |  0.50 |    0.01 |      - |         - |          NA |
| All_False_5000               | .NET 8.0  | .NET 8.0  |  2,641.2 ns |   609.52 ns |  33.41 ns |  1.00 |    0.02 |      - |         - |          NA |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_False_10000              | .NET 10.0 | .NET 10.0 |  2,664.6 ns |   158.15 ns |   8.67 ns |  0.50 |    0.00 |      - |         - |          NA |
| All_False_10000              | .NET 8.0  | .NET 8.0  |  5,358.5 ns | 1,010.73 ns |  55.40 ns |  1.00 |    0.01 |      - |         - |          NA |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_False_15000              | .NET 10.0 | .NET 10.0 |  3,950.1 ns | 1,304.82 ns |  71.52 ns |  0.50 |    0.01 |      - |         - |          NA |
| All_False_15000              | .NET 8.0  | .NET 8.0  |  7,955.4 ns |   437.43 ns |  23.98 ns |  1.00 |    0.00 |      - |         - |          NA |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_True_1000_WithCallback   | .NET 10.0 | .NET 10.0 |  2,373.9 ns |   318.10 ns |  17.44 ns |  0.18 |    0.00 | 0.0267 |     224 B |        0.97 |
| All_True_1000_WithCallback   | .NET 8.0  | .NET 8.0  | 13,219.3 ns | 2,082.35 ns | 114.14 ns |  1.00 |    0.01 | 0.0153 |     232 B |        1.00 |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_True_5000_WithCallback   | .NET 10.0 | .NET 10.0 |  3,421.6 ns |    29.32 ns |   1.61 ns |  0.22 |    0.00 | 0.0267 |     224 B |        0.97 |
| All_True_5000_WithCallback   | .NET 8.0  | .NET 8.0  | 15,363.4 ns | 1,347.36 ns |  73.85 ns |  1.00 |    0.01 |      - |     232 B |        1.00 |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_True_10000_WithCallback  | .NET 10.0 | .NET 10.0 |  4,856.9 ns |   700.24 ns |  38.38 ns |  0.27 |    0.00 | 0.0229 |     224 B |        0.97 |
| All_True_10000_WithCallback  | .NET 8.0  | .NET 8.0  | 18,012.9 ns |   890.31 ns |  48.80 ns |  1.00 |    0.00 |      - |     232 B |        1.00 |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_True_15000_WithCallback  | .NET 10.0 | .NET 10.0 |  6,294.5 ns |   429.05 ns |  23.52 ns |  0.30 |    0.00 | 0.0229 |     224 B |        0.97 |
| All_True_15000_WithCallback  | .NET 8.0  | .NET 8.0  | 20,807.8 ns | 3,508.80 ns | 192.33 ns |  1.00 |    0.01 |      - |     232 B |        1.00 |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_False_1000_WithCallback  | .NET 10.0 | .NET 10.0 |    264.7 ns |    63.56 ns |   3.48 ns |  0.49 |    0.01 |      - |         - |          NA |
| All_False_1000_WithCallback  | .NET 8.0  | .NET 8.0  |    545.7 ns |    37.04 ns |   2.03 ns |  1.00 |    0.00 |      - |         - |          NA |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_False_5000_WithCallback  | .NET 10.0 | .NET 10.0 |  1,354.9 ns |   338.94 ns |  18.58 ns |  0.51 |    0.01 |      - |         - |          NA |
| All_False_5000_WithCallback  | .NET 8.0  | .NET 8.0  |  2,639.8 ns |   335.78 ns |  18.41 ns |  1.00 |    0.01 |      - |         - |          NA |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_False_10000_WithCallback | .NET 10.0 | .NET 10.0 |  2,599.2 ns | 1,475.83 ns |  80.90 ns |  0.49 |    0.01 |      - |         - |          NA |
| All_False_10000_WithCallback | .NET 8.0  | .NET 8.0  |  5,321.8 ns |   170.80 ns |   9.36 ns |  1.00 |    0.00 |      - |         - |          NA |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_False_15000_WithCallback | .NET 10.0 | .NET 10.0 |  4,007.2 ns |   369.15 ns |  20.23 ns |  0.51 |    0.01 |      - |         - |          NA |
| All_False_15000_WithCallback | .NET 8.0  | .NET 8.0  |  7,879.6 ns | 4,353.95 ns | 238.65 ns |  1.00 |    0.04 |      - |         - |          NA |
