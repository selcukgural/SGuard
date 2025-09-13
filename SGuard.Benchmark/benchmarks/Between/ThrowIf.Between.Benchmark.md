```

BenchmarkDotNet v0.15.2, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD


```
| Method                                      | Mean         | Error       | StdDev      |
|-------------------------------------------- |-------------:|------------:|------------:|
| Int_Below                                   |     9.586 ns |   0.0668 ns |   0.0592 ns |
| Int_AtMin                                   | 8,663.385 ns |  64.6742 ns |  60.4963 ns |
| Int_Between                                 | 8,628.795 ns |  73.3590 ns |  68.6200 ns |
| Int_AtMax                                   | 8,910.887 ns | 137.8403 ns | 128.9359 ns |
| Int_Above                                   |     9.511 ns |   0.0245 ns |   0.0204 ns |
| Int_Between_WithCallback                    | 8,772.870 ns |  66.3124 ns |  62.0286 ns |
| Int_Between_CustomException                 | 8,274.773 ns |  63.0196 ns |  58.9486 ns |
| Int_Between_CustomException_WithCallback    | 8,780.223 ns | 167.7701 ns | 212.1755 ns |
| Double_Below                                |     9.880 ns |   0.1254 ns |   0.1112 ns |
| Double_AtMin                                | 8,889.465 ns |  79.0086 ns |  70.0391 ns |
| Double_Between                              | 9,078.472 ns | 177.3030 ns | 182.0771 ns |
| Double_AtMax                                | 8,927.127 ns |  82.6051 ns |  77.2688 ns |
| Double_Above                                |     9.949 ns |   0.1409 ns |   0.1318 ns |
| Double_Between_WithCallback                 | 8,802.813 ns |  94.6351 ns |  88.5217 ns |
| Double_Between_CustomException              | 8,351.660 ns | 121.3295 ns | 113.4917 ns |
| Double_Between_CustomException_WithCallback | 8,523.615 ns | 132.2316 ns | 123.6896 ns |
| String_Below                                |    11.502 ns |   0.2322 ns |   0.2172 ns |
| String_AtMin                                | 8,872.118 ns | 133.3930 ns | 124.7759 ns |
| String_Between                              | 8,988.303 ns |  79.3540 ns |  74.2278 ns |
| String_AtMax                                | 8,762.791 ns | 172.1382 ns | 176.7732 ns |
| String_Above                                |    10.285 ns |   0.0612 ns |   0.0572 ns |
| String_Empty                                |    10.260 ns |   0.0123 ns |   0.0096 ns |
| String_Whitespace                           |    10.297 ns |   0.0698 ns |   0.0545 ns |
| String_Null                                 | 8,608.857 ns |  83.2303 ns |  77.8537 ns |
| String_Between_WithCallback                 | 8,831.132 ns | 129.4252 ns | 121.0644 ns |
| String_Between_CustomException              | 8,297.508 ns |  66.2288 ns |  61.9505 ns |
| String_Between_CustomException_WithCallback | 8,459.259 ns |  60.9057 ns |  56.9712 ns |
| String_Between_OrdinalIgnoreCase            | 8,803.672 ns |  50.2182 ns |  46.9741 ns |
| String_Between_InvariantCulture             | 8,782.474 ns |  40.6367 ns |  36.0233 ns |
| String_Between_InvariantCultureIgnoreCase   | 8,770.496 ns |  28.1287 ns |  23.4887 ns |
