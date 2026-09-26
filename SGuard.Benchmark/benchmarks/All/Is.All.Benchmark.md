```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 10.0.105
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 8.0  : .NET 8.0.3 (8.0.3, 8.0.324.11423), Arm64 RyuJIT armv8.0-a

IterationCount=3  LaunchCount=1  WarmupCount=3  

```
| Method                       | Job       | Runtime   | Mean         | Error         | StdDev      | Ratio | RatioSD | Allocated | Alloc Ratio |
|----------------------------- |---------- |---------- |-------------:|--------------:|------------:|------:|--------:|----------:|------------:|
| All_True_NoCallback          | .NET 10.0 | .NET 10.0 |     1.795 ns |     0.5342 ns |   0.0293 ns |  0.31 |    0.00 |         - |          NA |
| All_True_NoCallback          | .NET 8.0  | .NET 8.0  |     5.788 ns |     0.1942 ns |   0.0106 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_True_WithCallback        | .NET 10.0 | .NET 10.0 |     2.618 ns |     0.5539 ns |   0.0304 ns |  0.27 |    0.00 |         - |          NA |
| All_True_WithCallback        | .NET 8.0  | .NET 8.0  |     9.800 ns |     0.5155 ns |   0.0283 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_False_NoCallback         | .NET 10.0 | .NET 10.0 |     1.891 ns |     0.2207 ns |   0.0121 ns |  0.25 |    0.03 |         - |          NA |
| All_False_NoCallback         | .NET 8.0  | .NET 8.0  |     7.808 ns |    19.6702 ns |   1.0782 ns |  1.01 |    0.17 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_False_WithCallback       | .NET 10.0 | .NET 10.0 |     2.694 ns |     0.8104 ns |   0.0444 ns |  0.48 |    0.01 |         - |          NA |
| All_False_WithCallback       | .NET 8.0  | .NET 8.0  |     5.651 ns |     2.1250 ns |   0.1165 ns |  1.00 |    0.03 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_True_1000                | .NET 10.0 | .NET 10.0 |   278.139 ns |    23.6560 ns |   1.2967 ns |  0.51 |    0.00 |         - |          NA |
| All_True_1000                | .NET 8.0  | .NET 8.0  |   545.083 ns |    54.8047 ns |   3.0040 ns |  1.00 |    0.01 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_True_5000                | .NET 10.0 | .NET 10.0 | 1,436.571 ns | 2,963.0300 ns | 162.4137 ns |  0.50 |    0.08 |         - |          NA |
| All_True_5000                | .NET 8.0  | .NET 8.0  | 2,935.395 ns | 7,704.6238 ns | 422.3164 ns |  1.01 |    0.17 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_True_10000               | .NET 10.0 | .NET 10.0 | 2,816.185 ns | 4,135.8065 ns | 226.6975 ns |  0.53 |    0.04 |         - |          NA |
| All_True_10000               | .NET 8.0  | .NET 8.0  | 5,327.620 ns |    87.7927 ns |   4.8122 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_True_15000               | .NET 10.0 | .NET 10.0 | 4,015.432 ns |   411.0042 ns |  22.5285 ns |  0.50 |    0.00 |         - |          NA |
| All_True_15000               | .NET 8.0  | .NET 8.0  | 8,016.163 ns |   561.7403 ns |  30.7909 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_False_1000               | .NET 10.0 | .NET 10.0 |   285.480 ns |   229.1330 ns |  12.5596 ns |  0.53 |    0.02 |         - |          NA |
| All_False_1000               | .NET 8.0  | .NET 8.0  |   540.816 ns |    11.8995 ns |   0.6522 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_False_5000               | .NET 10.0 | .NET 10.0 | 1,338.956 ns |    18.2380 ns |   0.9997 ns |  0.50 |    0.00 |         - |          NA |
| All_False_5000               | .NET 8.0  | .NET 8.0  | 2,679.493 ns |   255.6402 ns |  14.0125 ns |  1.00 |    0.01 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_False_10000              | .NET 10.0 | .NET 10.0 | 2,682.139 ns |   104.7842 ns |   5.7436 ns |  0.50 |    0.00 |         - |          NA |
| All_False_10000              | .NET 8.0  | .NET 8.0  | 5,384.231 ns |   773.5386 ns |  42.4003 ns |  1.00 |    0.01 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_False_15000              | .NET 10.0 | .NET 10.0 | 4,096.841 ns |   595.2114 ns |  32.6255 ns |  0.51 |    0.00 |         - |          NA |
| All_False_15000              | .NET 8.0  | .NET 8.0  | 7,992.648 ns |   326.9835 ns |  17.9231 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_True_1000_WithCallback   | .NET 10.0 | .NET 10.0 |   278.700 ns |    22.4197 ns |   1.2289 ns |  0.51 |    0.00 |         - |          NA |
| All_True_1000_WithCallback   | .NET 8.0  | .NET 8.0  |   549.262 ns |    86.8730 ns |   4.7618 ns |  1.00 |    0.01 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_True_5000_WithCallback   | .NET 10.0 | .NET 10.0 | 1,474.870 ns | 3,987.5118 ns | 218.5690 ns |  0.52 |    0.08 |         - |          NA |
| All_True_5000_WithCallback   | .NET 8.0  | .NET 8.0  | 2,876.593 ns | 5,980.3395 ns | 327.8026 ns |  1.01 |    0.14 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_True_10000_WithCallback  | .NET 10.0 | .NET 10.0 | 2,696.045 ns |   349.0584 ns |  19.1331 ns |  0.50 |    0.00 |         - |          NA |
| All_True_10000_WithCallback  | .NET 8.0  | .NET 8.0  | 5,379.770 ns |   531.5114 ns |  29.1339 ns |  1.00 |    0.01 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_True_15000_WithCallback  | .NET 10.0 | .NET 10.0 | 4,037.159 ns |   168.8858 ns |   9.2572 ns |  0.50 |    0.00 |         - |          NA |
| All_True_15000_WithCallback  | .NET 8.0  | .NET 8.0  | 7,994.563 ns |   191.4374 ns |  10.4933 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_False_1000_WithCallback  | .NET 10.0 | .NET 10.0 |   305.175 ns |   803.3133 ns |  44.0323 ns |  0.55 |    0.07 |         - |          NA |
| All_False_1000_WithCallback  | .NET 8.0  | .NET 8.0  |   557.691 ns |   497.4803 ns |  27.2686 ns |  1.00 |    0.06 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_False_5000_WithCallback  | .NET 10.0 | .NET 10.0 | 1,348.324 ns |   179.3681 ns |   9.8318 ns |  0.50 |    0.00 |         - |          NA |
| All_False_5000_WithCallback  | .NET 8.0  | .NET 8.0  | 2,673.725 ns |    29.8559 ns |   1.6365 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_False_10000_WithCallback | .NET 10.0 | .NET 10.0 | 2,705.040 ns |   580.5718 ns |  31.8231 ns |  0.51 |    0.01 |         - |          NA |
| All_False_10000_WithCallback | .NET 8.0  | .NET 8.0  | 5,351.588 ns |   394.3766 ns |  21.6171 ns |  1.00 |    0.00 |         - |          NA |
|                              |           |           |              |               |             |       |         |           |             |
| All_False_15000_WithCallback | .NET 10.0 | .NET 10.0 | 4,023.349 ns |   176.3613 ns |   9.6670 ns |  0.48 |    0.02 |         - |          NA |
| All_False_15000_WithCallback | .NET 8.0  | .NET 8.0  | 8,391.102 ns | 8,159.5920 ns | 447.2548 ns |  1.00 |    0.06 |         - |          NA |
