```

BenchmarkDotNet v0.15.2, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD


```
| Method                            | Mean          | Error         | StdDev        |
|---------------------------------- |--------------:|--------------:|--------------:|
| String_NonEmpty                   |     10.058 ns |     0.2058 ns |     0.1925 ns |
| String_Empty                      |  8,766.299 ns |   127.5326 ns |   119.2941 ns |
| String_Null                       |  8,712.074 ns |    93.0086 ns |    87.0003 ns |
| String_WithCallback               |      9.521 ns |     0.1430 ns |     0.1337 ns |
| Array_NonEmpty                    |     13.627 ns |     0.1195 ns |     0.1060 ns |
| Array_Empty                       |  8,768.696 ns |    78.1998 ns |    69.3221 ns |
| Array_WithCallback                |     13.627 ns |     0.2077 ns |     0.1943 ns |
| List_NonEmpty                     |     13.373 ns |     0.0704 ns |     0.0624 ns |
| List_Empty                        |  8,795.241 ns |   116.9610 ns |   109.4054 ns |
| List_WithCallback                 |     13.410 ns |     0.1164 ns |     0.0972 ns |
| String_CustomException            |  8,266.905 ns |    45.7759 ns |    42.8188 ns |
| Array_CustomException             |  8,366.580 ns |   166.1771 ns |   163.2082 ns |
| String_ExceptionWithArgs          |  8,962.798 ns |   145.7651 ns |   136.3487 ns |
| Array_ExceptionWithArgs           |  8,851.725 ns |   109.0751 ns |   102.0289 ns |
| Object_Selector_NonEmpty          | 50,288.787 ns |   481.6312 ns |   402.1839 ns |
| Object_Selector_Empty             | 60,562.018 ns | 1,199.0188 ns | 1,380.7918 ns |
| Object_Selector_Null              | 60,046.333 ns | 1,185.6605 ns |   990.0803 ns |
| Object_Selector_WithCallback      | 51,924.871 ns | 1,001.2727 ns | 1,266.2895 ns |
| Object_Selector_CustomException   | 59,270.411 ns | 1,058.2878 ns |   826.2415 ns |
| Object_Selector_ExceptionWithArgs | 61,321.871 ns | 1,131.9097 ns | 1,471.8025 ns |
| Array_1000                        |     13.382 ns |     0.0897 ns |     0.0701 ns |
| Array_5000                        |     13.555 ns |     0.1634 ns |     0.1528 ns |
| Array_10000                       |     13.302 ns |     0.0380 ns |     0.0337 ns |
| Array_15000                       |     13.418 ns |     0.1454 ns |     0.1360 ns |
| Array_1000_WithCallback           |     13.508 ns |     0.1956 ns |     0.1830 ns |
| Array_5000_WithCallback           |     13.521 ns |     0.2271 ns |     0.2124 ns |
| Array_10000_WithCallback          |     13.552 ns |     0.1656 ns |     0.1383 ns |
| Array_15000_WithCallback          |     13.467 ns |     0.1471 ns |     0.1304 ns |
| List_1000                         |     13.535 ns |     0.2140 ns |     0.2002 ns |
| List_5000                         |     13.324 ns |     0.0842 ns |     0.0703 ns |
| List_10000                        |     13.470 ns |     0.0689 ns |     0.0575 ns |
| List_15000                        |     13.456 ns |     0.1414 ns |     0.1323 ns |
| List_1000_WithCallback            |     13.469 ns |     0.1020 ns |     0.0954 ns |
| List_5000_WithCallback            |     13.467 ns |     0.1764 ns |     0.1650 ns |
| List_10000_WithCallback           |     13.463 ns |     0.1650 ns |     0.1377 ns |
| List_15000_WithCallback           |     13.761 ns |     0.1402 ns |     0.1311 ns |
