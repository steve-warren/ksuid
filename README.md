# KsuidDotNet [![nuget](https://img.shields.io/nuget/v/KsuidDotNet?style=flat-square)](https://www.nuget.org/packages/KsuidDotNet)

A high-performance, zero-allocation KSUID (Key Sortable Unique Identifier) generator for .NET 8 and .NET 10.

Designed for high-throughput database engines and distributed systems where unique ID generation is on the hot path.

## 🚀Performance

This library is engineered for speed. It uses hardware intrinsics, stackalloc, and a buffered RNG strategy to outperform standard implementations.

- creates crypto-random KSUID strings
- blazing-fast performance
- no additional memory overhead, memory allocation only for string
- thread-safe and lock-free
- targets .net 8, .net 10

### Key Optimizations

- Zero Allocation: Uses `string.Create` and `Span<T>` to write directly to the 
heap, bypassing intermediate string allocations.

- Buffered RNG: Uses a thread-static buffer to amortize the cost of Syscall 
(randomness) overhead by ~99%.

- Math Intrinsics: Leverages modern .NET `Math.DivRem` and `BinaryPrimitives` to 
optimize the Base62 encoding loop.

## installation

``` pwsh
$ dotnet add package KsuidDotNet
```

## how to use

Create a random KSUID:

``` csharp
string id = Ksuid.NewKsuid();

Console.WriteLine(id); // outputs 29faSiN1gPB6IzM74u6tMfTO02L
```

Creating a random KSUID `string` with the prefix `c_`:

``` csharp
string id = Ksuid.NewKsuid("c_");

Console.WriteLine(id); // outputs c_29faSiN1gPB6IzM74u6tMfTO02L
```

## micro benchmarks

Benchmark results for KsuidDotNet and other KSUID libraries.

``` ini
BenchmarkDotNet v0.15.6, macOS 26.1 (25B78) [Darwin 25.1.0]
Apple M2 Max, 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
```

| Method                                    | Mean       | Error   | StdDev  | StdErr  | Min        | Q1         | Median     | Q3         | Max        | Op/s        | Gen0   | Allocated |
|------------------------------------------ |-----------:|--------:|--------:|--------:|-----------:|-----------:|-----------:|-----------:|-----------:|------------:|-------:|----------:|
| KsuidDotNet_NewKsuid                      |   212.9 ns | 0.44 ns | 0.39 ns | 0.10 ns |   212.2 ns |   212.6 ns |   212.8 ns |   213.0 ns |   213.6 ns | 4,697,926.3 | 0.0095 |      80 B |
| KsuidDotNet_NewKsuid_WithPrefix           |   215.0 ns | 0.35 ns | 0.33 ns | 0.08 ns |   214.5 ns |   214.8 ns |   215.0 ns |   215.2 ns |   215.4 ns | 4,651,593.6 | 0.0095 |      80 B |
| StructKsuid_RandomKsuidToString           |   399.2 ns | 0.50 ns | 0.45 ns | 0.12 ns |   398.2 ns |   398.9 ns |   399.3 ns |   399.5 ns |   399.7 ns | 2,505,210.7 | 0.0095 |      80 B |
| StructKsuid_RandomKsuidToStringWithPrefix |   410.8 ns | 0.68 ns | 0.64 ns | 0.17 ns |   409.4 ns |   410.5 ns |   410.8 ns |   411.2 ns |   412.0 ns | 2,434,101.3 | 0.0191 |     160 B |
| DotKsuid_GenerateToString                 |   432.4 ns | 0.98 ns | 0.87 ns | 0.23 ns |   430.8 ns |   431.8 ns |   432.5 ns |   433.0 ns |   433.6 ns | 2,312,888.0 | 0.1516 |    1272 B |
| DotKsuid_GenerateToStringWithPrefix       |   438.7 ns | 0.63 ns | 0.59 ns | 0.15 ns |   437.6 ns |   438.3 ns |   438.7 ns |   439.0 ns |   439.8 ns | 2,279,615.1 | 0.1612 |    1352 B |
| ksuid_GenerateToString                    | 1,959.4 ns | 5.88 ns | 5.50 ns | 1.42 ns | 1,947.4 ns | 1,956.0 ns | 1,960.7 ns | 1,963.4 ns | 1,967.1 ns |   510,368.8 | 0.9346 |    7824 B |
| ksuid_GenerateToStringWithPrefix          | 1,972.9 ns | 5.97 ns | 4.98 ns | 1.38 ns | 1,961.1 ns | 1,972.1 ns | 1,974.1 ns | 1,975.4 ns | 1,980.4 ns |   506,877.3 | 0.9422 |    7904 B |

## license

This library is open-source software released under the MIT [License](LICENSE).