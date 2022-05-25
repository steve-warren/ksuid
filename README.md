# ksuid

This is a dotnet implementation of [Segment's K-Sortable Globally Unique Identifiers](https://github.com/segmentio/ksuid).

## overview

tl;dr

- targets .net standard 2.1
- blazing-fast performance
- virtually no memory overhead
- thread-safe and lock-free
- uses crypto PRNG to generate random bits
- unit-tested

## installation

``` sh
$ dotnet add package KsuidDotNet
```

## how to use

Creating a random KSUID `string`:

``` csharp
var id = Ksuid.NewKsuid();
```

```
29faSiN1gPB6IzM74u6tMfTO02L
```

Creating a random KSUID `string` with the prefix `c_`:

``` csharp
var id = Ksuid.NewKsuid("c_");
```

```
c_29faSiN1gPB6IzM74u6tMfTO02L
```

## micro benchmarks

Benchmark results for KsuidDotNet and other KSUID libraries.

## generate random KSUID

```
BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.2.1 (21D62) [Darwin 21.3.0]
Intel Core i7-9750H CPU 2.60GHz, 1 CPU, 6 logical and 6 physical cores
.NET SDK=6.0.202
  [Host]     : .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
  DefaultJob : .NET 6.0.4 (6.0.422.16404), X64 RyuJIT


|                          Method |       Mean |    Error |   StdDev |  Gen 0 | Allocated |
|-------------------------------- |-----------:|---------:|---------:|-------:|----------:|
|            KsuidDotNet_NewKsuid |   443.3 ns |  2.28 ns |  1.90 ns | 0.0124 |      80 B |
| StructKsuid_RandomKsuidToString |   623.6 ns |  2.18 ns |  1.71 ns | 0.0124 |      80 B |
|       DotKsuid_GenerateToString | 4,403.7 ns | 20.28 ns | 17.98 ns | 1.2436 |   7,824 B |
```

```
BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.3.1 (21E258) [Darwin 21.4.0]
Apple M1 Max, 1 CPU, 10 logical and 10 physical cores
.NET SDK=6.0.300
  [Host]     : .NET 6.0.5 (6.0.522.21309), Arm64 RyuJIT
  DefaultJob : .NET 6.0.5 (6.0.522.21309), Arm64 RyuJIT


|                          Method |       Mean |   Error |  StdDev |  Gen 0 | Allocated |
|-------------------------------- |-----------:|--------:|--------:|-------:|----------:|
|            KsuidDotNet_NewKsuid |   443.8 ns | 0.73 ns | 0.68 ns | 0.0381 |      80 B |
| StructKsuid_RandomKsuidToString |   579.8 ns | 1.03 ns | 0.97 ns | 0.0381 |      80 B |
|       DotKsuid_GenerateToString | 3,274.2 ns | 5.27 ns | 4.93 ns | 3.7384 |   7,824 B |
```

## generate random KSUID with prefix

```
BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.3.1 (21E258) [Darwin 21.4.0]
Apple M1 Max, 1 CPU, 10 logical and 10 physical cores
.NET SDK=6.0.300
  [Host]     : .NET 6.0.5 (6.0.522.21309), Arm64 RyuJIT
  DefaultJob : .NET 6.0.5 (6.0.522.21309), Arm64 RyuJIT


|                                    Method |       Mean |   Error |  StdDev |  Gen 0 | Allocated |
|------------------------------------------ |-----------:|--------:|--------:|-------:|----------:|
|                      KsuidDotNet_NewKsuid |   444.5 ns | 0.48 ns | 0.42 ns | 0.0381 |      80 B |
|           StructKsuid_RandomKsuidToString |   579.1 ns | 1.41 ns | 1.32 ns | 0.0381 |      80 B |
|                 DotKsuid_GenerateToString | 3,279.7 ns | 8.14 ns | 7.61 ns | 3.7384 |   7,824 B |
|                                                                                                 |
|           KsuidDotNet_NewKsuid_WithPrefix |   450.8 ns | 0.95 ns | 0.84 ns | 0.0381 |      80 B |
| StructKsuid_RandomKsuidToStringWithPrefix |   592.4 ns | 1.21 ns | 1.07 ns | 0.0763 |     160 B |
|       DotKsuid_GenerateToStringWithPrefix | 3,299.6 ns | 5.80 ns | 5.43 ns | 3.7766 |   7,904 B |
```

## license

This library is open-source software released under the MIT [License](LICENSE).