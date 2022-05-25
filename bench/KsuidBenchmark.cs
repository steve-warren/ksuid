using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

namespace KsuidDotNet.Bench;

[MemoryDiagnoser]
public class KsuidBenchmark
{
    [Benchmark]
    public void KsuidTest()
    {
		_ = Ksuid.NewKsuid(RandomNumberGenerator.Create(), DateTime.UtcNow, "cu_");
	}
}
