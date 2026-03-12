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

Creating a random KSUID `string` with the prefix `cust_`:

``` csharp
string id = Ksuid.NewKsuid("cust_");

Console.WriteLine(id); // outputs c_29faSiN1gPB6IzM74u6tMfTO02L
```

## micro benchmarks

Benchmark results for KsuidDotNet and other KSUID libraries.

### ARM64: M2 Ultra
BenchmarkDotNet v0.15.6, macOS Sequoia 15.4 (24E248) [Darwin 24.4.0]
Apple M2 Ultra, 1 CPU, 24 logical and 24 physical cores
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a

```
| Method                                       | Mean     | Min      | Max      | Op/s         | Gen0   | Allocated |
|--------------------------------------------- |---------:|---------:|---------:|-------------:|-------:|----------:|
| 'NewKsuid() - Real World'                    | 76.91 ns | 76.57 ns | 77.23 ns | 13,001,667.4 | 0.0095 |      80 B |
| 'NewKsuid(Prefix) - Real World'              | 77.70 ns | 76.70 ns | 78.19 ns | 12,870,678.6 | 0.0105 |      88 B |
| 'NewKsuid(DateTime, Prefix) - Deterministic' | 58.82 ns | 58.51 ns | 59.09 ns | 17,001,186.1 | 0.0105 |      88 B |
```

## license

This library is open-source software released under the MIT [License](LICENSE).
