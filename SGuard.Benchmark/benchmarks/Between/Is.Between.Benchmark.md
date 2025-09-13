```

BenchmarkDotNet v0.15.2, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD


```
| Method                                      | Mean      | Error     | StdDev    |
|-------------------------------------------- |----------:|----------:|----------:|
| Between_Int_True                            | 0.4647 ns | 0.0173 ns | 0.0162 ns |
| Between_Int_False_Low                       | 0.4686 ns | 0.0293 ns | 0.0259 ns |
| Between_Int_False_High                      | 0.7369 ns | 0.0088 ns | 0.0078 ns |
| Between_Int_True_WithCallback               | 0.4398 ns | 0.0011 ns | 0.0009 ns |
| Between_Int_False_Low_WithCallback          | 0.4615 ns | 0.0057 ns | 0.0047 ns |
| Between_Int_False_High_WithCallback         | 0.7309 ns | 0.0080 ns | 0.0075 ns |
| Between_String_True                         | 2.1394 ns | 0.0137 ns | 0.0129 ns |
| Between_String_False_Low                    | 1.1108 ns | 0.0210 ns | 0.0196 ns |
| Between_String_False_High                   | 3.4289 ns | 0.0189 ns | 0.0167 ns |
| Between_String_True_WithCallback            | 2.4614 ns | 0.0080 ns | 0.0067 ns |
| Between_String_False_Low_WithCallback       | 1.2951 ns | 0.0243 ns | 0.0203 ns |
| Between_String_False_High_WithCallback      | 3.6130 ns | 0.0399 ns | 0.0373 ns |
| Between_Int_Edge_1000000_True               | 0.2032 ns | 0.0048 ns | 0.0043 ns |
| Between_Int_Edge_1000000_False              | 0.2050 ns | 0.0071 ns | 0.0063 ns |
| Between_Int_Edge_1000000_True_WithCallback  | 0.2050 ns | 0.0049 ns | 0.0046 ns |
| Between_Int_Edge_1000000_False_WithCallback | 0.2055 ns | 0.0065 ns | 0.0061 ns |
