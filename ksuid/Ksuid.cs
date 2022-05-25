using System.Security.Cryptography;

namespace KsuidDotNet;

public static class Ksuid
{
    const string Base62Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    const int DestBase = 62;
    const ulong SourceBase = 4_294_967_296;

    /// <summary>
    /// The length of a KSUID when string (base62) encoded
    /// </summary>
    const int StringEncodedLength = 27;

    /// <summary>
    /// Timestamp is a uint32.
    /// </summary>
    const int TimestampLengthInBytes = 4;

    /// <summary>
    /// Payload is 16-bytes.
    /// </summary>
    const int PayloadLengthInBytes = 16;

    /// <summary>
    /// KSUIDs are 20 bytes when binary encoded.
    /// </summary>
    const int ByteLength = TimestampLengthInBytes + PayloadLengthInBytes;

    /// <summary>
    /// KSUID's epoch starts more recently so that the 32-bit number space gives a
    /// significantly higher useful lifetime of around 136 years from March 2017.
    /// This number (14e8) was picked to be easy to remember.
    /// </summary>
    const long KsuidOffsetFromUnixEpochSeconds = 1_400_000_000;

    /// <summary>
    /// The number of seconds at Unix epoch.
    /// </summary>
    const long UnixEpochSeconds = 62_135_596_800;

    const long KsuidEpochSeconds = UnixEpochSeconds + KsuidOffsetFromUnixEpochSeconds;

    public const int MaxPrefixLength = 4;

    public static readonly DateTime MaxTimestamp = new(678305640950000000L, DateTimeKind.Utc);

    public static readonly DateTime MinTimestamp = new(KsuidEpochSeconds * TimeSpan.TicksPerSecond, DateTimeKind.Utc);

    /// <summary>
    /// Generates a random KSUID using the current time.
    /// </summary>
    /// <returns></returns>
    public static string NewKsuid() => NewKsuid(DateTime.UtcNow);

    /// <summary>
    /// Generates a random KSUID using the specified time.
    /// </summary>
    /// <param name="utcTime">A DateTime object in UTC format.</param>
    public static string NewKsuid(DateTime utcNow) => NewKsuid(utcNow, default);

    /// <summary>
    /// Generates a random KSUID using the current time and prefix.
    /// </summary>
    /// <param name="prefix">A string of text to prepend the KSUID. The prefix should be short.</param>
    /// <returns>A 20-byte KSUID encoded in Base 62 format.</returns>
    public static string NewKsuid(ReadOnlySpan<char> prefix) => NewKsuid(DateTime.UtcNow, prefix);

    /// <summary>
    /// Generates a random KSUID using the current time and prefix.
    /// </summary>
    /// <param name="utcTime">A DateTime object in UTC format.</param>
    /// <param name="prefix">A string of text to prepend the KSUID. The prefix should be short.</param>
    /// <returns>A 20-byte KSUID encoded in Base 62 format.</returns>
    public static string NewKsuid(DateTime utcTime, ReadOnlySpan<char> prefix)
        => NewKsuid(RandomNumberGenerator.Create(), utcTime, prefix);

    /// <summary>
    /// Generates a random KSUID using the specified RNG, time, and prefix.
    /// </summary>
    /// <param name="rng">An instance of the RandomNumberGenerator class.</param>
    /// <param name="utcTime">A DateTime object in UTC format.</param>
    /// <param name="prefix">A string of text to prepend the KSUID. The prefix should be short.</param>
    /// <returns>A 20-byte KSUID encoded in Base 62 format.</returns>
    public static string NewKsuid(RandomNumberGenerator rng!!, DateTime utcTime, ReadOnlySpan<char> prefix)
    {
        if (utcTime.Kind is not DateTimeKind.Utc)
            throw new ArgumentException("Parameter is not in UTC.", nameof(utcTime));
        
        if (utcTime < MinTimestamp || MaxTimestamp < utcTime)
            throw new ArgumentOutOfRangeException(nameof(utcTime), "Timestamp out of range.");

        if (prefix.Length > MaxPrefixLength)
            throw new ArgumentOutOfRangeException(nameof(prefix), $"Prefix length should be fewer than {MaxPrefixLength} characters in length.");

        // allocate 20 bytes to hold the entire KSUID value
        Span<byte> ksuid = stackalloc byte[ByteLength];

        // write 16 random bytes to payload slice; this call is thread-safe.
        rng.GetBytes(ksuid.Slice(TimestampLengthInBytes, PayloadLengthInBytes));

        // converts current ticks into seconds, then
        // subtracts the number of seconds since custom epoch
        long ts = utcTime.Ticks / TimeSpan.TicksPerSecond - KsuidEpochSeconds;

        // stores the time value in the first 4
        // bytes of the KSUID, encoding it into big endian
        ksuid[0] = (byte) (ts >> 24);
        ksuid[1] = (byte) (ts >> 16);
        ksuid[2] = (byte) (ts >> 8);
        ksuid[3] = (byte) ts;

        var prefixLength = prefix.Length;

        // stores the full KSUID as a char array
        Span<char> dest = stackalloc char[prefixLength + StringEncodedLength];

        // encodes the KSUID into base 62
        Span<uint> quotient = stackalloc uint[5];
        Span<uint> parts = stackalloc uint[5]
        {
            ksuid[3] | (uint)ksuid[2] << 8 | (uint)ksuid[1] << 16 | (uint)ksuid[0] << 24,
            ksuid[7] | (uint)ksuid[6] << 8 | (uint)ksuid[5] << 16 | (uint)ksuid[4] << 24,
            ksuid[11] | (uint)ksuid[10] << 8 | (uint)ksuid[9] << 16 | (uint)ksuid[8] << 24,
            ksuid[15] | (uint)ksuid[14] << 8 | (uint)ksuid[13] << 16 | (uint)ksuid[12] << 24,
            ksuid[19] | (uint)ksuid[18] << 8 | (uint)ksuid[17] << 16 | (uint)ksuid[16] << 24
        };
        
        // the length of the KSUID is always 27 chars
        var n = StringEncodedLength;
        var partsLength = parts.Length;

        while (partsLength > 0)
        {
            // calls to Span<uint>.Clear()
            // appear to improve performance
            quotient.Clear();

            ulong remainder = 0;
            int quotientLength = 0;

            for (var i = 0; i < partsLength; i ++)
            {
                ulong value = parts[i] + remainder * SourceBase;
                ulong digit = value / DestBase;
                remainder = value % DestBase;

                if (quotientLength != 0 || digit != 0)
                {
                    quotient[quotientLength] = (uint) digit;
                    quotientLength++;
                }
            }

            // Writes at the end of the destination buffer because we computed the
            // lowest bits first.
            n--;

            // set the char to the output array
            dest[prefixLength + n] = Base62Characters[(int)remainder];

            // copy quotient to parts
            for (var i = 0; i < quotientLength; i ++)
                parts[i] = quotient[i];

            // count the remaining parts
            partsLength = quotientLength;
        }

        // Add padding at the head of the destination buffer for all bytes that were
        // not set.
        while(n-- > 0)
            dest[prefixLength + n] = '0';

        while(prefixLength-- > 0)
            dest[prefixLength] = prefix[prefixLength];

        return new string(dest);
    }
}
