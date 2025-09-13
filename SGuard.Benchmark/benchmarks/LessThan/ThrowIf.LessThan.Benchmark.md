```

BenchmarkDotNet v0.15.2, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD


```
| Method                      | Mean        | Error      | StdDev     |
|---------------------------- |------------:|-----------:|-----------:|
| SmallVsLarge                | 8,608.43 ns |  69.840 ns |  61.911 ns |
| LargeVsSmall                |    10.03 ns |   0.031 ns |   0.026 ns |
| ZeroVsNegative              |    10.03 ns |   0.030 ns |   0.027 ns |
| LargeVsLarge                |    10.33 ns |   0.128 ns |   0.113 ns |
| SmallVsLarge_WithCallback   | 8,601.90 ns | 120.955 ns | 113.141 ns |
| LargeVsSmall_WithCallback   |    10.72 ns |   0.235 ns |   0.242 ns |
| ZeroVsNegative_WithCallback |    10.49 ns |   0.061 ns |   0.051 ns |
| LargeVsLarge_WithCallback   |    10.67 ns |   0.046 ns |   0.043 ns |
| A_vs_B                      | 8,672.10 ns |  63.502 ns |  59.400 ns |
| B_vs_A                      |    28.53 ns |   0.135 ns |   0.120 ns |
| Empty_vs_A                  | 8,833.43 ns |  51.845 ns |  48.496 ns |
| A_vs_UpperA                 | 8,555.66 ns |  45.332 ns |  42.404 ns |
| A_vs_B_WithCallback         | 8,490.76 ns |  65.440 ns |  58.011 ns |
| B_vs_A_WithCallback         |    28.96 ns |   0.088 ns |   0.069 ns |
| Empty_vs_A_WithCallback     | 8,814.10 ns |  65.243 ns |  61.028 ns |
| A_vs_UpperA_WithCallback    | 8,567.66 ns |  63.024 ns |  52.627 ns |
