```

BenchmarkDotNet v0.15.2, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD


```
| Method                      | Mean       | Error     | StdDev    |
|---------------------------- |-----------:|----------:|----------:|
| SmallVsLarge                |  0.4716 ns | 0.0019 ns | 0.0015 ns |
| LargeVsSmall                |  0.4690 ns | 0.0011 ns | 0.0008 ns |
| ZeroVsNegative              |  0.4683 ns | 0.0016 ns | 0.0014 ns |
| LargeVsLarge                |  0.1989 ns | 0.0012 ns | 0.0009 ns |
| SmallVsLarge_WithCallback   |  0.4612 ns | 0.0010 ns | 0.0009 ns |
| LargeVsSmall_WithCallback   |  0.4611 ns | 0.0015 ns | 0.0013 ns |
| ZeroVsNegative_WithCallback |  0.4611 ns | 0.0022 ns | 0.0019 ns |
| LargeVsLarge_WithCallback   |  0.1902 ns | 0.0007 ns | 0.0006 ns |
| A_vs_B                      | 15.2624 ns | 0.0655 ns | 0.0613 ns |
| B_vs_A                      | 15.2270 ns | 0.0120 ns | 0.0094 ns |
| Empty_vs_A                  | 15.7637 ns | 0.0175 ns | 0.0146 ns |
| A_vs_UpperA                 | 28.4242 ns | 0.2482 ns | 0.2200 ns |
| A_vs_B_WithCallback         | 15.7168 ns | 0.2991 ns | 0.2798 ns |
| B_vs_A_WithCallback         | 15.7967 ns | 0.2562 ns | 0.2397 ns |
| Empty_vs_A_WithCallback     | 16.4865 ns | 0.2800 ns | 0.2619 ns |
| A_vs_UpperA_WithCallback    | 29.2752 ns | 0.2626 ns | 0.2328 ns |
