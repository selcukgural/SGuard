```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 10.0.105
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 8.0  : .NET 8.0.3 (8.0.3, 8.0.324.11423), Arm64 RyuJIT armv8.0-a

IterationCount=3  LaunchCount=1  WarmupCount=3  

```
| Method                                      | Job       | Runtime   | Mean      | Error      | StdDev    | Median    | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------------------------------- |---------- |---------- |----------:|-----------:|----------:|----------:|------:|--------:|----------:|------------:|
| Between_Int_True                            | .NET 10.0 | .NET 10.0 | 0.2176 ns |  0.1167 ns | 0.0064 ns | 0.2146 ns |  0.79 |    0.02 |         - |          NA |
| Between_Int_True                            | .NET 8.0  | .NET 8.0  | 0.2743 ns |  0.0204 ns | 0.0011 ns | 0.2738 ns |  1.00 |    0.00 |         - |          NA |
|                                             |           |           |           |            |           |           |       |         |           |             |
| Between_Int_False_Low                       | .NET 10.0 | .NET 10.0 | 0.6852 ns |  8.1126 ns | 0.4447 ns | 0.6966 ns |  2.56 |    1.44 |         - |          NA |
| Between_Int_False_Low                       | .NET 8.0  | .NET 8.0  | 0.2678 ns |  0.1242 ns | 0.0068 ns | 0.2648 ns |  1.00 |    0.03 |         - |          NA |
|                                             |           |           |           |            |           |           |       |         |           |             |
| Between_Int_False_High                      | .NET 10.0 | .NET 10.0 | 0.2268 ns |  0.0458 ns | 0.0025 ns | 0.2254 ns |  0.78 |    0.01 |         - |          NA |
| Between_Int_False_High                      | .NET 8.0  | .NET 8.0  | 0.2920 ns |  0.0571 ns | 0.0031 ns | 0.2906 ns |  1.00 |    0.01 |         - |          NA |
|                                             |           |           |           |            |           |           |       |         |           |             |
| Between_Int_True_WithCallback               | .NET 10.0 | .NET 10.0 | 0.8215 ns |  2.9906 ns | 0.1639 ns | 0.7269 ns |  1.14 |    0.20 |         - |          NA |
| Between_Int_True_WithCallback               | .NET 8.0  | .NET 8.0  | 0.7179 ns |  0.0346 ns | 0.0019 ns | 0.7184 ns |  1.00 |    0.00 |         - |          NA |
|                                             |           |           |           |            |           |           |       |         |           |             |
| Between_Int_False_Low_WithCallback          | .NET 10.0 | .NET 10.0 | 0.4550 ns |  0.0915 ns | 0.0050 ns | 0.4527 ns |  1.02 |    0.01 |         - |          NA |
| Between_Int_False_Low_WithCallback          | .NET 8.0  | .NET 8.0  | 0.4443 ns |  0.0523 ns | 0.0029 ns | 0.4432 ns |  1.00 |    0.01 |         - |          NA |
|                                             |           |           |           |            |           |           |       |         |           |             |
| Between_Int_False_High_WithCallback         | .NET 10.0 | .NET 10.0 | 1.6730 ns | 21.6541 ns | 1.1869 ns | 0.9946 ns |  1.31 |    0.81 |         - |          NA |
| Between_Int_False_High_WithCallback         | .NET 8.0  | .NET 8.0  | 1.2728 ns |  0.1524 ns | 0.0084 ns | 1.2713 ns |  1.00 |    0.01 |         - |          NA |
|                                             |           |           |           |            |           |           |       |         |           |             |
| Between_String_True                         | .NET 10.0 | .NET 10.0 | 0.9900 ns |  0.1751 ns | 0.0096 ns | 0.9945 ns |  0.35 |    0.00 |         - |          NA |
| Between_String_True                         | .NET 8.0  | .NET 8.0  | 2.8205 ns |  0.3039 ns | 0.0167 ns | 2.8181 ns |  1.00 |    0.01 |         - |          NA |
|                                             |           |           |           |            |           |           |       |         |           |             |
| Between_String_False_Low                    | .NET 10.0 | .NET 10.0 | 0.7077 ns |  0.0576 ns | 0.0032 ns | 0.7068 ns |  0.44 |    0.00 |         - |          NA |
| Between_String_False_Low                    | .NET 8.0  | .NET 8.0  | 1.5969 ns |  0.0502 ns | 0.0028 ns | 1.5967 ns |  1.00 |    0.00 |         - |          NA |
|                                             |           |           |           |            |           |           |       |         |           |             |
| Between_String_False_High                   | .NET 10.0 | .NET 10.0 | 1.7880 ns |  0.1736 ns | 0.0095 ns | 1.7842 ns |  0.46 |    0.00 |         - |          NA |
| Between_String_False_High                   | .NET 8.0  | .NET 8.0  | 3.8461 ns |  0.0253 ns | 0.0014 ns | 3.8466 ns |  1.00 |    0.00 |         - |          NA |
|                                             |           |           |           |            |           |           |       |         |           |             |
| Between_String_True_WithCallback            | .NET 10.0 | .NET 10.0 | 2.0594 ns |  7.9388 ns | 0.4352 ns | 1.8131 ns |  0.37 |    0.18 |         - |          NA |
| Between_String_True_WithCallback            | .NET 8.0  | .NET 8.0  | 6.5671 ns | 48.5536 ns | 2.6614 ns | 7.9112 ns |  1.17 |    0.69 |         - |          NA |
|                                             |           |           |           |            |           |           |       |         |           |             |
| Between_String_False_Low_WithCallback       | .NET 10.0 | .NET 10.0 | 1.2513 ns |  0.1882 ns | 0.0103 ns | 1.2501 ns |  0.32 |    0.13 |         - |          NA |
| Between_String_False_Low_WithCallback       | .NET 8.0  | .NET 8.0  | 4.5047 ns | 36.9441 ns | 2.0250 ns | 4.3075 ns |  1.16 |    0.67 |         - |          NA |
|                                             |           |           |           |            |           |           |       |         |           |             |
| Between_String_False_High_WithCallback      | .NET 10.0 | .NET 10.0 | 2.3444 ns |  0.3143 ns | 0.0172 ns | 2.3395 ns |  0.51 |    0.02 |         - |          NA |
| Between_String_False_High_WithCallback      | .NET 8.0  | .NET 8.0  | 4.5974 ns |  3.7527 ns | 0.2057 ns | 4.4866 ns |  1.00 |    0.05 |         - |          NA |
|                                             |           |           |           |            |           |           |       |         |           |             |
| Between_Int_Edge_1000000_True               | .NET 10.0 | .NET 10.0 | 0.0075 ns |  0.2295 ns | 0.0126 ns | 0.0005 ns |     ? |       ? |         - |           ? |
| Between_Int_Edge_1000000_True               | .NET 8.0  | .NET 8.0  | 0.0000 ns |  0.0000 ns | 0.0000 ns | 0.0000 ns |     ? |       ? |         - |           ? |
|                                             |           |           |           |            |           |           |       |         |           |             |
| Between_Int_Edge_1000000_False              | .NET 10.0 | .NET 10.0 | 0.1343 ns |  4.2445 ns | 0.2327 ns | 0.0000 ns |     ? |       ? |         - |           ? |
| Between_Int_Edge_1000000_False              | .NET 8.0  | .NET 8.0  | 0.0385 ns |  1.2176 ns | 0.0667 ns | 0.0000 ns |     ? |       ? |         - |           ? |
|                                             |           |           |           |            |           |           |       |         |           |             |
| Between_Int_Edge_1000000_True_WithCallback  | .NET 10.0 | .NET 10.0 | 0.1943 ns |  0.0504 ns | 0.0028 ns | 0.1938 ns |  0.54 |    0.44 |         - |          NA |
| Between_Int_Edge_1000000_True_WithCallback  | .NET 8.0  | .NET 8.0  | 0.8794 ns | 17.9131 ns | 0.9819 ns | 0.4604 ns |  2.44 |    3.60 |         - |          NA |
|                                             |           |           |           |            |           |           |       |         |           |             |
| Between_Int_Edge_1000000_False_WithCallback | .NET 10.0 | .NET 10.0 | 0.1903 ns |  0.2591 ns | 0.0142 ns | 0.1914 ns |  0.83 |    0.37 |         - |          NA |
| Between_Int_Edge_1000000_False_WithCallback | .NET 8.0  | .NET 8.0  | 0.3004 ns |  3.9037 ns | 0.2140 ns | 0.1808 ns |  1.32 |    1.05 |         - |          NA |
