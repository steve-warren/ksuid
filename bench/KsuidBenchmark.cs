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

    [Benchmark]
    public void KsuidDotNet_NewKsuid_WithPrefix()
    {
        _ = KsuidDotNet.Ksuid.NewKsuid("c_");
    }

    [Benchmark]
    public void StructKsuid_RandomKsuidToStringWithPrefix()
    {
        _ = "c_" + StructKsuid.Ksuid.RandomKsuid().ToString();
    }

    [Benchmark]
    public void DotKsuid_GenerateToStringWithPrefix()
    {
        _ = "c_" + KSUID.Ksuid.Generate().ToString();
    }
}
