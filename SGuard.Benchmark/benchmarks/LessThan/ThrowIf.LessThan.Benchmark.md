```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 10.0.105
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 8.0  : .NET 8.0.3 (8.0.3, 8.0.324.11423), Arm64 RyuJIT armv8.0-a

IterationCount=3  LaunchCount=1  WarmupCount=3  

```
| Method                      | Job       | Runtime   | Mean           | Error         | StdDev      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------------------------- |---------- |---------- |---------------:|--------------:|------------:|------:|--------:|-------:|----------:|------------:|
| SmallVsLarge                | .NET 10.0 | .NET 10.0 |  2,099.3393 ns |    94.9788 ns |   5.2061 ns |  0.16 |    0.00 | 0.0725 |     608 B |        0.99 |
| SmallVsLarge                | .NET 8.0  | .NET 8.0  | 12,814.1604 ns | 1,994.8740 ns | 109.3458 ns |  1.00 |    0.01 | 0.0610 |     616 B |        1.00 |
|                             |           |           |                |               |             |       |         |        |           |             |
| LargeVsSmall                | .NET 10.0 | .NET 10.0 |      0.6333 ns |     0.1224 ns |   0.0067 ns |  0.91 |    0.01 |      - |         - |          NA |
| LargeVsSmall                | .NET 8.0  | .NET 8.0  |      0.6939 ns |     0.0555 ns |   0.0030 ns |  1.00 |    0.01 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| ZeroVsNegative              | .NET 10.0 | .NET 10.0 |      0.6632 ns |     0.0802 ns |   0.0044 ns |  0.95 |    0.01 |      - |         - |          NA |
| ZeroVsNegative              | .NET 8.0  | .NET 8.0  |      0.6978 ns |     0.1057 ns |   0.0058 ns |  1.00 |    0.01 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| LargeVsLarge                | .NET 10.0 | .NET 10.0 |      0.6650 ns |     0.1379 ns |   0.0076 ns |  1.06 |    0.01 |      - |         - |          NA |
| LargeVsLarge                | .NET 8.0  | .NET 8.0  |      0.6269 ns |     0.1325 ns |   0.0073 ns |  1.00 |    0.01 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| SmallVsLarge_WithCallback   | .NET 10.0 | .NET 10.0 |  2,112.1693 ns |    44.0209 ns |   2.4129 ns |  0.17 |    0.00 | 0.0725 |     608 B |        0.99 |
| SmallVsLarge_WithCallback   | .NET 8.0  | .NET 8.0  | 12,745.3319 ns | 1,829.4338 ns | 100.2774 ns |  1.00 |    0.01 | 0.0610 |     616 B |        1.00 |
|                             |           |           |                |               |             |       |         |        |           |             |
| LargeVsSmall_WithCallback   | .NET 10.0 | .NET 10.0 |      1.3451 ns |     0.2711 ns |   0.0149 ns |  1.01 |    0.01 |      - |         - |          NA |
| LargeVsSmall_WithCallback   | .NET 8.0  | .NET 8.0  |      1.3327 ns |     0.0838 ns |   0.0046 ns |  1.00 |    0.00 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| ZeroVsNegative_WithCallback | .NET 10.0 | .NET 10.0 |      1.3333 ns |     0.0419 ns |   0.0023 ns |  1.00 |    0.00 |      - |         - |          NA |
| ZeroVsNegative_WithCallback | .NET 8.0  | .NET 8.0  |      1.3271 ns |     0.0683 ns |   0.0037 ns |  1.00 |    0.00 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| LargeVsLarge_WithCallback   | .NET 10.0 | .NET 10.0 |      1.3424 ns |     0.2376 ns |   0.0130 ns |  1.01 |    0.01 |      - |         - |          NA |
| LargeVsLarge_WithCallback   | .NET 8.0  | .NET 8.0  |      1.3323 ns |     0.2302 ns |   0.0126 ns |  1.00 |    0.01 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| A_vs_B                      | .NET 10.0 | .NET 10.0 |  2,150.1954 ns |   790.5675 ns |  43.3337 ns |  0.16 |    0.00 | 0.0648 |     560 B |        0.81 |
| A_vs_B                      | .NET 8.0  | .NET 8.0  | 13,200.6094 ns |   983.9497 ns |  53.9336 ns |  1.00 |    0.01 | 0.0763 |     688 B |        1.00 |
|                             |           |           |                |               |             |       |         |        |           |             |
| B_vs_A                      | .NET 10.0 | .NET 10.0 |     17.6176 ns |     0.2733 ns |   0.0150 ns |  0.68 |    0.00 |      - |         - |          NA |
| B_vs_A                      | .NET 8.0  | .NET 8.0  |     25.8803 ns |     1.6678 ns |   0.0914 ns |  1.00 |    0.00 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| Empty_vs_A                  | .NET 10.0 | .NET 10.0 |  2,060.6510 ns |    78.4997 ns |   4.3028 ns |  0.16 |    0.00 | 0.0648 |     560 B |        0.81 |
| Empty_vs_A                  | .NET 8.0  | .NET 8.0  | 13,201.6769 ns | 1,727.7052 ns |  94.7014 ns |  1.00 |    0.01 | 0.0763 |     688 B |        1.00 |
|                             |           |           |                |               |             |       |         |        |           |             |
| A_vs_UpperA                 | .NET 10.0 | .NET 10.0 |  2,058.4644 ns |   171.1723 ns |   9.3825 ns |  0.16 |    0.00 | 0.0648 |     560 B |        0.81 |
| A_vs_UpperA                 | .NET 8.0  | .NET 8.0  | 13,115.1411 ns | 1,276.8310 ns |  69.9874 ns |  1.00 |    0.01 | 0.0763 |     688 B |        1.00 |
|                             |           |           |                |               |             |       |         |        |           |             |
| A_vs_B_WithCallback         | .NET 10.0 | .NET 10.0 |  2,059.5963 ns |   110.6626 ns |   6.0658 ns |  0.15 |    0.00 | 0.0648 |     560 B |        0.81 |
| A_vs_B_WithCallback         | .NET 8.0  | .NET 8.0  | 13,296.6542 ns | 4,550.6325 ns | 249.4355 ns |  1.00 |    0.02 | 0.0763 |     688 B |        1.00 |
|                             |           |           |                |               |             |       |         |        |           |             |
| B_vs_A_WithCallback         | .NET 10.0 | .NET 10.0 |     18.6790 ns |     0.6938 ns |   0.0380 ns |  0.70 |    0.00 |      - |         - |          NA |
| B_vs_A_WithCallback         | .NET 8.0  | .NET 8.0  |     26.5299 ns |     0.6718 ns |   0.0368 ns |  1.00 |    0.00 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| Empty_vs_A_WithCallback     | .NET 10.0 | .NET 10.0 |  2,099.9297 ns |    54.1199 ns |   2.9665 ns |  0.16 |    0.00 | 0.0648 |     560 B |        0.81 |
| Empty_vs_A_WithCallback     | .NET 8.0  | .NET 8.0  | 13,281.4946 ns | 3,272.4215 ns | 179.3725 ns |  1.00 |    0.02 | 0.0763 |     688 B |        1.00 |
|                             |           |           |                |               |             |       |         |        |           |             |
| A_vs_UpperA_WithCallback    | .NET 10.0 | .NET 10.0 |  2,057.1953 ns |   499.5709 ns |  27.3832 ns |  0.15 |    0.00 | 0.0648 |     560 B |        0.81 |
| A_vs_UpperA_WithCallback    | .NET 8.0  | .NET 8.0  | 13,445.3471 ns | 2,250.7522 ns | 123.3713 ns |  1.00 |    0.01 | 0.0763 |     688 B |        1.00 |
