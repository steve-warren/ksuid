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
		_ = KsuidFactory.New("cu_");
	}
}
