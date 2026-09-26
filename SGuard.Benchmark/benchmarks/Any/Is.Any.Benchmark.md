```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 10.0.105
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 8.0  : .NET 8.0.3 (8.0.3, 8.0.324.11423), Arm64 RyuJIT armv8.0-a

IterationCount=3  LaunchCount=1  WarmupCount=3  

```
| Method                       | Job       | Runtime   | Mean       | Error        | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|----------------------------- |---------- |---------- |-----------:|-------------:|----------:|------:|--------:|----------:|------------:|
| All_False_1000               | .NET 10.0 | .NET 10.0 |   278.9 ns |     18.05 ns |   0.99 ns |  0.51 |    0.00 |         - |          NA |
| All_False_1000               | .NET 8.0  | .NET 8.0  |   543.3 ns |     63.56 ns |   3.48 ns |  1.00 |    0.01 |         - |          NA |
|                              |           |           |            |              |           |       |         |           |             |
| All_False_5000               | .NET 10.0 | .NET 10.0 | 1,341.6 ns |     69.69 ns |   3.82 ns |  0.50 |    0.00 |         - |          NA |
| All_False_5000               | .NET 8.0  | .NET 8.0  | 2,675.2 ns |     38.38 ns |   2.10 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |            |              |           |       |         |           |             |
| All_False_10000              | .NET 10.0 | .NET 10.0 | 2,674.6 ns |    107.25 ns |   5.88 ns |  0.50 |    0.00 |         - |          NA |
| All_False_10000              | .NET 8.0  | .NET 8.0  | 5,332.5 ns |     97.19 ns |   5.33 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |            |              |           |       |         |           |             |
| All_False_15000              | .NET 10.0 | .NET 10.0 | 4,013.5 ns |    359.98 ns |  19.73 ns |  0.50 |    0.00 |         - |          NA |
| All_False_15000              | .NET 8.0  | .NET 8.0  | 7,978.6 ns |    147.94 ns |   8.11 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |            |              |           |       |         |           |             |
| One_True_1000                | .NET 10.0 | .NET 10.0 |   412.5 ns |  2,415.59 ns | 132.41 ns |  0.71 |    0.21 |         - |          NA |
| One_True_1000                | .NET 8.0  | .NET 8.0  |   588.0 ns |  1,484.97 ns |  81.40 ns |  1.01 |    0.17 |         - |          NA |
|                              |           |           |            |              |           |       |         |           |             |
| One_True_5000                | .NET 10.0 | .NET 10.0 | 1,339.9 ns |     35.70 ns |   1.96 ns |  0.50 |    0.01 |         - |          NA |
| One_True_5000                | .NET 8.0  | .NET 8.0  | 2,706.2 ns |    776.41 ns |  42.56 ns |  1.00 |    0.02 |         - |          NA |
|                              |           |           |            |              |           |       |         |           |             |
| One_True_10000               | .NET 10.0 | .NET 10.0 | 2,676.0 ns |     68.50 ns |   3.75 ns |  0.50 |    0.00 |         - |          NA |
| One_True_10000               | .NET 8.0  | .NET 8.0  | 5,337.8 ns |     75.28 ns |   4.13 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |            |              |           |       |         |           |             |
| One_True_15000               | .NET 10.0 | .NET 10.0 | 4,300.2 ns |  5,018.99 ns | 275.11 ns |  0.52 |    0.04 |         - |          NA |
| One_True_15000               | .NET 8.0  | .NET 8.0  | 8,253.0 ns |  8,210.61 ns | 450.05 ns |  1.00 |    0.07 |         - |          NA |
|                              |           |           |            |              |           |       |         |           |             |
| All_False_1000_WithCallback  | .NET 10.0 | .NET 10.0 |   278.2 ns |     24.35 ns |   1.33 ns |  0.51 |    0.00 |         - |          NA |
| All_False_1000_WithCallback  | .NET 8.0  | .NET 8.0  |   541.2 ns |      9.44 ns |   0.52 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |            |              |           |       |         |           |             |
| All_False_5000_WithCallback  | .NET 10.0 | .NET 10.0 | 1,343.8 ns |     33.12 ns |   1.82 ns |  0.47 |    0.04 |         - |          NA |
| All_False_5000_WithCallback  | .NET 8.0  | .NET 8.0  | 2,867.2 ns |  5,850.34 ns | 320.68 ns |  1.01 |    0.13 |         - |          NA |
|                              |           |           |            |              |           |       |         |           |             |
| All_False_10000_WithCallback | .NET 10.0 | .NET 10.0 | 2,701.4 ns |    767.53 ns |  42.07 ns |  0.50 |    0.01 |         - |          NA |
| All_False_10000_WithCallback | .NET 8.0  | .NET 8.0  | 5,422.9 ns |  1,581.56 ns |  86.69 ns |  1.00 |    0.02 |         - |          NA |
|                              |           |           |            |              |           |       |         |           |             |
| All_False_15000_WithCallback | .NET 10.0 | .NET 10.0 | 4,013.1 ns |    568.35 ns |  31.15 ns |  0.50 |    0.00 |         - |          NA |
| All_False_15000_WithCallback | .NET 8.0  | .NET 8.0  | 7,990.3 ns |    156.51 ns |   8.58 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |            |              |           |       |         |           |             |
| One_True_1000_WithCallback   | .NET 10.0 | .NET 10.0 |   307.6 ns |    785.28 ns |  43.04 ns |  0.52 |    0.09 |         - |          NA |
| One_True_1000_WithCallback   | .NET 8.0  | .NET 8.0  |   607.4 ns |  1,836.23 ns | 100.65 ns |  1.02 |    0.20 |         - |          NA |
|                              |           |           |            |              |           |       |         |           |             |
| One_True_5000_WithCallback   | .NET 10.0 | .NET 10.0 | 1,341.8 ns |     37.16 ns |   2.04 ns |  0.50 |    0.00 |         - |          NA |
| One_True_5000_WithCallback   | .NET 8.0  | .NET 8.0  | 2,676.4 ns |    162.15 ns |   8.89 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |            |              |           |       |         |           |             |
| One_True_10000_WithCallback  | .NET 10.0 | .NET 10.0 | 2,681.0 ns |    233.81 ns |  12.82 ns |  0.50 |    0.00 |         - |          NA |
| One_True_10000_WithCallback  | .NET 8.0  | .NET 8.0  | 5,336.1 ns |    168.00 ns |   9.21 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |            |              |           |       |         |           |             |
| One_True_15000_WithCallback  | .NET 10.0 | .NET 10.0 | 4,378.7 ns | 11,948.03 ns | 654.91 ns |  0.55 |    0.07 |         - |          NA |
| One_True_15000_WithCallback  | .NET 8.0  | .NET 8.0  | 7,999.0 ns |    169.97 ns |   9.32 ns |  1.00 |    0.00 |         - |          NA |
