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

## license

This library is open-source software released under the MIT [License](LICENSE).