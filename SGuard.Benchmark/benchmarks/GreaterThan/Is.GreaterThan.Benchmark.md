```

BenchmarkDotNet v0.15.2, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD


```
| Method                      | Mean       | Error     | StdDev    |
|---------------------------- |-----------:|----------:|----------:|
| SmallVsLarge                |  0.4783 ns | 0.0384 ns | 0.0359 ns |
| LargeVsSmall                |  0.4666 ns | 0.0168 ns | 0.0157 ns |
| ZeroVsNegative              |  0.4722 ns | 0.0167 ns | 0.0156 ns |
| LargeVsLarge                |  0.2099 ns | 0.0102 ns | 0.0096 ns |
| SmallVsLarge_WithCallback   |  0.4666 ns | 0.0047 ns | 0.0039 ns |
| LargeVsSmall_WithCallback   |  0.4905 ns | 0.0171 ns | 0.0152 ns |
| ZeroVsNegative_WithCallback |  0.4581 ns | 0.0149 ns | 0.0132 ns |
| LargeVsLarge_WithCallback   |  0.2111 ns | 0.0129 ns | 0.0121 ns |
| A_vs_B                      | 15.4065 ns | 0.2467 ns | 0.2187 ns |
| B_vs_A                      | 15.2288 ns | 0.0496 ns | 0.0414 ns |
| Empty_vs_A                  | 15.8033 ns | 0.1141 ns | 0.1011 ns |
| A_vs_UpperA                 | 27.9542 ns | 0.1172 ns | 0.1097 ns |
| A_vs_B_WithCallback         | 15.5059 ns | 0.0679 ns | 0.0602 ns |
| B_vs_A_WithCallback         | 15.5444 ns | 0.1189 ns | 0.1113 ns |
| Empty_vs_A_WithCallback     | 16.0188 ns | 0.0135 ns | 0.0105 ns |
| A_vs_UpperA_WithCallback    | 29.0064 ns | 0.1116 ns | 0.1044 ns |
