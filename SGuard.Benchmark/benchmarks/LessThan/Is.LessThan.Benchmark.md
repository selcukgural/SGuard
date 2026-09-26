```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 10.0.105
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 8.0  : .NET 8.0.3 (8.0.3, 8.0.324.11423), Arm64 RyuJIT armv8.0-a

IterationCount=3  LaunchCount=1  WarmupCount=3  

```
| Method                      | Job       | Runtime   | Mean       | Error      | StdDev    | Median     | Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------- |---------- |---------- |-----------:|-----------:|----------:|-----------:|------:|--------:|----------:|------------:|
| SmallVsLarge                | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |         - |           ? |
| SmallVsLarge                | .NET 8.0  | .NET 8.0  |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |         - |           ? |
|                             |           |           |            |            |           |            |       |         |           |             |
| LargeVsSmall                | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |         - |           ? |
| LargeVsSmall                | .NET 8.0  | .NET 8.0  |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |         - |           ? |
|                             |           |           |            |            |           |            |       |         |           |             |
| ZeroVsNegative              | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |         - |           ? |
| ZeroVsNegative              | .NET 8.0  | .NET 8.0  |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |         - |           ? |
|                             |           |           |            |            |           |            |       |         |           |             |
| LargeVsLarge                | .NET 10.0 | .NET 10.0 |  0.0104 ns |  0.1601 ns | 0.0088 ns |  0.0054 ns |     ? |       ? |         - |           ? |
| LargeVsLarge                | .NET 8.0  | .NET 8.0  |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |         - |           ? |
|                             |           |           |            |            |           |            |       |         |           |             |
| SmallVsLarge_WithCallback   | .NET 10.0 | .NET 10.0 |  0.4446 ns |  0.0164 ns | 0.0009 ns |  0.4448 ns |  0.85 |    0.07 |         - |          NA |
| SmallVsLarge_WithCallback   | .NET 8.0  | .NET 8.0  |  0.5256 ns |  0.8131 ns | 0.0446 ns |  0.5436 ns |  1.01 |    0.11 |         - |          NA |
|                             |           |           |            |            |           |            |       |         |           |             |
| LargeVsSmall_WithCallback   | .NET 10.0 | .NET 10.0 |  0.6926 ns |  6.3846 ns | 0.3500 ns |  0.5119 ns |  1.51 |    0.66 |         - |          NA |
| LargeVsSmall_WithCallback   | .NET 8.0  | .NET 8.0  |  0.4594 ns |  0.1768 ns | 0.0097 ns |  0.4581 ns |  1.00 |    0.03 |         - |          NA |
|                             |           |           |            |            |           |            |       |         |           |             |
| ZeroVsNegative_WithCallback | .NET 10.0 | .NET 10.0 |  0.4583 ns |  0.0076 ns | 0.0004 ns |  0.4584 ns |  0.94 |    0.03 |         - |          NA |
| ZeroVsNegative_WithCallback | .NET 8.0  | .NET 8.0  |  0.4870 ns |  0.2787 ns | 0.0153 ns |  0.4809 ns |  1.00 |    0.04 |         - |          NA |
|                             |           |           |            |            |           |            |       |         |           |             |
| LargeVsLarge_WithCallback   | .NET 10.0 | .NET 10.0 |  0.2951 ns |  3.1038 ns | 0.1701 ns |  0.2044 ns |  1.61 |    0.80 |         - |          NA |
| LargeVsLarge_WithCallback   | .NET 8.0  | .NET 8.0  |  0.1837 ns |  0.0648 ns | 0.0036 ns |  0.1819 ns |  1.00 |    0.02 |         - |          NA |
|                             |           |           |            |            |           |            |       |         |           |             |
| A_vs_B                      | .NET 10.0 | .NET 10.0 | 14.2680 ns |  3.7480 ns | 0.2054 ns | 14.3552 ns |  0.96 |    0.02 |         - |          NA |
| A_vs_B                      | .NET 8.0  | .NET 8.0  | 14.9282 ns |  3.4690 ns | 0.1901 ns | 14.8200 ns |  1.00 |    0.02 |         - |          NA |
|                             |           |           |            |            |           |            |       |         |           |             |
| B_vs_A                      | .NET 10.0 | .NET 10.0 | 14.1396 ns |  1.5976 ns | 0.0876 ns | 14.1778 ns |  0.93 |    0.01 |         - |          NA |
| B_vs_A                      | .NET 8.0  | .NET 8.0  | 15.1827 ns |  1.2907 ns | 0.0707 ns | 15.1762 ns |  1.00 |    0.01 |         - |          NA |
|                             |           |           |            |            |           |            |       |         |           |             |
| Empty_vs_A                  | .NET 10.0 | .NET 10.0 | 16.1306 ns | 49.1291 ns | 2.6929 ns | 14.8345 ns |  1.05 |    0.15 |         - |          NA |
| Empty_vs_A                  | .NET 8.0  | .NET 8.0  | 15.3413 ns |  0.5101 ns | 0.0280 ns | 15.3535 ns |  1.00 |    0.00 |         - |          NA |
|                             |           |           |            |            |           |            |       |         |           |             |
| A_vs_UpperA                 | .NET 10.0 | .NET 10.0 | 28.2353 ns | 40.5769 ns | 2.2242 ns | 27.3697 ns |  1.01 |    0.07 |         - |          NA |
| A_vs_UpperA                 | .NET 8.0  | .NET 8.0  | 28.0477 ns |  4.1496 ns | 0.2275 ns | 28.0731 ns |  1.00 |    0.01 |         - |          NA |
|                             |           |           |            |            |           |            |       |         |           |             |
| A_vs_B_WithCallback         | .NET 10.0 | .NET 10.0 | 15.1141 ns |  0.4406 ns | 0.0241 ns | 15.1002 ns |  0.90 |    0.10 |         - |          NA |
| A_vs_B_WithCallback         | .NET 8.0  | .NET 8.0  | 17.0920 ns | 44.8355 ns | 2.4576 ns | 15.6889 ns |  1.01 |    0.17 |         - |          NA |
|                             |           |           |            |            |           |            |       |         |           |             |
| B_vs_A_WithCallback         | .NET 10.0 | .NET 10.0 | 15.1678 ns |  1.7500 ns | 0.0959 ns | 15.1246 ns |  0.92 |    0.01 |         - |          NA |
| B_vs_A_WithCallback         | .NET 8.0  | .NET 8.0  | 16.4352 ns |  2.5953 ns | 0.1423 ns | 16.3763 ns |  1.00 |    0.01 |         - |          NA |
|                             |           |           |            |            |           |            |       |         |           |             |
| Empty_vs_A_WithCallback     | .NET 10.0 | .NET 10.0 | 16.7224 ns | 50.9949 ns | 2.7952 ns | 15.1260 ns |  1.05 |    0.15 |         - |          NA |
| Empty_vs_A_WithCallback     | .NET 8.0  | .NET 8.0  | 15.9637 ns |  1.9621 ns | 0.1075 ns | 15.9077 ns |  1.00 |    0.01 |         - |          NA |
|                             |           |           |            |            |           |            |       |         |           |             |
| A_vs_UpperA_WithCallback    | .NET 10.0 | .NET 10.0 | 29.4069 ns | 52.5334 ns | 2.8795 ns | 27.7842 ns |  1.01 |    0.09 |         - |          NA |
| A_vs_UpperA_WithCallback    | .NET 8.0  | .NET 8.0  | 29.0418 ns |  9.3874 ns | 0.5146 ns | 28.7739 ns |  1.00 |    0.02 |         - |          NA |
