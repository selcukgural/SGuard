```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 10.0.105
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 8.0  : .NET 8.0.3 (8.0.3, 8.0.324.11423), Arm64 RyuJIT armv8.0-a

IterationCount=3  LaunchCount=1  WarmupCount=3  

```
| Method                      | Job       | Runtime   | Mean       | Error      | StdDev    | Median     | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------------------------- |---------- |---------- |-----------:|-----------:|----------:|-----------:|------:|--------:|-------:|----------:|------------:|
| NullObj                     | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
| NullObj                     | .NET 8.0  | .NET 8.0  |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| EmptyString                 | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| EmptyString                 | .NET 8.0  | .NET 8.0  |  1.7904 ns |  0.1072 ns | 0.0059 ns |  1.7905 ns | 1.000 |    0.00 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| NonEmptyString              | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| NonEmptyString              | .NET 8.0  | .NET 8.0  |  1.8052 ns |  0.0343 ns | 0.0019 ns |  1.8047 ns | 1.000 |    0.00 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| EmptyArray                  | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| EmptyArray                  | .NET 8.0  | .NET 8.0  |  2.5072 ns |  0.5060 ns | 0.0277 ns |  2.5022 ns | 1.000 |    0.01 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| NonEmptyArray               | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| NonEmptyArray               | .NET 8.0  | .NET 8.0  |  2.4740 ns |  1.1412 ns | 0.0626 ns |  2.4415 ns | 1.000 |    0.03 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| EmptyList                   | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| EmptyList                   | .NET 8.0  | .NET 8.0  |  2.5044 ns |  0.4065 ns | 0.0223 ns |  2.5095 ns | 1.000 |    0.01 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| NonEmptyList                | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| NonEmptyList                | .NET 8.0  | .NET 8.0  |  2.5044 ns |  0.3892 ns | 0.0213 ns |  2.4960 ns | 1.000 |    0.01 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| ZeroInt                     | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
| ZeroInt                     | .NET 8.0  | .NET 8.0  |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| NonZeroInt                  | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
| NonZeroInt                  | .NET 8.0  | .NET 8.0  |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| NullObj_WithCallback        | .NET 10.0 | .NET 10.0 |  0.3117 ns |  0.1174 ns | 0.0064 ns |  0.3141 ns |  0.78 |    0.11 |      - |         - |          NA |
| NullObj_WithCallback        | .NET 8.0  | .NET 8.0  |  0.4076 ns |  1.3488 ns | 0.0739 ns |  0.3787 ns |  1.02 |    0.22 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| EmptyString_WithCallback    | .NET 10.0 | .NET 10.0 |  0.4432 ns |  0.0594 ns | 0.0033 ns |  0.4414 ns |  0.17 |    0.01 |      - |         - |          NA |
| EmptyString_WithCallback    | .NET 8.0  | .NET 8.0  |  2.6451 ns |  2.6716 ns | 0.1464 ns |  2.5911 ns |  1.00 |    0.07 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| NonEmptyString_WithCallback | .NET 10.0 | .NET 10.0 |  0.4753 ns |  0.2666 ns | 0.0146 ns |  0.4739 ns |  0.17 |    0.03 |      - |         - |          NA |
| NonEmptyString_WithCallback | .NET 8.0  | .NET 8.0  |  2.8656 ns | 10.1054 ns | 0.5539 ns |  2.6795 ns |  1.02 |    0.24 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| EmptyArray_WithCallback     | .NET 10.0 | .NET 10.0 |  0.7281 ns |  0.0284 ns | 0.0016 ns |  0.7275 ns |  0.22 |    0.01 |      - |         - |          NA |
| EmptyArray_WithCallback     | .NET 8.0  | .NET 8.0  |  3.2882 ns |  2.2635 ns | 0.1241 ns |  3.2341 ns |  1.00 |    0.05 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| NonEmptyArray_WithCallback  | .NET 10.0 | .NET 10.0 |  0.7052 ns |  0.0141 ns | 0.0008 ns |  0.7055 ns |  0.20 |    0.03 |      - |         - |          NA |
| NonEmptyArray_WithCallback  | .NET 8.0  | .NET 8.0  |  3.5732 ns | 11.9798 ns | 0.6567 ns |  3.2509 ns |  1.02 |    0.22 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| EmptyList_WithCallback      | .NET 10.0 | .NET 10.0 |  0.7294 ns |  0.4439 ns | 0.0243 ns |  0.7172 ns |  0.20 |    0.03 |      - |         - |          NA |
| EmptyList_WithCallback      | .NET 8.0  | .NET 8.0  |  3.7662 ns | 10.6846 ns | 0.5857 ns |  3.4887 ns |  1.02 |    0.19 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| NonEmptyList_WithCallback   | .NET 10.0 | .NET 10.0 |  0.7112 ns |  0.0350 ns | 0.0019 ns |  0.7113 ns |  0.21 |    0.00 |      - |         - |          NA |
| NonEmptyList_WithCallback   | .NET 8.0  | .NET 8.0  |  3.4342 ns |  1.3419 ns | 0.0736 ns |  3.4142 ns |  1.00 |    0.03 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| ZeroInt_WithCallback        | .NET 10.0 | .NET 10.0 |  0.1648 ns |  0.0276 ns | 0.0015 ns |  0.1640 ns |  0.34 |    0.02 |      - |         - |          NA |
| ZeroInt_WithCallback        | .NET 8.0  | .NET 8.0  |  0.4864 ns |  0.5849 ns | 0.0321 ns |  0.4757 ns |  1.00 |    0.08 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| NonZeroInt_WithCallback     | .NET 10.0 | .NET 10.0 |  0.1895 ns |  0.4027 ns | 0.0221 ns |  0.1821 ns |  0.26 |    0.03 |      - |         - |          NA |
| NonZeroInt_WithCallback     | .NET 8.0  | .NET 8.0  |  0.7288 ns |  0.2275 ns | 0.0125 ns |  0.7231 ns |  1.00 |    0.02 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| WhitespaceString            | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| WhitespaceString            | .NET 8.0  | .NET 8.0  |  2.1088 ns |  4.7608 ns | 0.2610 ns |  2.1088 ns | 1.010 |    0.15 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| EmptyGuid                   | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
| EmptyGuid                   | .NET 8.0  | .NET 8.0  |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| NonEmptyGuid                | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| NonEmptyGuid                | .NET 8.0  | .NET 8.0  |  0.1101 ns |  0.1144 ns | 0.0063 ns |  0.1074 ns | 1.002 |    0.07 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| EmptyDateTime               | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
| EmptyDateTime               | .NET 8.0  | .NET 8.0  |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| NonEmptyDateTime            | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
| NonEmptyDateTime            | .NET 8.0  | .NET 8.0  |  0.0928 ns |  2.9331 ns | 0.1608 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| EmptyTimeSpan               | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
| EmptyTimeSpan               | .NET 8.0  | .NET 8.0  |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| NonEmptyTimeSpan            | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
| NonEmptyTimeSpan            | .NET 8.0  | .NET 8.0  |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| EmptyDateOnly               | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
| EmptyDateOnly               | .NET 8.0  | .NET 8.0  |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| NonEmptyDateOnly            | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
| NonEmptyDateOnly            | .NET 8.0  | .NET 8.0  |  0.0703 ns |  2.2212 ns | 0.1218 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| EmptyTimeOnly               | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
| EmptyTimeOnly               | .NET 8.0  | .NET 8.0  |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| NonEmptyTimeOnly            | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
| NonEmptyTimeOnly            | .NET 8.0  | .NET 8.0  |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| EmptyDateTimeOffset         | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
| EmptyDateTimeOffset         | .NET 8.0  | .NET 8.0  |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns |     ? |       ? |      - |         - |           ? |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| NonEmptyDateTimeOffset      | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| NonEmptyDateTimeOffset      | .NET 8.0  | .NET 8.0  |  0.6533 ns |  0.1667 ns | 0.0091 ns |  0.6493 ns | 1.000 |    0.02 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| EmptyDict                   | .NET 10.0 | .NET 10.0 |  0.1540 ns |  0.3679 ns | 0.0202 ns |  0.1449 ns |  0.05 |    0.01 |      - |         - |          NA |
| EmptyDict                   | .NET 8.0  | .NET 8.0  |  2.9464 ns | 11.7188 ns | 0.6423 ns |  2.5790 ns |  1.03 |    0.26 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| SingleDict                  | .NET 10.0 | .NET 10.0 |  0.1276 ns |  0.1050 ns | 0.0058 ns |  0.1263 ns |  0.04 |    0.01 |      - |         - |          NA |
| SingleDict                  | .NET 8.0  | .NET 8.0  |  2.9987 ns | 13.8452 ns | 0.7589 ns |  2.5634 ns |  1.04 |    0.30 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| MultiDict                   | .NET 10.0 | .NET 10.0 |  0.1496 ns |  0.4297 ns | 0.0236 ns |  0.1378 ns |  0.06 |    0.01 |      - |         - |          NA |
| MultiDict                   | .NET 8.0  | .NET 8.0  |  2.5609 ns |  0.0527 ns | 0.0029 ns |  2.5597 ns |  1.00 |    0.00 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| EmptyReadOnlyColl           | .NET 10.0 | .NET 10.0 |  0.1791 ns |  0.0533 ns | 0.0029 ns |  0.1791 ns |  0.07 |    0.00 |      - |         - |          NA |
| EmptyReadOnlyColl           | .NET 8.0  | .NET 8.0  |  2.4933 ns |  0.0858 ns | 0.0047 ns |  2.4953 ns |  1.00 |    0.00 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| SingleReadOnlyColl          | .NET 10.0 | .NET 10.0 |  0.1759 ns |  0.0504 ns | 0.0028 ns |  0.1771 ns |  0.06 |    0.01 |      - |         - |          NA |
| SingleReadOnlyColl          | .NET 8.0  | .NET 8.0  |  3.0823 ns | 10.2437 ns | 0.5615 ns |  3.0626 ns |  1.02 |    0.23 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| MultiReadOnlyColl           | .NET 10.0 | .NET 10.0 |  0.1984 ns |  0.3733 ns | 0.0205 ns |  0.2041 ns |  0.07 |    0.01 |      - |         - |          NA |
| MultiReadOnlyColl           | .NET 8.0  | .NET 8.0  |  3.0040 ns | 11.4926 ns | 0.6299 ns |  2.8321 ns |  1.03 |    0.26 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| EmptyEnumerable             | .NET 10.0 | .NET 10.0 |  0.1679 ns |  0.0911 ns | 0.0050 ns |  0.1695 ns |  0.01 |    0.00 |      - |         - |          NA |
| EmptyEnumerable             | .NET 8.0  | .NET 8.0  | 12.7551 ns | 17.7542 ns | 0.9732 ns | 12.7086 ns |  1.00 |    0.09 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| SingleEnumerable            | .NET 10.0 | .NET 10.0 |  0.1783 ns |  0.1214 ns | 0.0067 ns |  0.1779 ns |  0.06 |    0.01 |      - |         - |          NA |
| SingleEnumerable            | .NET 8.0  | .NET 8.0  |  2.8535 ns | 10.8785 ns | 0.5963 ns |  2.5168 ns |  1.03 |    0.25 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| MultiEnumerable             | .NET 10.0 | .NET 10.0 | 10.8817 ns |  0.5010 ns | 0.0275 ns | 10.8690 ns |  0.76 |    0.00 | 0.0048 |      40 B |        1.00 |
| MultiEnumerable             | .NET 8.0  | .NET 8.0  | 14.2944 ns |  0.6191 ns | 0.0339 ns | 14.2837 ns |  1.00 |    0.00 | 0.0048 |      40 B |        1.00 |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| Array1                      | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| Array1                      | .NET 8.0  | .NET 8.0  |  2.4149 ns |  1.1481 ns | 0.0629 ns |  2.4048 ns | 1.000 |    0.03 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| Array10                     | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| Array10                     | .NET 8.0  | .NET 8.0  |  2.5263 ns |  0.1087 ns | 0.0060 ns |  2.5250 ns | 1.000 |    0.00 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| Array1000                   | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| Array1000                   | .NET 8.0  | .NET 8.0  |  2.6730 ns |  0.3144 ns | 0.0172 ns |  2.6721 ns | 1.000 |    0.01 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| Array10000                  | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| Array10000                  | .NET 8.0  | .NET 8.0  |  2.5353 ns |  0.7163 ns | 0.0393 ns |  2.5546 ns | 1.000 |    0.02 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| List1                       | .NET 10.0 | .NET 10.0 |  0.0466 ns |  1.4723 ns | 0.0807 ns |  0.0000 ns |  0.02 |    0.03 |      - |         - |          NA |
| List1                       | .NET 8.0  | .NET 8.0  |  2.4733 ns |  0.0694 ns | 0.0038 ns |  2.4722 ns |  1.00 |    0.00 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| List10                      | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| List10                      | .NET 8.0  | .NET 8.0  |  2.5000 ns |  0.3702 ns | 0.0203 ns |  2.4974 ns | 1.000 |    0.01 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| List1000                    | .NET 10.0 | .NET 10.0 |  0.0534 ns |  1.6864 ns | 0.0924 ns |  0.0000 ns |  0.02 |    0.03 |      - |         - |          NA |
| List1000                    | .NET 8.0  | .NET 8.0  |  2.4748 ns |  0.0560 ns | 0.0031 ns |  2.4732 ns |  1.00 |    0.00 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| List10000                   | .NET 10.0 | .NET 10.0 |  0.0000 ns |  0.0000 ns | 0.0000 ns |  0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| List10000                   | .NET 8.0  | .NET 8.0  |  2.5390 ns |  0.7660 ns | 0.0420 ns |  2.5604 ns | 1.000 |    0.02 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| ComplexAllNull              | .NET 10.0 | .NET 10.0 |  5.7427 ns | 14.1398 ns | 0.7750 ns |  5.3487 ns |  0.84 |    0.10 |      - |         - |          NA |
| ComplexAllNull              | .NET 8.0  | .NET 8.0  |  6.8227 ns |  0.3060 ns | 0.0168 ns |  6.8147 ns |  1.00 |    0.00 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| ComplexSomeNonNull          | .NET 10.0 | .NET 10.0 |  5.2493 ns |  0.3151 ns | 0.0173 ns |  5.2456 ns |  0.77 |    0.00 |      - |         - |          NA |
| ComplexSomeNonNull          | .NET 8.0  | .NET 8.0  |  6.8193 ns |  0.1215 ns | 0.0067 ns |  6.8158 ns |  1.00 |    0.00 |      - |         - |          NA |
|                             |           |           |            |            |           |            |       |         |        |           |             |
| ComplexAllNonNull           | .NET 10.0 | .NET 10.0 |  5.2011 ns |  0.2376 ns | 0.0130 ns |  5.2067 ns |  0.76 |    0.00 |      - |         - |          NA |
| ComplexAllNonNull           | .NET 8.0  | .NET 8.0  |  6.8550 ns |  0.8917 ns | 0.0489 ns |  6.8278 ns |  1.00 |    0.01 |      - |         - |          NA |
