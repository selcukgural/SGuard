```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 10.0.105
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 8.0  : .NET 8.0.3 (8.0.3, 8.0.324.11423), Arm64 RyuJIT armv8.0-a

IterationCount=3  LaunchCount=1  WarmupCount=3  

```
| Method                            | Job       | Runtime   | Mean           | Error         | StdDev      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------------------------------- |---------- |---------- |---------------:|--------------:|------------:|------:|--------:|-------:|----------:|------------:|
| String_NonEmpty                   | .NET 10.0 | .NET 10.0 |      0.0361 ns |     0.0770 ns |   0.0042 ns |  0.02 |    0.00 |      - |         - |          NA |
| String_NonEmpty                   | .NET 8.0  | .NET 8.0  |      2.1598 ns |     0.2195 ns |   0.0120 ns |  1.00 |    0.01 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| String_Empty                      | .NET 10.0 | .NET 10.0 |  2,028.3119 ns |   333.3913 ns |  18.2743 ns |  0.16 |    0.00 | 0.0458 |     392 B |        0.75 |
| String_Empty                      | .NET 8.0  | .NET 8.0  | 12,999.3555 ns | 1,458.0286 ns |  79.9195 ns |  1.00 |    0.01 | 0.0610 |     520 B |        1.00 |
|                                   |           |           |                |               |             |       |         |        |           |             |
| String_Null                       | .NET 10.0 | .NET 10.0 |  1,971.9701 ns |   384.3743 ns |  21.0689 ns |  0.15 |    0.00 | 0.0458 |     392 B |        0.75 |
| String_Null                       | .NET 8.0  | .NET 8.0  | 13,173.4924 ns | 2,666.6060 ns | 146.1657 ns |  1.00 |    0.01 | 0.0610 |     520 B |        1.00 |
|                                   |           |           |                |               |             |       |         |        |           |             |
| String_WithCallback               | .NET 10.0 | .NET 10.0 |      0.5548 ns |     0.4644 ns |   0.0255 ns |  0.21 |    0.01 |      - |         - |          NA |
| String_WithCallback               | .NET 8.0  | .NET 8.0  |      2.6313 ns |     0.3939 ns |   0.0216 ns |  1.00 |    0.01 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Array_NonEmpty                    | .NET 10.0 | .NET 10.0 |      0.6318 ns |     0.1149 ns |   0.0063 ns |  0.22 |    0.01 |      - |         - |          NA |
| Array_NonEmpty                    | .NET 8.0  | .NET 8.0  |      2.9199 ns |     2.2722 ns |   0.1245 ns |  1.00 |    0.05 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Array_Empty                       | .NET 10.0 | .NET 10.0 |  1,950.7167 ns |    38.7098 ns |   2.1218 ns |  0.15 |    0.00 | 0.0458 |     392 B |        0.75 |
| Array_Empty                       | .NET 8.0  | .NET 8.0  | 13,176.1125 ns | 2,503.6700 ns | 137.2346 ns |  1.00 |    0.01 | 0.0610 |     520 B |        1.00 |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Array_WithCallback                | .NET 10.0 | .NET 10.0 |      1.0719 ns |     0.1141 ns |   0.0063 ns |  0.28 |    0.00 |      - |         - |          NA |
| Array_WithCallback                | .NET 8.0  | .NET 8.0  |      3.8236 ns |     1.3473 ns |   0.0739 ns |  1.00 |    0.02 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| List_NonEmpty                     | .NET 10.0 | .NET 10.0 |      0.6679 ns |     0.0433 ns |   0.0024 ns |  0.23 |    0.00 |      - |         - |          NA |
| List_NonEmpty                     | .NET 8.0  | .NET 8.0  |      2.8963 ns |     0.1558 ns |   0.0085 ns |  1.00 |    0.00 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| List_Empty                        | .NET 10.0 | .NET 10.0 |  2,046.2903 ns |   414.1247 ns |  22.6996 ns |  0.16 |    0.00 | 0.0458 |     392 B |        0.75 |
| List_Empty                        | .NET 8.0  | .NET 8.0  | 13,140.9522 ns | 1,714.3983 ns |  93.9720 ns |  1.00 |    0.01 | 0.0610 |     520 B |        1.00 |
|                                   |           |           |                |               |             |       |         |        |           |             |
| List_WithCallback                 | .NET 10.0 | .NET 10.0 |      1.0915 ns |     0.3010 ns |   0.0165 ns |  0.29 |    0.00 |      - |         - |          NA |
| List_WithCallback                 | .NET 8.0  | .NET 8.0  |      3.7804 ns |     0.2021 ns |   0.0111 ns |  1.00 |    0.00 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| String_CustomException            | .NET 10.0 | .NET 10.0 |  2,000.6835 ns |   460.0909 ns |  25.2191 ns |  0.15 |    0.00 | 0.0267 |     232 B |        0.64 |
| String_CustomException            | .NET 8.0  | .NET 8.0  | 13,166.3189 ns | 3,970.1968 ns | 217.6199 ns |  1.00 |    0.02 | 0.0305 |     360 B |        1.00 |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Array_CustomException             | .NET 10.0 | .NET 10.0 |  1,983.5569 ns |   865.1197 ns |  47.4201 ns |  0.15 |    0.00 | 0.0267 |     232 B |        0.64 |
| Array_CustomException             | .NET 8.0  | .NET 8.0  | 13,070.8046 ns | 1,651.1486 ns |  90.5050 ns |  1.00 |    0.01 | 0.0305 |     360 B |        1.00 |
|                                   |           |           |                |               |             |       |         |        |           |             |
| String_ExceptionWithArgs          | .NET 10.0 | .NET 10.0 |  2,123.1760 ns |   381.2674 ns |  20.8986 ns |  0.16 |    0.00 | 0.0687 |     600 B |        0.82 |
| String_ExceptionWithArgs          | .NET 8.0  | .NET 8.0  | 13,211.7971 ns | 1,207.4172 ns |  66.1826 ns |  1.00 |    0.01 | 0.0763 |     728 B |        1.00 |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Array_ExceptionWithArgs           | .NET 10.0 | .NET 10.0 |  2,138.3153 ns |   837.2453 ns |  45.8922 ns |  0.16 |    0.00 | 0.0687 |     600 B |        0.82 |
| Array_ExceptionWithArgs           | .NET 8.0  | .NET 8.0  | 13,390.7146 ns | 1,219.4418 ns |  66.8417 ns |  1.00 |    0.01 | 0.0763 |     728 B |        1.00 |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Object_Selector_NonEmpty          | .NET 10.0 | .NET 10.0 |    227.0795 ns |     7.6765 ns |   0.4208 ns |  0.76 |    0.00 | 0.0677 |     568 B |        1.00 |
| Object_Selector_NonEmpty          | .NET 8.0  | .NET 8.0  |    299.8806 ns |    16.4951 ns |   0.9042 ns |  1.00 |    0.00 | 0.0677 |     568 B |        1.00 |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Object_Selector_Empty             | .NET 10.0 | .NET 10.0 |  2,238.3795 ns |   547.2299 ns |  29.9955 ns |  0.16 |    0.00 | 0.1144 |     968 B |        0.88 |
| Object_Selector_Empty             | .NET 8.0  | .NET 8.0  | 13,637.9087 ns | 1,236.5478 ns |  67.7794 ns |  1.00 |    0.01 | 0.1221 |    1096 B |        1.00 |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Object_Selector_Null              | .NET 10.0 | .NET 10.0 |  2,275.8639 ns |   449.2672 ns |  24.6259 ns |  0.17 |    0.00 | 0.1144 |     968 B |        0.88 |
| Object_Selector_Null              | .NET 8.0  | .NET 8.0  | 13,784.7120 ns | 3,483.4432 ns | 190.9393 ns |  1.00 |    0.02 | 0.1221 |    1096 B |        1.00 |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Object_Selector_WithCallback      | .NET 10.0 | .NET 10.0 |    228.7074 ns |    14.2270 ns |   0.7798 ns |  0.76 |    0.01 | 0.0677 |     568 B |        1.00 |
| Object_Selector_WithCallback      | .NET 8.0  | .NET 8.0  |    300.5374 ns |    38.2580 ns |   2.0970 ns |  1.00 |    0.01 | 0.0677 |     568 B |        1.00 |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Object_Selector_CustomException   | .NET 10.0 | .NET 10.0 |  2,166.9396 ns |   243.6468 ns |  13.3551 ns |  0.16 |    0.00 | 0.0954 |     800 B |        0.86 |
| Object_Selector_CustomException   | .NET 8.0  | .NET 8.0  | 13,415.6808 ns | 1,247.3058 ns |  68.3690 ns |  1.00 |    0.01 | 0.1068 |     928 B |        1.00 |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Object_Selector_ExceptionWithArgs | .NET 10.0 | .NET 10.0 |  2,446.2616 ns |   230.0651 ns |  12.6106 ns |  0.18 |    0.00 | 0.1373 |    1168 B |        0.90 |
| Object_Selector_ExceptionWithArgs | .NET 8.0  | .NET 8.0  | 13,772.1350 ns | 1,842.6863 ns | 101.0039 ns |  1.00 |    0.01 | 0.1526 |    1296 B |        1.00 |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Array_1000                        | .NET 10.0 | .NET 10.0 |      0.6582 ns |     0.1886 ns |   0.0103 ns |  0.22 |    0.00 |      - |         - |          NA |
| Array_1000                        | .NET 8.0  | .NET 8.0  |      2.9330 ns |     0.2157 ns |   0.0118 ns |  1.00 |    0.00 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Array_5000                        | .NET 10.0 | .NET 10.0 |      0.6533 ns |     0.0401 ns |   0.0022 ns |  0.23 |    0.00 |      - |         - |          NA |
| Array_5000                        | .NET 8.0  | .NET 8.0  |      2.8845 ns |     0.0314 ns |   0.0017 ns |  1.00 |    0.00 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Array_10000                       | .NET 10.0 | .NET 10.0 |      0.6649 ns |     0.2607 ns |   0.0143 ns |  0.23 |    0.00 |      - |         - |          NA |
| Array_10000                       | .NET 8.0  | .NET 8.0  |      2.9085 ns |     0.6277 ns |   0.0344 ns |  1.00 |    0.01 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Array_15000                       | .NET 10.0 | .NET 10.0 |      0.6409 ns |     0.0924 ns |   0.0051 ns |  0.21 |    0.01 |      - |         - |          NA |
| Array_15000                       | .NET 8.0  | .NET 8.0  |      3.0379 ns |     1.6571 ns |   0.0908 ns |  1.00 |    0.04 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Array_1000_WithCallback           | .NET 10.0 | .NET 10.0 |      1.0581 ns |     0.0671 ns |   0.0037 ns |  0.30 |    0.00 |      - |         - |          NA |
| Array_1000_WithCallback           | .NET 8.0  | .NET 8.0  |      3.5333 ns |     0.6270 ns |   0.0344 ns |  1.00 |    0.01 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Array_5000_WithCallback           | .NET 10.0 | .NET 10.0 |      1.0502 ns |     0.0782 ns |   0.0043 ns |  0.28 |    0.00 |      - |         - |          NA |
| Array_5000_WithCallback           | .NET 8.0  | .NET 8.0  |      3.7118 ns |     0.2277 ns |   0.0125 ns |  1.00 |    0.00 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Array_10000_WithCallback          | .NET 10.0 | .NET 10.0 |      1.0600 ns |     0.2262 ns |   0.0124 ns |  0.28 |    0.00 |      - |         - |          NA |
| Array_10000_WithCallback          | .NET 8.0  | .NET 8.0  |      3.7872 ns |     0.0888 ns |   0.0049 ns |  1.00 |    0.00 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| Array_15000_WithCallback          | .NET 10.0 | .NET 10.0 |      1.0580 ns |     0.1504 ns |   0.0082 ns |  0.28 |    0.00 |      - |         - |          NA |
| Array_15000_WithCallback          | .NET 8.0  | .NET 8.0  |      3.7398 ns |     0.1257 ns |   0.0069 ns |  1.00 |    0.00 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| List_1000                         | .NET 10.0 | .NET 10.0 |      0.6783 ns |     0.0561 ns |   0.0031 ns |  0.23 |    0.00 |      - |         - |          NA |
| List_1000                         | .NET 8.0  | .NET 8.0  |      2.9018 ns |     0.0385 ns |   0.0021 ns |  1.00 |    0.00 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| List_5000                         | .NET 10.0 | .NET 10.0 |      0.6948 ns |     0.1550 ns |   0.0085 ns |  0.23 |    0.00 |      - |         - |          NA |
| List_5000                         | .NET 8.0  | .NET 8.0  |      2.9592 ns |     1.0201 ns |   0.0559 ns |  1.00 |    0.02 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| List_10000                        | .NET 10.0 | .NET 10.0 |      0.6758 ns |     0.2054 ns |   0.0113 ns |  0.23 |    0.00 |      - |         - |          NA |
| List_10000                        | .NET 8.0  | .NET 8.0  |      2.8993 ns |     0.0960 ns |   0.0053 ns |  1.00 |    0.00 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| List_15000                        | .NET 10.0 | .NET 10.0 |      0.6459 ns |     0.0973 ns |   0.0053 ns |  0.22 |    0.00 |      - |         - |          NA |
| List_15000                        | .NET 8.0  | .NET 8.0  |      2.8948 ns |     0.0705 ns |   0.0039 ns |  1.00 |    0.00 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| List_1000_WithCallback            | .NET 10.0 | .NET 10.0 |      1.1006 ns |     0.3715 ns |   0.0204 ns |  0.29 |    0.00 |      - |         - |          NA |
| List_1000_WithCallback            | .NET 8.0  | .NET 8.0  |      3.7664 ns |     0.2063 ns |   0.0113 ns |  1.00 |    0.00 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| List_5000_WithCallback            | .NET 10.0 | .NET 10.0 |      1.0721 ns |     0.5015 ns |   0.0275 ns |  0.29 |    0.01 |      - |         - |          NA |
| List_5000_WithCallback            | .NET 8.0  | .NET 8.0  |      3.7398 ns |     0.4728 ns |   0.0259 ns |  1.00 |    0.01 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| List_10000_WithCallback           | .NET 10.0 | .NET 10.0 |      1.0843 ns |     0.7875 ns |   0.0432 ns |  0.29 |    0.01 |      - |         - |          NA |
| List_10000_WithCallback           | .NET 8.0  | .NET 8.0  |      3.7648 ns |     0.3734 ns |   0.0205 ns |  1.00 |    0.01 |      - |         - |          NA |
|                                   |           |           |                |               |             |       |         |        |           |             |
| List_15000_WithCallback           | .NET 10.0 | .NET 10.0 |      1.0550 ns |     0.2724 ns |   0.0149 ns |  0.28 |    0.00 |      - |         - |          NA |
| List_15000_WithCallback           | .NET 8.0  | .NET 8.0  |      3.7902 ns |     0.9688 ns |   0.0531 ns |  1.00 |    0.02 |      - |         - |          NA |
