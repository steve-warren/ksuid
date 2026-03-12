using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace KsuidDotNet.Bench;

[MemoryDiagnoser]
[HideColumns("Error", "StdDev", "RatioSD")] // Clean up the output
[MinColumn, MaxColumn, OperationsPerSecond]
[SimpleJob]
public class KsuidPerformanceBenchmarks
{
  private readonly DateTime _timestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
  private readonly byte[] _entropy = new byte[16];
  private readonly string _prefix = "test_";

  [GlobalSetup]
  public void Setup()
  {
    RandomNumberGenerator.Fill(_entropy);
  }

  [Benchmark(Description = "NewKsuid() - Real World")]
  public string RealWorldGeneration()
  {
    return Ksuid.NewKsuid();
  }

  [Benchmark(Description = "NewKsuid(Prefix) - Real World")]
  public string RealWorldGenerationWithPrefix()
  {
    return Ksuid.NewKsuid(_prefix);
  }

  [Benchmark(Description = "NewKsuid(DateTime, Prefix) - Deterministic")]
  public string DeterministicGeneration()
  {
    return Ksuid.NewKsuid(_timestamp, _prefix);
  }
}
