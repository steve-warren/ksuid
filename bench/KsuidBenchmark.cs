using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

namespace KsuidDotNet.Bench;

[MemoryDiagnoser]
public class KsuidBenchmark
{
    [Benchmark]
    public void KsuidDotNet_NewKsuid()
    {
		_ = KsuidDotNet.Ksuid.NewKsuid();
	}

    [Benchmark]
    public void StructKsuid_RandomKsuidToString()
    {
		_ = StructKsuid.Ksuid.RandomKsuid().ToString();
	}

    [Benchmark]
    public void DotKsuid_GenerateToString()
    {
		_ = KSUID.Ksuid.Generate().ToString();
	}
}
