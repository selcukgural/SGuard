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
| SmallVsLarge                | .NET 10.0 | .NET 10.0 |      0.6902 ns |     0.3683 ns |   0.0202 ns |  1.12 |    0.03 |      - |         - |          NA |
| SmallVsLarge                | .NET 8.0  | .NET 8.0  |      0.6188 ns |     0.0948 ns |   0.0052 ns |  1.00 |    0.01 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| LargeVsSmall                | .NET 10.0 | .NET 10.0 |  2,057.5365 ns |   420.0776 ns |  23.0259 ns |  0.16 |    0.00 | 0.0725 |     616 B |        0.99 |
| LargeVsSmall                | .NET 8.0  | .NET 8.0  | 12,615.3779 ns | 2,643.5260 ns | 144.9006 ns |  1.00 |    0.01 | 0.0610 |     624 B |        1.00 |
|                             |           |           |                |               |             |       |         |        |           |             |
| ZeroVsNegative              | .NET 10.0 | .NET 10.0 |  2,074.1966 ns |   630.8174 ns |  34.5772 ns |  0.16 |    0.00 | 0.0725 |     616 B |        0.99 |
| ZeroVsNegative              | .NET 8.0  | .NET 8.0  | 12,705.0904 ns |   644.7209 ns |  35.3393 ns |  1.00 |    0.00 | 0.0610 |     624 B |        1.00 |
|                             |           |           |                |               |             |       |         |        |           |             |
| LargeVsLarge                | .NET 10.0 | .NET 10.0 |      0.6578 ns |     0.3526 ns |   0.0193 ns |  1.12 |    0.03 |      - |         - |          NA |
| LargeVsLarge                | .NET 8.0  | .NET 8.0  |      0.5871 ns |     0.1050 ns |   0.0058 ns |  1.00 |    0.01 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| SmallVsLarge_WithCallback   | .NET 10.0 | .NET 10.0 |      1.3383 ns |     0.1647 ns |   0.0090 ns |  1.02 |    0.01 |      - |         - |          NA |
| SmallVsLarge_WithCallback   | .NET 8.0  | .NET 8.0  |      1.3110 ns |     0.1079 ns |   0.0059 ns |  1.00 |    0.01 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| LargeVsSmall_WithCallback   | .NET 10.0 | .NET 10.0 |  2,127.9237 ns |   270.9701 ns |  14.8528 ns |  0.16 |    0.00 | 0.0725 |     616 B |        0.99 |
| LargeVsSmall_WithCallback   | .NET 8.0  | .NET 8.0  | 13,364.7105 ns | 1,569.4803 ns |  86.0285 ns |  1.00 |    0.01 | 0.0610 |     624 B |        1.00 |
|                             |           |           |                |               |             |       |         |        |           |             |
| ZeroVsNegative_WithCallback | .NET 10.0 | .NET 10.0 |  2,118.0402 ns |   423.6920 ns |  23.2240 ns |  0.17 |    0.00 | 0.0725 |     616 B |        0.99 |
| ZeroVsNegative_WithCallback | .NET 8.0  | .NET 8.0  | 12,761.0825 ns | 1,391.3868 ns |  76.2666 ns |  1.00 |    0.01 | 0.0610 |     624 B |        1.00 |
|                             |           |           |                |               |             |       |         |        |           |             |
| LargeVsLarge_WithCallback   | .NET 10.0 | .NET 10.0 |      1.3313 ns |     0.0776 ns |   0.0043 ns |  1.02 |    0.00 |      - |         - |          NA |
| LargeVsLarge_WithCallback   | .NET 8.0  | .NET 8.0  |      1.3015 ns |     0.0608 ns |   0.0033 ns |  1.00 |    0.00 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| A_vs_B                      | .NET 10.0 | .NET 10.0 |     17.8883 ns |     3.8348 ns |   0.2102 ns |  0.68 |    0.01 |      - |         - |          NA |
| A_vs_B                      | .NET 8.0  | .NET 8.0  |     26.1286 ns |     6.9920 ns |   0.3833 ns |  1.00 |    0.02 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| B_vs_A                      | .NET 10.0 | .NET 10.0 |  2,076.7443 ns |   444.1820 ns |  24.3471 ns |  0.16 |    0.00 | 0.0648 |     568 B |        0.82 |
| B_vs_A                      | .NET 8.0  | .NET 8.0  | 13,334.7802 ns | 1,044.4711 ns |  57.2510 ns |  1.00 |    0.01 | 0.0763 |     696 B |        1.00 |
|                             |           |           |                |               |             |       |         |        |           |             |
| Empty_vs_A                  | .NET 10.0 | .NET 10.0 |     18.1164 ns |     0.4769 ns |   0.0261 ns |  0.68 |    0.00 |      - |         - |          NA |
| Empty_vs_A                  | .NET 8.0  | .NET 8.0  |     26.7261 ns |     1.8376 ns |   0.1007 ns |  1.00 |    0.00 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| A_vs_UpperA                 | .NET 10.0 | .NET 10.0 |     29.4698 ns |     0.8598 ns |   0.0471 ns |  0.78 |    0.00 |      - |         - |          NA |
| A_vs_UpperA                 | .NET 8.0  | .NET 8.0  |     37.8585 ns |     1.7592 ns |   0.0964 ns |  1.00 |    0.00 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| A_vs_B_WithCallback         | .NET 10.0 | .NET 10.0 |     18.6427 ns |     0.4643 ns |   0.0254 ns |  0.70 |    0.00 |      - |         - |          NA |
| A_vs_B_WithCallback         | .NET 8.0  | .NET 8.0  |     26.7089 ns |     2.7649 ns |   0.1516 ns |  1.00 |    0.01 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| B_vs_A_WithCallback         | .NET 10.0 | .NET 10.0 |  2,098.9211 ns |   123.1819 ns |   6.7520 ns |  0.16 |    0.00 | 0.0648 |     568 B |        0.82 |
| B_vs_A_WithCallback         | .NET 8.0  | .NET 8.0  | 13,354.7259 ns | 5,307.6546 ns | 290.9305 ns |  1.00 |    0.03 | 0.0763 |     696 B |        1.00 |
|                             |           |           |                |               |             |       |         |        |           |             |
| Empty_vs_A_WithCallback     | .NET 10.0 | .NET 10.0 |     18.9803 ns |     1.4216 ns |   0.0779 ns |  0.71 |    0.00 |      - |         - |          NA |
| Empty_vs_A_WithCallback     | .NET 8.0  | .NET 8.0  |     26.8881 ns |     0.7670 ns |   0.0420 ns |  1.00 |    0.00 |      - |         - |          NA |
|                             |           |           |                |               |             |       |         |        |           |             |
| A_vs_UpperA_WithCallback    | .NET 10.0 | .NET 10.0 |     30.3341 ns |     7.0861 ns |   0.3884 ns |  0.79 |    0.01 |      - |         - |          NA |
| A_vs_UpperA_WithCallback    | .NET 8.0  | .NET 8.0  |     38.4605 ns |     4.6528 ns |   0.2550 ns |  1.00 |    0.01 |      - |         - |          NA |
