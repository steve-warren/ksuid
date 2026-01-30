using Xunit;
using FluentAssertions;

namespace KsuidDotNet.Tests;

public class KsuidTests
{
    [Fact]
    public void GivenMinPayloadAndMinTimestamp_When_GeneratingNewKsuid_ReturnMinString()
    {
        var id = Ksuid.NewKsuid(new RngStub(Ksuid.MinPayload), Ksuid.MinTimestamp, string.Empty);

        id.Should().Be(Ksuid.MinString, because: "it is the minimum value for a KSUID.");
    }

    [Fact]
    public void GivenMaxPayloadAndMaxTimestamp_When_GeneratingNewKsuid_ReturnMaxString()
    {
        var id = Ksuid.NewKsuid(new RngStub(Ksuid.MaxPayload), Ksuid.MaxTimestamp, string.Empty);

        id.Should().Be(Ksuid.MaxString, because: "it is the maximum value for a KSUID.");
    }

    [Fact]
    public void Given_Prefix_When_GeneratingNewKsuid_Then_ReturnKsuidWithPrefix()
    {
        var prefix = "aaa_";

        var id = Ksuid.NewKsuid(prefix);

        id.Should().StartWith(prefix, because: "it should have a prefix.");
    }
}