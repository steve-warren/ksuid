# ksuid [![nuget](https://img.shields.io/nuget/v/KsuidDotNet?style=flat-square)](https://www.nuget.org/packages/KsuidDotNet)

This is a dotnet implementation of [Segment's K-Sortable Globally Unique Identifiers](https://github.com/segmentio/ksuid).

## overview

- creates random KSUID strings
- blazing-fast performance
- no additional memory overhead, memory allocation only for string
- thread-safe and lock-free
- uses crypto PRNG to generate random bits
- unit-tested
- targets .net standard 2.1

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

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22621.1555)
13th Gen Intel Core i7-13700K, 1 CPU, 24 logical and 16 physical cores
.NET SDK=7.0.203
  [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
```

|                                    Method |       Mean |   Error |  StdDev |   Gen0 | Allocated |
|------------------------------------------ |-----------:|--------:|--------:|-------:|----------:|
|              KsuidDotNet_NewKsuidToString |   194.8 ns | 0.46 ns | 0.43 ns | 0.0050 |      80 B |
|           StructKsuid_RandomKsuidToString |   380.2 ns | 0.91 ns | 0.85 ns | 0.0048 |      80 B |
|                 DotKsuid_GenerateToString |   412.0 ns | 1.15 ns | 1.08 ns | 0.0839 |    1320 B |
|                    ksuid_GenerateToString | 1,884.6 ns | 7.08 ns | 6.27 ns | 0.4978 |    7824 B |
|           KsuidDotNet_NewKsuid_WithStringPrefix |   196.5 ns | 0.37 ns | 0.35 ns | 0.0050 |      80 B |
| StructKsuid_RandomKsuidToStringWithStringPrefix |   387.2 ns | 0.50 ns | 0.44 ns | 0.0100 |     160 B |
|       DotKsuid_GenerateToStringWithStringPrefix |   421.5 ns | 1.05 ns | 0.98 ns | 0.0892 |    1400 B |
|          ksuid_GenerateToStringWithStringPrefix | 1,881.8 ns | 8.95 ns | 7.94 ns | 0.5035 |    7904 B |


## license

This library is open-source software released under the MIT [License](LICENSE).