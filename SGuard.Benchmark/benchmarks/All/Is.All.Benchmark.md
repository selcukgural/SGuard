```

BenchmarkDotNet v0.15.2, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD


```
| Method                       | Mean         | Error      | StdDev     |
|----------------------------- |-------------:|-----------:|-----------:|
| All_True_NoCallback          |     3.001 ns |  0.0233 ns |  0.0217 ns |
| All_True_WithCallback        |     3.029 ns |  0.0314 ns |  0.0294 ns |
| All_False_NoCallback         |     3.296 ns |  0.0693 ns |  0.0614 ns |
| All_False_WithCallback       |     3.610 ns |  0.0968 ns |  0.1325 ns |
| All_True_1000                |   281.459 ns |  1.0918 ns |  0.9678 ns |
| All_True_5000                | 1,358.419 ns | 10.4591 ns |  9.7835 ns |
| All_True_10000               | 2,705.775 ns | 18.7755 ns | 17.5626 ns |
| All_True_15000               | 4,052.994 ns | 29.9851 ns | 28.0481 ns |
| All_False_1000               |   281.367 ns |  0.9584 ns |  0.8965 ns |
| All_False_5000               | 1,350.677 ns |  3.6094 ns |  3.3763 ns |
| All_False_10000              | 2,686.624 ns |  7.4698 ns |  6.9872 ns |
| All_False_15000              | 4,048.739 ns | 26.4279 ns | 23.4276 ns |
| All_True_1000_WithCallback   |   282.682 ns |  0.9363 ns |  0.8300 ns |
| All_True_5000_WithCallback   | 1,351.754 ns |  4.0711 ns |  3.6089 ns |
| All_True_10000_WithCallback  | 2,688.719 ns |  5.6382 ns |  4.7082 ns |
| All_True_15000_WithCallback  | 4,030.090 ns | 10.1919 ns |  8.5107 ns |
| All_False_1000_WithCallback  |   280.881 ns |  0.8627 ns |  0.8070 ns |
| All_False_5000_WithCallback  | 1,348.809 ns |  2.2569 ns |  2.0007 ns |
| All_False_10000_WithCallback | 2,687.645 ns | 11.4427 ns | 10.1437 ns |
| All_False_15000_WithCallback | 4,025.259 ns | 12.4619 ns | 11.0472 ns |
