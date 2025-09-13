```

BenchmarkDotNet v0.15.2, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD


```
| Method                      | Mean        | Error      | StdDev     |
|---------------------------- |------------:|-----------:|-----------:|
| SmallVsLarge                |    10.54 ns |   0.223 ns |   0.219 ns |
| LargeVsSmall                | 8,642.42 ns | 125.756 ns | 117.632 ns |
| ZeroVsNegative              | 8,549.97 ns |  74.938 ns |  70.097 ns |
| LargeVsLarge                |    10.51 ns |   0.205 ns |   0.191 ns |
| SmallVsLarge_WithCallback   |    11.04 ns |   0.233 ns |   0.240 ns |
| LargeVsSmall_WithCallback   | 8,549.13 ns | 109.705 ns | 102.619 ns |
| ZeroVsNegative_WithCallback | 8,681.57 ns |  42.599 ns |  39.847 ns |
| LargeVsLarge_WithCallback   |    10.67 ns |   0.013 ns |   0.010 ns |
| A_vs_B                      |    29.25 ns |   0.327 ns |   0.305 ns |
| B_vs_A                      | 8,607.55 ns |  74.998 ns |  66.484 ns |
| Empty_vs_A                  |    28.83 ns |   0.087 ns |   0.073 ns |
| A_vs_UpperA                 |    41.97 ns |   0.397 ns |   0.352 ns |
| A_vs_B_WithCallback         |    29.30 ns |   0.195 ns |   0.173 ns |
| B_vs_A_WithCallback         | 8,646.46 ns |  82.311 ns |  76.993 ns |
| Empty_vs_A_WithCallback     |    29.80 ns |   0.615 ns |   0.708 ns |
| A_vs_UpperA_WithCallback    |    42.48 ns |   0.196 ns |   0.183 ns |
