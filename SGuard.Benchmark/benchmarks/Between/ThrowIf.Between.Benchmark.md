```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 10.0.105
  [Host]    : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  .NET 8.0  : .NET 8.0.3 (8.0.3, 8.0.324.11423), Arm64 RyuJIT armv8.0-a

IterationCount=3  LaunchCount=1  WarmupCount=3  

```
| Method                                      | Job       | Runtime   | Mean           | Error         | StdDev      | Median         | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------------------------------- |---------- |---------- |---------------:|--------------:|------------:|---------------:|------:|--------:|-------:|----------:|------------:|
| Int_Below                                   | .NET 10.0 | .NET 10.0 |      0.0017 ns |     0.0527 ns |   0.0029 ns |      0.0000 ns | 0.003 |    0.00 |      - |         - |          NA |
| Int_Below                                   | .NET 8.0  | .NET 8.0  |      0.6084 ns |     0.0677 ns |   0.0037 ns |      0.6081 ns | 1.000 |    0.01 |      - |         - |          NA |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| Int_AtMin                                   | .NET 10.0 | .NET 10.0 |  2,158.3997 ns |    52.6127 ns |   2.8839 ns |  2,158.3799 ns |  0.17 |    0.00 | 0.0916 |     776 B |        0.99 |
| Int_AtMin                                   | .NET 8.0  | .NET 8.0  | 12,874.6588 ns | 3,399.6318 ns | 186.3453 ns | 12,790.1313 ns |  1.00 |    0.02 | 0.0916 |     784 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| Int_Between                                 | .NET 10.0 | .NET 10.0 |  2,157.0354 ns |   273.9253 ns |  15.0148 ns |  2,158.5366 ns |  0.17 |    0.00 | 0.0916 |     776 B |        0.99 |
| Int_Between                                 | .NET 8.0  | .NET 8.0  | 12,873.0195 ns | 1,309.0741 ns |  71.7548 ns | 12,900.7899 ns |  1.00 |    0.01 | 0.0916 |     784 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| Int_AtMax                                   | .NET 10.0 | .NET 10.0 |  2,223.3535 ns |   761.1321 ns |  41.7202 ns |  2,204.8818 ns |  0.16 |    0.00 | 0.0916 |     776 B |        0.99 |
| Int_AtMax                                   | .NET 8.0  | .NET 8.0  | 13,657.5570 ns | 2,053.9290 ns | 112.5828 ns | 13,684.8819 ns |  1.00 |    0.01 | 0.0916 |     784 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| Int_Above                                   | .NET 10.0 | .NET 10.0 |      0.0067 ns |     0.0247 ns |   0.0014 ns |      0.0074 ns | 0.010 |    0.00 |      - |         - |          NA |
| Int_Above                                   | .NET 8.0  | .NET 8.0  |      0.6717 ns |     0.4084 ns |   0.0224 ns |      0.6735 ns | 1.001 |    0.04 |      - |         - |          NA |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| Int_Between_WithCallback                    | .NET 10.0 | .NET 10.0 |  2,144.1886 ns |   265.4339 ns |  14.5493 ns |  2,147.3934 ns |  0.17 |    0.00 | 0.0916 |     776 B |        0.99 |
| Int_Between_WithCallback                    | .NET 8.0  | .NET 8.0  | 12,833.1184 ns | 2,279.9881 ns | 124.9738 ns | 12,780.7191 ns |  1.00 |    0.01 | 0.0916 |     784 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| Int_Between_CustomException                 | .NET 10.0 | .NET 10.0 |  1,939.1113 ns |   422.1287 ns |  23.1383 ns |  1,937.2177 ns |  0.15 |    0.00 | 0.0229 |     216 B |        0.96 |
| Int_Between_CustomException                 | .NET 8.0  | .NET 8.0  | 12,576.3054 ns |   657.4567 ns |  36.0374 ns | 12,559.0572 ns |  1.00 |    0.00 | 0.0153 |     224 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| Int_Between_CustomException_WithCallback    | .NET 10.0 | .NET 10.0 |  1,952.4529 ns |   222.2006 ns |  12.1796 ns |  1,948.8600 ns |  0.15 |    0.00 | 0.0229 |     216 B |        0.96 |
| Int_Between_CustomException_WithCallback    | .NET 8.0  | .NET 8.0  | 12,683.0637 ns | 3,006.0000 ns | 164.7690 ns | 12,662.7852 ns |  1.00 |    0.02 | 0.0153 |     224 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| Double_Below                                | .NET 10.0 | .NET 10.0 |      0.0000 ns |     0.0000 ns |   0.0000 ns |      0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| Double_Below                                | .NET 8.0  | .NET 8.0  |      0.6222 ns |     0.4253 ns |   0.0233 ns |      0.6143 ns | 1.001 |    0.05 |      - |         - |          NA |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| Double_AtMin                                | .NET 10.0 | .NET 10.0 |  2,177.5581 ns |   651.1750 ns |  35.6931 ns |  2,167.0826 ns |  0.17 |    0.00 | 0.0954 |     808 B |        0.99 |
| Double_AtMin                                | .NET 8.0  | .NET 8.0  | 12,946.6538 ns | 2,087.3591 ns | 114.4152 ns | 12,985.6809 ns |  1.00 |    0.01 | 0.0916 |     816 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| Double_Between                              | .NET 10.0 | .NET 10.0 |  2,188.6509 ns |   420.1645 ns |  23.0306 ns |  2,187.9829 ns |  0.17 |    0.00 | 0.0954 |     816 B |        0.99 |
| Double_Between                              | .NET 8.0  | .NET 8.0  | 12,733.9647 ns | 1,630.4239 ns |  89.3690 ns | 12,721.0197 ns |  1.00 |    0.01 | 0.0916 |     824 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| Double_AtMax                                | .NET 10.0 | .NET 10.0 |  2,255.9651 ns |   620.2034 ns |  33.9954 ns |  2,259.5220 ns |  0.17 |    0.00 | 0.0954 |     808 B |        0.99 |
| Double_AtMax                                | .NET 8.0  | .NET 8.0  | 13,023.9862 ns | 2,109.1501 ns | 115.6096 ns | 12,970.3109 ns |  1.00 |    0.01 | 0.0916 |     816 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| Double_Above                                | .NET 10.0 | .NET 10.0 |      0.0000 ns |     0.0000 ns |   0.0000 ns |      0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| Double_Above                                | .NET 8.0  | .NET 8.0  |      0.6372 ns |     0.0507 ns |   0.0028 ns |      0.6360 ns | 1.000 |    0.01 |      - |         - |          NA |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| Double_Between_WithCallback                 | .NET 10.0 | .NET 10.0 |  2,179.9940 ns |   690.9771 ns |  37.8748 ns |  2,172.8109 ns |  0.17 |    0.00 | 0.0954 |     816 B |        0.99 |
| Double_Between_WithCallback                 | .NET 8.0  | .NET 8.0  | 13,108.9456 ns | 4,057.4746 ns | 222.4039 ns | 13,201.9329 ns |  1.00 |    0.02 | 0.0916 |     824 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| Double_Between_CustomException              | .NET 10.0 | .NET 10.0 |  1,863.9852 ns |   187.0580 ns |  10.2533 ns |  1,869.0849 ns |  0.15 |    0.00 | 0.0248 |     216 B |        0.96 |
| Double_Between_CustomException              | .NET 8.0  | .NET 8.0  | 12,457.3292 ns |   579.4499 ns |  31.7616 ns | 12,460.8269 ns |  1.00 |    0.00 | 0.0153 |     224 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| Double_Between_CustomException_WithCallback | .NET 10.0 | .NET 10.0 |  1,870.5826 ns |   134.9883 ns |   7.3992 ns |  1,874.2589 ns |  0.15 |    0.00 | 0.0248 |     216 B |        0.96 |
| Double_Between_CustomException_WithCallback | .NET 8.0  | .NET 8.0  | 12,647.4605 ns | 1,530.6599 ns |  83.9006 ns | 12,694.6837 ns |  1.00 |    0.01 | 0.0153 |     224 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| String_Below                                | .NET 10.0 | .NET 10.0 |      0.5878 ns |     0.0629 ns |   0.0035 ns |      0.5878 ns |  0.24 |    0.00 |      - |         - |          NA |
| String_Below                                | .NET 8.0  | .NET 8.0  |      2.4229 ns |     0.1848 ns |   0.0101 ns |      2.4259 ns |  1.00 |    0.01 |      - |         - |          NA |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| String_AtMin                                | .NET 10.0 | .NET 10.0 |  2,225.4741 ns |   136.0560 ns |   7.4577 ns |  2,224.2257 ns |  0.17 |    0.00 | 0.0839 |     704 B |        0.99 |
| String_AtMin                                | .NET 8.0  | .NET 8.0  | 12,901.9237 ns |   697.3444 ns |  38.2238 ns | 12,883.6880 ns |  1.00 |    0.00 | 0.0763 |     712 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| String_Between                              | .NET 10.0 | .NET 10.0 |  2,298.5690 ns |   345.3678 ns |  18.9308 ns |  2,293.2560 ns |  0.18 |    0.00 | 0.0839 |     704 B |        0.99 |
| String_Between                              | .NET 8.0  | .NET 8.0  | 12,974.7249 ns | 2,413.8740 ns | 132.3126 ns | 13,016.4534 ns |  1.00 |    0.01 | 0.0763 |     712 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| String_AtMax                                | .NET 10.0 | .NET 10.0 |  2,244.0591 ns |   462.3819 ns |  25.3447 ns |  2,233.0817 ns |  0.17 |    0.00 | 0.0839 |     704 B |        0.99 |
| String_AtMax                                | .NET 8.0  | .NET 8.0  | 12,972.9805 ns |   956.5167 ns |  52.4299 ns | 12,987.7192 ns |  1.00 |    0.00 | 0.0763 |     712 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| String_Above                                | .NET 10.0 | .NET 10.0 |      0.0000 ns |     0.0000 ns |   0.0000 ns |      0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| String_Above                                | .NET 8.0  | .NET 8.0  |      1.0951 ns |     0.3128 ns |   0.0171 ns |      1.1040 ns | 1.000 |    0.02 |      - |         - |          NA |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| String_Empty                                | .NET 10.0 | .NET 10.0 |      0.0000 ns |     0.0000 ns |   0.0000 ns |      0.0000 ns | 0.000 |    0.00 |      - |         - |          NA |
| String_Empty                                | .NET 8.0  | .NET 8.0  |      1.0598 ns |     0.0876 ns |   0.0048 ns |      1.0587 ns | 1.000 |    0.01 |      - |         - |          NA |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| String_Whitespace                           | .NET 10.0 | .NET 10.0 |      0.0356 ns |     0.0587 ns |   0.0032 ns |      0.0370 ns |  0.03 |    0.00 |      - |         - |          NA |
| String_Whitespace                           | .NET 8.0  | .NET 8.0  |      1.0691 ns |     0.0492 ns |   0.0027 ns |      1.0682 ns |  1.00 |    0.00 |      - |         - |          NA |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| String_Null                                 | .NET 10.0 | .NET 10.0 |  2,321.4547 ns |   787.4212 ns |  43.1612 ns |  2,322.7043 ns |  0.17 |    0.00 | 0.0381 |     328 B |        0.93 |
| String_Null                                 | .NET 8.0  | .NET 8.0  | 13,304.5873 ns | 3,933.9487 ns | 215.6330 ns | 13,407.2965 ns |  1.00 |    0.02 | 0.0305 |     352 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| String_Between_WithCallback                 | .NET 10.0 | .NET 10.0 |  2,242.6030 ns |   417.5626 ns |  22.8880 ns |  2,238.6730 ns |  0.17 |    0.00 | 0.0839 |     704 B |        0.99 |
| String_Between_WithCallback                 | .NET 8.0  | .NET 8.0  | 12,948.0638 ns | 1,614.0680 ns |  88.4725 ns | 12,937.0454 ns |  1.00 |    0.01 | 0.0763 |     712 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| String_Between_CustomException              | .NET 10.0 | .NET 10.0 |  1,942.6586 ns |   649.7838 ns |  35.6168 ns |  1,945.1050 ns |  0.15 |    0.00 | 0.0229 |     216 B |        0.96 |
| String_Between_CustomException              | .NET 8.0  | .NET 8.0  | 12,794.6790 ns | 2,467.3830 ns | 135.2456 ns | 12,730.1591 ns |  1.00 |    0.01 | 0.0153 |     224 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| String_Between_CustomException_WithCallback | .NET 10.0 | .NET 10.0 |  1,951.8969 ns |   393.6272 ns |  21.5760 ns |  1,958.4462 ns |  0.16 |    0.00 | 0.0229 |     216 B |        0.96 |
| String_Between_CustomException_WithCallback | .NET 8.0  | .NET 8.0  | 12,449.0490 ns |   685.5354 ns |  37.5765 ns | 12,469.1366 ns |  1.00 |    0.00 | 0.0153 |     224 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| String_Between_OrdinalIgnoreCase            | .NET 10.0 | .NET 10.0 |  2,307.5797 ns |   287.1164 ns |  15.7378 ns |  2,298.6635 ns |  0.18 |    0.00 | 0.0839 |     704 B |        0.99 |
| String_Between_OrdinalIgnoreCase            | .NET 8.0  | .NET 8.0  | 12,986.5553 ns |   786.0668 ns |  43.0870 ns | 12,970.9733 ns |  1.00 |    0.00 | 0.0763 |     712 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| String_Between_InvariantCulture             | .NET 10.0 | .NET 10.0 |  2,309.0472 ns |   407.7181 ns |  22.3484 ns |  2,296.7577 ns |  0.18 |    0.00 | 0.0839 |     704 B |        0.99 |
| String_Between_InvariantCulture             | .NET 8.0  | .NET 8.0  | 12,969.0113 ns |   841.1024 ns |  46.1037 ns | 12,985.8373 ns |  1.00 |    0.00 | 0.0763 |     712 B |        1.00 |
|                                             |           |           |                |               |             |                |       |         |        |           |             |
| String_Between_InvariantCultureIgnoreCase   | .NET 10.0 | .NET 10.0 |  2,364.2380 ns |   415.1550 ns |  22.7560 ns |  2,374.3914 ns |  0.18 |    0.00 | 0.0839 |     704 B |        0.99 |
| String_Between_InvariantCultureIgnoreCase   | .NET 8.0  | .NET 8.0  | 13,086.0680 ns | 2,801.0771 ns | 153.5365 ns | 13,151.1059 ns |  1.00 |    0.01 | 0.0763 |     712 B |        1.00 |
