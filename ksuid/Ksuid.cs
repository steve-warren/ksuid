using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace KsuidDotNet;

/// <summary>
/// Represents a K-Sortable Unique Identifier.
/// </summary>
public static class Ksuid
{
    private const int BufferSize = 4096;

    private static readonly ThreadLocal<EntropyBuffer> tlsBuffer_ = new(() => new EntropyBuffer());

    /// <summary>
    /// The length of a KSUID when string (base62) encoded.
    /// </summary>
    private const int StringEncodedLength = 27;

    /// <summary>
    /// Timestamp is an uint32.
    /// </summary>
    private const int TimestampLengthInBytes = 4;

    /// <summary>
    /// Payload is 16-bytes.
    /// </summary>
    private const int PayloadLengthInBytes = 16;

    /// <summary>
    /// KSUIDs are 20 bytes when binary encoded.
    /// </summary>
    private const int ByteLength = TimestampLengthInBytes + PayloadLengthInBytes;

    /// <summary>
    /// KSUID's epoch starts more recently so that the 32-bit number space gives a
    /// significantly higher useful lifetime of around 136 years from March 2017.
    /// This number (14e8) was picked to be easy to remember.
    /// </summary>
    private const long KsuidOffsetFromUnixEpochSeconds = 1_400_000_000;

    /// <summary>
    /// The number of seconds at the Unix epoch.
    /// </summary>
    private const long UnixEpochSeconds = 62_135_596_800;

    /// <summary>
    /// The number of seconds at KSUID epoch.
    /// </summary>
    private const long KsuidEpochSeconds = UnixEpochSeconds + KsuidOffsetFromUnixEpochSeconds;

    /// <summary>
    /// The maximum length of the prefix string.
    /// </summary>
    public const int MaxPrefixLength = 5;

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the end of the KSUID epoch expressed in UTC.
    /// </summary>
    public static readonly DateTime MaxTimestamp = new(678305640950000000L, DateTimeKind.Utc);

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the beginning of the KSUID epoch expressed in UTC.
    /// </summary>
    public static readonly DateTime MinTimestamp = new(
        KsuidEpochSeconds * TimeSpan.TicksPerSecond,
        DateTimeKind.Utc
    );

    /// <summary>
    /// The smallest payload value.
    /// </summary>
    public static readonly IReadOnlyList<byte> MinPayload = Array.AsReadOnly(
        Enumerable.Range(0, 16).Select(_ => (byte)0x00).ToArray()
    );

    /// <summary>
    /// The largest payload value.
    /// </summary>
    public static readonly IReadOnlyList<byte> MaxPayload = Array.AsReadOnly(
        Enumerable.Range(0, 16).Select(_ => (byte)0xFF).ToArray()
    );

    /// <summary>
    /// The largest KSUID value encoded in base 62.
    /// </summary>
    public static readonly string MaxString = "aWgEPTl1tmebfsQzFP4bxwgy80V";

    /// <summary>
    /// The smallest KSUID value encoded in base 62.
    /// </summary>
    public static readonly string MinString = "000000000000000000000000000";

    private static ReadOnlySpan<byte> Base62Characters =>
        "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"u8;

    /// <summary>
    /// Generates a random KSUID using the current time expressed in UTC.
    /// </summary>
    /// <returns>A 20-byte/27-character KSUID encoded in Base 62.</returns>
    public static string NewKsuid() => NewKsuid(DateTime.UtcNow);

    /// <summary>
    /// Generates a random KSUID using the specified time expressed in UTC.
    /// </summary>
    /// <param name="utcNow">A DateTime object expressed in UTC.</param>
    /// <returns>A 20-byte/27-character KSUID encoded in Base 62.</returns>
    public static string NewKsuid(DateTime utcNow) => NewKsuid(utcNow, ReadOnlySpan<char>.Empty);

    /// <summary>
    /// Generates a random KSUID using the current time expressed in UTC with a prefix.
    /// </summary>
    /// <param name="prefix">A string of text to prepend the KSUID. The prefix should be short.</param>
    /// <returns>A 20-byte/27-character KSUID encoded in Base 62.</returns>
    public static string NewKsuid(ReadOnlySpan<char> prefix) => NewKsuid(DateTime.UtcNow, prefix);

    /// <summary>
    /// Generates a random KSUID using the current time expressed in UTC with a prefix.
    /// </summary>
    /// <param name="utcTime">A DateTime object in UTC format.</param>
    /// <param name="prefix">A string of text to prepend the KSUID. The prefix should be short.</param>
    /// <returns>A 20-byte/27-character KSUID encoded in Base 62.</returns>
    public static string NewKsuid(DateTime utcTime, ReadOnlySpan<char> prefix)
    {
        var entropy = tlsBuffer_.Value!;
        var rng = entropy.GetNextBytes();
        return NewKsuid(rng, utcTime, prefix);
    }

    /// <summary>
    /// Generates a random KSUID using the specified RNG, time expressed in UTC, and prefix.
    /// </summary>
    /// <param name="rng">An instance of the RandomNumberGenerator class.</param>
    /// <param name="utcTime">A DateTime object in UTC format.</param>
    /// <param name="prefix">A string of text to prepend the KSUID. The prefix should be short.</param>
    /// <returns>A 20-byte/27-character KSUID encoded in Base 62.</returns>
    public static string NewKsuid(
        RandomNumberGenerator rng,
        DateTime utcTime,
        ReadOnlySpan<char> prefix
    )
    {
        Span<byte> rngBytes = stackalloc byte[16];
        rng.GetBytes(rngBytes);

        return NewKsuid(rngBytes, utcTime, prefix);
    }

    private static unsafe string NewKsuid(
        ReadOnlySpan<byte> rngBytes,
        DateTime utcTime,
        ReadOnlySpan<char> prefix
    )
    {
        var totalLength = prefix.Length + StringEncodedLength;

        return string.Create(
            totalLength,
            (
                rngPtr: (IntPtr)Unsafe.AsPointer(ref MemoryMarshal.GetReference(rngBytes)),
                utcTime,
                prefixPtr: (IntPtr)Unsafe.AsPointer(ref MemoryMarshal.GetReference(prefix)),
                prefixLen: prefix.Length
            ),
            FillKsuidBuffer
        );
    }

    private static unsafe void FillKsuidBuffer(
        Span<char> span,
        (IntPtr rngPtr, DateTime utcTime, IntPtr prefixPtr, int prefixLength) state
    )
    {
        // recover the prefix span from the state
        var prefixSpan = new ReadOnlySpan<char>(state.prefixPtr.ToPointer(), state.prefixLength);
        prefixSpan.CopyTo(span);

        // generate binary KSUID (20 bytes)
        Span<byte> binaryBuffer = stackalloc byte[ByteLength];

        // timestamp in big endian
        var timestamp = state.utcTime.Ticks / TimeSpan.TicksPerSecond - KsuidEpochSeconds;
        BinaryPrimitives.WriteUInt32BigEndian(binaryBuffer, (uint)timestamp);

        // rng payload fill
        var rngSpan = new ReadOnlySpan<byte>(state.rngPtr.ToPointer(), PayloadLengthInBytes);
        rngSpan.CopyTo(binaryBuffer.Slice(4));

        // encode directly into the tail of the string span
        EncodeBase62(binaryBuffer, span.Slice(state.prefixLength));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EncodeBase62(ReadOnlySpan<byte> source, Span<char> destination)
    {
        Span<uint> parts = stackalloc uint[5];

        parts[0] = BinaryPrimitives.ReadUInt32BigEndian(source.Slice(0, 4));
        parts[1] = BinaryPrimitives.ReadUInt32BigEndian(source.Slice(4, 4));
        parts[2] = BinaryPrimitives.ReadUInt32BigEndian(source.Slice(8, 4));
        parts[3] = BinaryPrimitives.ReadUInt32BigEndian(source.Slice(12, 4));
        parts[4] = BinaryPrimitives.ReadUInt32BigEndian(source.Slice(16, 4));

        var n = StringEncodedLength;
        var partsLen = 5;

        // get reference to Base62 chars avoiding bounds checks in loop
        ref byte base62Ref = ref MemoryMarshal.GetReference(Base62Characters);

        while (partsLen > 0)
        {
            ulong remainder = 0;
            int nextPartsLen = 0;

            for (int i = 0; i < partsLen; i++)
            {
                // compiles to single `div` instruction on x64/ARM64
                // which returns both quotient and remainder.
                ulong value = parts[i] + (remainder << 32); // remainder * 2^32
                (ulong quotient, ulong rem) = Math.DivRem(value, 62);

                remainder = rem;

                if (nextPartsLen == 0 && quotient == 0)
                    continue;

                // write back to parts array for next pass
                parts[nextPartsLen++] = (uint)quotient;
            }

            partsLen = nextPartsLen;
            n--;

            // map remainder to character
            destination[n] = (char)Unsafe.Add(ref base62Ref, (nint)remainder);
        }

        // pad with leading zeros if we finished early
        while (n > 0)
            destination[--n] = '0';
    }

    private sealed class EntropyBuffer
    {
        private readonly byte[] _buffer = new byte[BufferSize];
        private int _offset = BufferSize; // force refill on first call

        public ReadOnlySpan<byte> GetNextBytes()
        {
            // refill if we don't have enough bytes left
            if (_offset + 16 > BufferSize)
                Refill();

            var slice = new ReadOnlySpan<byte>(_buffer, _offset, 16);
            _offset += 16;
            return slice;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Refill()
        {
            RandomNumberGenerator.Fill(_buffer);
            _offset = 0;
        }
    }
}
