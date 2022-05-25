using Xunit;
using FluentAssertions;

namespace KsuidDotNet.Tests;

public class KsuidTests
{
    readonly byte[] MinPayload = Enumerable.Range(0, 16).Select(_ => (byte)0x00).ToArray();

    readonly byte[] MaxPayload = Enumerable.Range(0, 16).Select(_ => (byte)0xFF).ToArray();

    [Fact]
    public void GivenMinPayloadAndMinTimestamp_When_GeneratingNewKsuid_ReturnMinString()
    {
        var id = Ksuid.NewKsuid(new RngStub(MinPayload), Ksuid.MinTimestamp, string.Empty);

        id.Should().Be("000000000000000000000000000", because: "it is the minimum value for a KSUID.");
    }

    [Fact]
    public void GivenMaxPayloadAndMaxTimestamp_When_GeneratingNewKsuid_ReturnMaxString()
    {
        var id = Ksuid.NewKsuid(new RngStub(MaxPayload), Ksuid.MaxTimestamp, string.Empty);

        id.Should().Be("aWgEPTl1tmebfsQzFP4bxwgy80V", because: "it is the maximum value for a KSUID.");
    }

    [Fact]
    public void Given_OverflowedTimestamp_When_GeneratingNewKsuid_Then_Throw()
    {
        FluentActions.Invoking(() => Ksuid.NewKsuid(Ksuid.MaxTimestamp.AddSeconds(1))).Should().Throw<ArgumentOutOfRangeException>(because: "the timestamp overflowed.");
    }

    [Fact]
    public void Given_UnderflowedTimestamp_When_GeneratingNewKsuid_Then_Throw()
    {
        FluentActions.Invoking(() => Ksuid.NewKsuid(Ksuid.MinTimestamp.AddSeconds(-1))).Should().Throw<ArgumentOutOfRangeException>(because: "the timestamp underflowed.");
    }

    [Fact]
    public void Given_Prefix_When_GeneratingNewKsuid_Then_ReturnKsuidWithPrefix()
    {
        var prefix = "aaa_";

        var id = Ksuid.NewKsuid(prefix);

        id.Should().StartWith(prefix, because: "it should have a prefix.");
    }

    [Fact]
    public void Given_TooLongPrefix_When_GeneratingNewKsuid_Then_Throw()
    {
        var prefix = "a".PadRight(Ksuid.MaxPrefixLength + 1);

        FluentActions.Invoking(() => Ksuid.NewKsuid(prefix)).Should().Throw<ArgumentOutOfRangeException>(because: "the prefix is too long.");
    }
}