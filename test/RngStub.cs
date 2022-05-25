using System.Security.Cryptography;

namespace KsuidDotNet.Tests;

public class RngStub : RandomNumberGenerator
{
    private readonly byte[] _data;

    public RngStub(byte[] data)
    {
        _data = data;
    }

    public override void GetBytes(byte[] data)
    {
        for (var i = 0; i < data.Length; i++)
            data[i] = _data[i];
    }
}