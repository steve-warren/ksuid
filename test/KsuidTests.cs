using Xunit;
using FluentAssertions;

namespace KsuidDotNet.Tests;

public class KsuidTests
{
    readonly byte[] MinPayload = Enumerable.Range(0, 16).Select(_ => (byte)0x00).ToArray();

    readonly byte[] MaxPayload = Enumerable.Range(0, 16).Select(_ => (byte)0xFF).ToArray();

    [Fact]
    public void MinKsuidValue()
    {
        var id = Ksuid.NewKsuid(new RngStub(MinPayload), Ksuid.MinTimestamp, string.Empty);

        id.Should().Be("000000000000000000000000000", because: "it is the minimum value for a KSUID.");
    }

    [Fact]
    public void MaxKsuidValue()
    {
        var id = Ksuid.NewKsuid(new RngStub(MaxPayload), Ksuid.MaxTimestamp, string.Empty);

        id.Should().Be("aWgEPTl1tmebfsQzFP4bxwgy80V", because: "it is the maximum value for a KSUID.");
    }

    [Fact]
    public void Given_Ksuid_When_TimestampOverflows_Then_Throw()
    {
        FluentActions.Invoking(() => Ksuid.NewKsuid(new RngStub(MinPayload), Ksuid.MaxTimestamp.AddSeconds(1), string.Empty)).Should().Throw<ArgumentOutOfRangeException>(because: "the timestamp overflowed.");
    }

    [Fact]
    public void Given_Ksuid_When_TimestampUderflows_Then_Throw()
    {
        FluentActions.Invoking(() => Ksuid.NewKsuid(new RngStub(MinPayload), Ksuid.MinTimestamp.AddSeconds(-1), string.Empty)).Should().Throw<ArgumentOutOfRangeException>(because: "the timestamp underflowed.");
    }
}