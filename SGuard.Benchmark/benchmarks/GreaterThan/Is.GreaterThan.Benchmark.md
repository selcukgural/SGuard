```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 10.0.105
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 8.0  : .NET 8.0.3 (8.0.3, 8.0.324.11423), Arm64 RyuJIT armv8.0-a

IterationCount=3  LaunchCount=1  WarmupCount=3  

```
| Method                      | Job       | Runtime   | Mean       | Error       | StdDev    | Median     | Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------- |---------- |---------- |-----------:|------------:|----------:|-----------:|------:|--------:|----------:|------------:|
| SmallVsLarge                | .NET 10.0 | .NET 10.0 |  0.0000 ns |   0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |         - |           ? |
| SmallVsLarge                | .NET 8.0  | .NET 8.0  |  0.0000 ns |   0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |         - |           ? |
|                             |           |           |            |             |           |            |       |         |           |             |
| LargeVsSmall                | .NET 10.0 | .NET 10.0 |  0.0000 ns |   0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |         - |           ? |
| LargeVsSmall                | .NET 8.0  | .NET 8.0  |  0.0000 ns |   0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |         - |           ? |
|                             |           |           |            |             |           |            |       |         |           |             |
| ZeroVsNegative              | .NET 10.0 | .NET 10.0 |  0.0000 ns |   0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |         - |           ? |
| ZeroVsNegative              | .NET 8.0  | .NET 8.0  |  0.0000 ns |   0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |         - |           ? |
|                             |           |           |            |             |           |            |       |         |           |             |
| LargeVsLarge                | .NET 10.0 | .NET 10.0 |  0.0037 ns |   0.0925 ns | 0.0051 ns |  0.0016 ns |     ? |       ? |         - |           ? |
| LargeVsLarge                | .NET 8.0  | .NET 8.0  |  0.0000 ns |   0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |         - |           ? |
|                             |           |           |            |             |           |            |       |         |           |             |
| SmallVsLarge_WithCallback   | .NET 10.0 | .NET 10.0 |  0.4334 ns |   0.0192 ns | 0.0011 ns |  0.4332 ns |  0.77 |    0.01 |         - |          NA |
| SmallVsLarge_WithCallback   | .NET 8.0  | .NET 8.0  |  0.5640 ns |   0.1241 ns | 0.0068 ns |  0.5617 ns |  1.00 |    0.01 |         - |          NA |
|                             |           |           |            |             |           |            |       |         |           |             |
| LargeVsSmall_WithCallback   | .NET 10.0 | .NET 10.0 |  0.4523 ns |   0.5693 ns | 0.0312 ns |  0.4396 ns |  0.91 |    0.07 |         - |          NA |
| LargeVsSmall_WithCallback   | .NET 8.0  | .NET 8.0  |  0.4965 ns |   0.4675 ns | 0.0256 ns |  0.4823 ns |  1.00 |    0.06 |         - |          NA |
|                             |           |           |            |             |           |            |       |         |           |             |
| ZeroVsNegative_WithCallback | .NET 10.0 | .NET 10.0 |  0.4526 ns |   0.1402 ns | 0.0077 ns |  0.4492 ns |  0.87 |    0.05 |         - |          NA |
| ZeroVsNegative_WithCallback | .NET 8.0  | .NET 8.0  |  0.5191 ns |   0.5403 ns | 0.0296 ns |  0.5333 ns |  1.00 |    0.07 |         - |          NA |
|                             |           |           |            |             |           |            |       |         |           |             |
| LargeVsLarge_WithCallback   | .NET 10.0 | .NET 10.0 |  0.2872 ns |   3.0359 ns | 0.1664 ns |  0.2086 ns |  0.32 |    0.33 |         - |          NA |
| LargeVsLarge_WithCallback   | .NET 8.0  | .NET 8.0  |  1.4953 ns |  18.1769 ns | 0.9963 ns |  1.6361 ns |  1.65 |    1.80 |         - |          NA |
|                             |           |           |            |             |           |            |       |         |           |             |
| A_vs_B                      | .NET 10.0 | .NET 10.0 | 14.0302 ns |   0.8109 ns | 0.0445 ns | 14.0078 ns |  0.94 |    0.01 |         - |          NA |
| A_vs_B                      | .NET 8.0  | .NET 8.0  | 14.9324 ns |   2.9339 ns | 0.1608 ns | 14.8499 ns |  1.00 |    0.01 |         - |          NA |
|                             |           |           |            |             |           |            |       |         |           |             |
| B_vs_A                      | .NET 10.0 | .NET 10.0 | 14.1004 ns |   1.6714 ns | 0.0916 ns | 14.0609 ns |  1.02 |    0.01 |         - |          NA |
| B_vs_A                      | .NET 8.0  | .NET 8.0  | 13.8702 ns |   3.3676 ns | 0.1846 ns | 13.7791 ns |  1.00 |    0.02 |         - |          NA |
|                             |           |           |            |             |           |            |       |         |           |             |
| Empty_vs_A                  | .NET 10.0 | .NET 10.0 | 15.9208 ns |  50.2508 ns | 2.7544 ns | 14.3513 ns |  1.04 |    0.16 |         - |          NA |
| Empty_vs_A                  | .NET 8.0  | .NET 8.0  | 15.2739 ns |   0.5708 ns | 0.0313 ns | 15.2878 ns |  1.00 |    0.00 |         - |          NA |
|                             |           |           |            |             |           |            |       |         |           |             |
| A_vs_UpperA                 | .NET 10.0 | .NET 10.0 | 28.8238 ns |  47.4160 ns | 2.5990 ns | 28.5709 ns |  1.06 |    0.08 |         - |          NA |
| A_vs_UpperA                 | .NET 8.0  | .NET 8.0  | 27.1415 ns |   1.6168 ns | 0.0886 ns | 27.1314 ns |  1.00 |    0.00 |         - |          NA |
|                             |           |           |            |             |           |            |       |         |           |             |
| A_vs_B_WithCallback         | .NET 10.0 | .NET 10.0 | 15.2855 ns |   4.8525 ns | 0.2660 ns | 15.1536 ns |  0.79 |    0.23 |         - |          NA |
| A_vs_B_WithCallback         | .NET 8.0  | .NET 8.0  | 21.4882 ns | 164.7686 ns | 9.0315 ns | 16.3348 ns |  1.10 |    0.53 |         - |          NA |
|                             |           |           |            |             |           |            |       |         |           |             |
| B_vs_A_WithCallback         | .NET 10.0 | .NET 10.0 | 15.1660 ns |   2.0258 ns | 0.1110 ns | 15.1038 ns |  0.88 |    0.12 |         - |          NA |
| B_vs_A_WithCallback         | .NET 8.0  | .NET 8.0  | 17.5588 ns |  55.8883 ns | 3.0634 ns | 15.7982 ns |  1.02 |    0.21 |         - |          NA |
|                             |           |           |            |             |           |            |       |         |           |             |
| Empty_vs_A_WithCallback     | .NET 10.0 | .NET 10.0 | 13.8168 ns |   1.4441 ns | 0.0792 ns | 13.7869 ns |  0.86 |    0.00 |         - |          NA |
| Empty_vs_A_WithCallback     | .NET 8.0  | .NET 8.0  | 16.1449 ns |   0.4426 ns | 0.0243 ns | 16.1401 ns |  1.00 |    0.00 |         - |          NA |
|                             |           |           |            |             |           |            |       |         |           |             |
| A_vs_UpperA_WithCallback    | .NET 10.0 | .NET 10.0 | 27.5333 ns |   4.1954 ns | 0.2300 ns | 27.4379 ns |  0.98 |    0.01 |         - |          NA |
| A_vs_UpperA_WithCallback    | .NET 8.0  | .NET 8.0  | 28.0671 ns |   0.4186 ns | 0.0229 ns | 28.0636 ns |  1.00 |    0.00 |         - |          NA |
