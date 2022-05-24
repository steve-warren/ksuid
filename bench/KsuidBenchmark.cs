using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

namespace Ksuid.Bench;

[MemoryDiagnoser]
public class KsuidBenchmark
{
    readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    [Benchmark]
    public void KsuidTest()
    {
        ReadOnlySpan<char> prefix = "cu_".AsSpan();
        _ = KsuidFactory.New(_rng, DateTime.UtcNow, prefix);
    }
}
