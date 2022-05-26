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
BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.3.1 (21E258) [Darwin 21.4.0]
Apple M1 Max, 1 CPU, 10 logical and 10 physical cores
.NET SDK=6.0.300
  [Host]     : .NET 6.0.5 (6.0.522.21309), Arm64 RyuJIT
  DefaultJob : .NET 6.0.5 (6.0.522.21309), Arm64 RyuJIT


|                                    Method |       Mean |   Error |  StdDev |  Gen 0 | Allocated |
|------------------------------------------ |-----------:|--------:|--------:|-------:|----------:|
|                      KsuidDotNet_NewKsuid |   443.8 ns | 0.41 ns | 0.36 ns | 0.0381 |      80 B |
|           StructKsuid_RandomKsuidToString |   578.1 ns | 0.60 ns | 0.53 ns | 0.0381 |      80 B |
|                 DotKsuid_GenerateToString |   647.7 ns | 0.70 ns | 0.65 ns | 0.6304 |   1,320 B |
|                    ksuid_GenerateToString | 3,276.8 ns | 4.49 ns | 4.20 ns | 3.7384 |   7,824 B |
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
|           KsuidDotNet_NewKsuid_WithPrefix |   451.4 ns | 0.30 ns | 0.27 ns | 0.0381 |      80 B |
| StructKsuid_RandomKsuidToStringWithPrefix |   590.2 ns | 0.79 ns | 0.70 ns | 0.0763 |     160 B |
|       DotKsuid_GenerateToStringWithPrefix |   662.1 ns | 1.47 ns | 1.37 ns | 0.6695 |   1,400 B |
|          ksuid_GenerateToStringWithPrefix | 3,289.6 ns | 4.55 ns | 3.80 ns | 3.7766 |   7,904 B |
```

## license

This library is open-source software released under the MIT [License](LICENSE).