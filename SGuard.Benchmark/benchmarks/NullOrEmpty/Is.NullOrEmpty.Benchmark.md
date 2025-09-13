```

BenchmarkDotNet v0.15.2, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
Apple M3 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD


```
| Method                      | Mean       | Error     | StdDev    | Median     |
|---------------------------- |-----------:|----------:|----------:|-----------:|
| NullObj                     |  0.2802 ns | 0.0042 ns | 0.0039 ns |  0.2784 ns |
| EmptyString                 |  1.0042 ns | 0.0083 ns | 0.0077 ns |  1.0048 ns |
| NonEmptyString              |  0.9969 ns | 0.0029 ns | 0.0024 ns |  0.9961 ns |
| EmptyArray                  |  4.3745 ns | 0.0862 ns | 0.0806 ns |  4.3673 ns |
| NonEmptyArray               |  4.2343 ns | 0.0267 ns | 0.0236 ns |  4.2209 ns |
| EmptyList                   |  4.2180 ns | 0.0023 ns | 0.0018 ns |  4.2182 ns |
| NonEmptyList                |  4.2191 ns | 0.0077 ns | 0.0064 ns |  4.2168 ns |
| ZeroInt                     |  0.2005 ns | 0.0059 ns | 0.0053 ns |  0.1987 ns |
| NonZeroInt                  |  0.7358 ns | 0.0021 ns | 0.0018 ns |  0.7352 ns |
| NullObj_WithCallback        |  0.2936 ns | 0.0026 ns | 0.0022 ns |  0.2928 ns |
| EmptyString_WithCallback    |  1.2759 ns | 0.0070 ns | 0.0058 ns |  1.2741 ns |
| NonEmptyString_WithCallback |  1.2719 ns | 0.0025 ns | 0.0019 ns |  1.2717 ns |
| EmptyArray_WithCallback     |  4.7306 ns | 0.0589 ns | 0.0551 ns |  4.7328 ns |
| NonEmptyArray_WithCallback  |  4.6877 ns | 0.0547 ns | 0.0512 ns |  4.6746 ns |
| EmptyList_WithCallback      |  4.6171 ns | 0.0349 ns | 0.0327 ns |  4.6156 ns |
| NonEmptyList_WithCallback   |  4.5035 ns | 0.0293 ns | 0.0274 ns |  4.4940 ns |
| ZeroInt_WithCallback        |  0.3194 ns | 0.0045 ns | 0.0040 ns |  0.3176 ns |
| NonZeroInt_WithCallback     |  0.7382 ns | 0.0036 ns | 0.0030 ns |  0.7373 ns |
| WhitespaceString            |  1.0023 ns | 0.0039 ns | 0.0032 ns |  1.0015 ns |
| EmptyGuid                   |  0.4678 ns | 0.0089 ns | 0.0069 ns |  0.4651 ns |
| NonEmptyGuid                |  0.7357 ns | 0.0015 ns | 0.0013 ns |  0.7359 ns |
| EmptyDateTime               |  0.1985 ns | 0.0043 ns | 0.0038 ns |  0.1968 ns |
| NonEmptyDateTime            |  0.7419 ns | 0.0086 ns | 0.0076 ns |  0.7383 ns |
| EmptyTimeSpan               |  0.1992 ns | 0.0012 ns | 0.0010 ns |  0.1992 ns |
| NonEmptyTimeSpan            |  0.7456 ns | 0.0245 ns | 0.0191 ns |  0.7359 ns |
| EmptyDateOnly               |  0.2020 ns | 0.0045 ns | 0.0040 ns |  0.2004 ns |
| NonEmptyDateOnly            |  0.7365 ns | 0.0028 ns | 0.0022 ns |  0.7364 ns |
| EmptyTimeOnly               |  0.2015 ns | 0.0053 ns | 0.0044 ns |  0.1995 ns |
| NonEmptyTimeOnly            |  0.7374 ns | 0.0031 ns | 0.0026 ns |  0.7370 ns |
| EmptyDateTimeOffset         |  0.4711 ns | 0.0039 ns | 0.0033 ns |  0.4701 ns |
| NonEmptyDateTimeOffset      |  1.5416 ns | 0.0027 ns | 0.0023 ns |  1.5416 ns |
| EmptyDict                   |  4.2188 ns | 0.0205 ns | 0.0160 ns |  4.2129 ns |
| SingleDict                  |  4.2136 ns | 0.0061 ns | 0.0051 ns |  4.2129 ns |
| MultiDict                   |  4.2259 ns | 0.0216 ns | 0.0181 ns |  4.2191 ns |
| EmptyReadOnlyColl           |  4.2250 ns | 0.0055 ns | 0.0046 ns |  4.2235 ns |
| SingleReadOnlyColl          |  4.2392 ns | 0.0211 ns | 0.0187 ns |  4.2296 ns |
| MultiReadOnlyColl           |  4.2163 ns | 0.0067 ns | 0.0056 ns |  4.2152 ns |
| EmptyEnumerable             |  4.2171 ns | 0.0050 ns | 0.0042 ns |  4.2164 ns |
| SingleEnumerable            |  8.6878 ns | 0.7376 ns | 2.1748 ns |  9.6944 ns |
| MultiEnumerable             | 14.4149 ns | 1.7766 ns | 5.2383 ns | 10.9981 ns |
| Array1                      |  4.2136 ns | 0.0162 ns | 0.0144 ns |  4.2112 ns |
| Array10                     |  4.2673 ns | 0.0597 ns | 0.0558 ns |  4.2600 ns |
| Array1000                   |  4.2264 ns | 0.0182 ns | 0.0170 ns |  4.2192 ns |
| Array10000                  |  4.2314 ns | 0.0266 ns | 0.0222 ns |  4.2346 ns |
| List1                       |  4.2223 ns | 0.0041 ns | 0.0032 ns |  4.2224 ns |
| List10                      |  4.2245 ns | 0.0100 ns | 0.0089 ns |  4.2200 ns |
| List1000                    |  4.2196 ns | 0.0029 ns | 0.0024 ns |  4.2196 ns |
| List10000                   |  4.2194 ns | 0.0048 ns | 0.0040 ns |  4.2189 ns |
| ComplexAllNull              |  4.2230 ns | 0.0044 ns | 0.0034 ns |  4.2225 ns |
| ComplexSomeNonNull          |  4.2256 ns | 0.0070 ns | 0.0054 ns |  4.2240 ns |
| ComplexAllNonNull           |  4.2264 ns | 0.0039 ns | 0.0031 ns |  4.2262 ns |
