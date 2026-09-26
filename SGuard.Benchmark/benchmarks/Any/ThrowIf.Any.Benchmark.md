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
| All_False_1000               | .NET 10.0 | .NET 10.0 |    280.7 ns |    53.32 ns |   2.92 ns |  0.53 |    0.01 |      - |         - |          NA |
| All_False_1000               | .NET 8.0  | .NET 8.0  |    528.0 ns |    35.89 ns |   1.97 ns |  1.00 |    0.00 |      - |         - |          NA |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_False_5000               | .NET 10.0 | .NET 10.0 |  1,275.7 ns |   539.95 ns |  29.60 ns |  0.49 |    0.02 |      - |         - |          NA |
| All_False_5000               | .NET 8.0  | .NET 8.0  |  2,586.7 ns | 1,415.81 ns |  77.61 ns |  1.00 |    0.04 |      - |         - |          NA |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_False_10000              | .NET 10.0 | .NET 10.0 |  2,590.7 ns | 1,331.36 ns |  72.98 ns |  0.49 |    0.01 |      - |         - |          NA |
| All_False_10000              | .NET 8.0  | .NET 8.0  |  5,260.4 ns |   281.35 ns |  15.42 ns |  1.00 |    0.00 |      - |         - |          NA |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_False_15000              | .NET 10.0 | .NET 10.0 |  3,895.6 ns | 1,101.53 ns |  60.38 ns |  0.49 |    0.01 |      - |         - |          NA |
| All_False_15000              | .NET 8.0  | .NET 8.0  |  8,000.7 ns | 2,207.23 ns | 120.99 ns |  1.00 |    0.02 |      - |         - |          NA |
|                              |           |           |             |             |           |       |         |        |           |             |
| One_True_1000                | .NET 10.0 | .NET 10.0 |  2,335.4 ns |   181.73 ns |   9.96 ns |  0.18 |    0.00 | 0.0267 |     224 B |        0.97 |
| One_True_1000                | .NET 8.0  | .NET 8.0  | 13,175.3 ns |   635.10 ns |  34.81 ns |  1.00 |    0.00 | 0.0153 |     232 B |        1.00 |
|                              |           |           |             |             |           |       |         |        |           |             |
| One_True_5000                | .NET 10.0 | .NET 10.0 |  3,496.5 ns |   122.56 ns |   6.72 ns |  0.23 |    0.00 | 0.0267 |     224 B |        0.97 |
| One_True_5000                | .NET 8.0  | .NET 8.0  | 15,438.6 ns | 1,933.61 ns | 105.99 ns |  1.00 |    0.01 |      - |     232 B |        1.00 |
|                              |           |           |             |             |           |       |         |        |           |             |
| One_True_10000               | .NET 10.0 | .NET 10.0 |  4,807.4 ns |    73.06 ns |   4.00 ns |  0.27 |    0.00 | 0.0229 |     224 B |        0.97 |
| One_True_10000               | .NET 8.0  | .NET 8.0  | 18,105.4 ns | 2,457.68 ns | 134.71 ns |  1.00 |    0.01 |      - |     232 B |        1.00 |
|                              |           |           |             |             |           |       |         |        |           |             |
| One_True_15000               | .NET 10.0 | .NET 10.0 |  6,249.2 ns | 1,314.20 ns |  72.04 ns |  0.30 |    0.00 | 0.0229 |     224 B |        0.97 |
| One_True_15000               | .NET 8.0  | .NET 8.0  | 20,841.6 ns | 3,252.40 ns | 178.28 ns |  1.00 |    0.01 |      - |     232 B |        1.00 |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_False_1000_WithCallback  | .NET 10.0 | .NET 10.0 |    261.3 ns |     5.37 ns |   0.29 ns |  0.48 |    0.01 |      - |         - |          NA |
| All_False_1000_WithCallback  | .NET 8.0  | .NET 8.0  |    539.7 ns |   125.02 ns |   6.85 ns |  1.00 |    0.02 |      - |         - |          NA |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_False_5000_WithCallback  | .NET 10.0 | .NET 10.0 |  1,330.6 ns |   222.61 ns |  12.20 ns |  0.51 |    0.01 |      - |         - |          NA |
| All_False_5000_WithCallback  | .NET 8.0  | .NET 8.0  |  2,596.0 ns |   393.00 ns |  21.54 ns |  1.00 |    0.01 |      - |         - |          NA |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_False_10000_WithCallback | .NET 10.0 | .NET 10.0 |  2,556.9 ns | 1,866.75 ns | 102.32 ns |  0.49 |    0.02 |      - |         - |          NA |
| All_False_10000_WithCallback | .NET 8.0  | .NET 8.0  |  5,256.2 ns |   841.41 ns |  46.12 ns |  1.00 |    0.01 |      - |         - |          NA |
|                              |           |           |             |             |           |       |         |        |           |             |
| All_False_15000_WithCallback | .NET 10.0 | .NET 10.0 |  3,992.2 ns |   437.67 ns |  23.99 ns |  0.51 |    0.00 |      - |         - |          NA |
| All_False_15000_WithCallback | .NET 8.0  | .NET 8.0  |  7,756.4 ns |   560.06 ns |  30.70 ns |  1.00 |    0.00 |      - |         - |          NA |
|                              |           |           |             |             |           |       |         |        |           |             |
| One_True_1000_WithCallback   | .NET 10.0 | .NET 10.0 |  2,403.6 ns |   131.48 ns |   7.21 ns |  0.18 |    0.00 | 0.0267 |     224 B |        0.97 |
| One_True_1000_WithCallback   | .NET 8.0  | .NET 8.0  | 13,198.4 ns | 1,883.99 ns | 103.27 ns |  1.00 |    0.01 | 0.0153 |     232 B |        1.00 |
|                              |           |           |             |             |           |       |         |        |           |             |
| One_True_5000_WithCallback   | .NET 10.0 | .NET 10.0 |  3,443.7 ns |   489.08 ns |  26.81 ns |  0.22 |    0.00 | 0.0267 |     224 B |        0.97 |
| One_True_5000_WithCallback   | .NET 8.0  | .NET 8.0  | 15,389.3 ns | 1,175.23 ns |  64.42 ns |  1.00 |    0.01 |      - |     232 B |        1.00 |
|                              |           |           |             |             |           |       |         |        |           |             |
| One_True_10000_WithCallback  | .NET 10.0 | .NET 10.0 |  4,847.4 ns |   836.21 ns |  45.84 ns |  0.26 |    0.00 | 0.0229 |     224 B |        0.97 |
| One_True_10000_WithCallback  | .NET 8.0  | .NET 8.0  | 18,451.6 ns | 3,953.63 ns | 216.71 ns |  1.00 |    0.01 |      - |     232 B |        1.00 |
|                              |           |           |             |             |           |       |         |        |           |             |
| One_True_15000_WithCallback  | .NET 10.0 | .NET 10.0 |  6,204.4 ns |   246.29 ns |  13.50 ns |  0.30 |    0.00 | 0.0229 |     224 B |        0.97 |
| One_True_15000_WithCallback  | .NET 8.0  | .NET 8.0  | 20,853.0 ns | 1,519.56 ns |  83.29 ns |  1.00 |    0.00 |      - |     232 B |        1.00 |
