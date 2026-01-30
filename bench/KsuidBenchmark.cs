using BenchmarkDotNet.Attributes;

namespace KsuidDotNet.Bench;

[MemoryDiagnoser]
public class KsuidBenchmark
{
    [Benchmark]
    public string KsuidDotNet_NewKsuid()
    {
        return KsuidDotNet.Ksuid.NewKsuid();
    }

    [Benchmark]
    public string KsuidDotNet_NewKsuid_WithPrefix()
    {
        return KsuidDotNet.Ksuid.NewKsuid("c_");
    }

    [Benchmark]
    public string StructKsuid_RandomKsuidToString()
    {
        return StructKsuid.Ksuid.RandomKsuid().ToString();
    }

    [Benchmark]
    public string StructKsuid_RandomKsuidToStringWithPrefix()
    {
        return "c_" + StructKsuid.Ksuid.RandomKsuid().ToString();
    }

    [Benchmark]
    public string DotKsuid_GenerateToString()
    {
        return DotKsuid.Ksuid.NewKsuid().ToString();
    }

    [Benchmark]
    public string DotKsuid_GenerateToStringWithPrefix()
    {
        return "c_" + DotKsuid.Ksuid.NewKsuid().ToString();
    }

    [Benchmark]
    public string ksuid_GenerateToString()
    {
        return KSUID.Ksuid.Generate().ToString();
    }

    [Benchmark]
    public string ksuid_GenerateToStringWithPrefix()
    {
        return "c_" + KSUID.Ksuid.Generate().ToString();
    }
}
