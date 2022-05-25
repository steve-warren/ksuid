using System.Security.Cryptography;

namespace Ksuid;

public static class KsuidFactory
{
    const string Base62Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    const int TicksPerSecond = 10_000_000;
    const int Base62 = 62;
    const ulong UnsignedIntMaxValue = 4_294_967_296;

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
    const long EpochStamp = 1_400_000_000;

    /// <summary>
    /// The number of seconds at epoch.
    /// </summary>
    const long EpochSeconds = 62_135_596_800;

    const long EpochDelta = EpochSeconds + EpochStamp;

	public static readonly DateTime MaxTimestamp = new(678305640950000000L);

    /// <summary>
    /// Generates a random KSUID using the current time.
    /// </summary>
    /// <returns></returns>
    public static string New()
        => New(RandomNumberGenerator.Create(), DateTime.UtcNow, default); // RNG Create method points to a singleton whose Dispose method no-ops

    /// <summary>
    /// Generates a random KSUID using the current time and optional prefix.
    /// </summary>
    /// <param name="prefix">A string of text to prepend the KSUID. The prefix should be short.</param>
    /// <returns></returns>
    public static string New(ReadOnlySpan<char> prefix = default)
        => New(RandomNumberGenerator.Create(), DateTime.UtcNow, prefix); // RNG Create method points to a singleton whose Dispose method no-ops

    /// <summary>
    /// Generates a random KSUID using the current time and optional prefix.
    /// </summary>
    /// <param name="rng">An instance of the RandomNumberGenerator class.</param>
    /// <param name="prefix">A string of text to prepend the KSUID. The prefix should be short.</param>
    /// <returns></returns>
    public static string New(RandomNumberGenerator rng, ReadOnlySpan<char> prefix = default)
        => New(rng, DateTime.UtcNow, prefix);

	/// <summary>
	/// Generates a random KSUID using the specified time and optional prefix.
	/// </summary>
	/// <param name="rng">An instance of the RandomNumberGenerator class.</param>
	/// <param name="utcTime">A DateTime object in UTC format.</param>
	/// <param name="prefix">A string of text to prepend the KSUID. The prefix should be short.</param>
	/// <returns></returns>
	public static string New(RandomNumberGenerator rng, DateTime utcTime, ReadOnlySpan<char> prefix)
    {
        // allocate 20 bytes to hold the entire KSUID value
        Span<byte> ksuid = stackalloc byte[ByteLength];

        // 16 byte slice of the KSUID array
        // where random bytes will be written
        Span<byte> payloadSlice = ksuid.Slice(TimestampLengthInBytes, PayloadLengthInBytes);

        rng.GetBytes(payloadSlice);

		// converts current ticks into seconds, then
		// subtracts the number of seconds since custom epoch
		long ts = utcTime.Ticks / TicksPerSecond - EpochDelta;

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
        var destLength = StringEncodedLength;
        var partsLength = parts.Length;

        while (partsLength > 0)
        {
            quotient.Clear(); // this call appears to improve performance

            ulong remainder = 0;
            int counter = 0;

            for (var i = 0; i < partsLength; i ++)
            {
                ulong accumulator = parts[i] + remainder * UnsignedIntMaxValue;
                ulong digit = accumulator / Base62;
                remainder = accumulator % Base62;

                if (counter != 0 || digit != 0)
                {
                    quotient[counter] = (uint) digit;
                    counter++;
                }
            }

            destLength--;

            // set the char to the output array
            dest[prefixLength + destLength] = Base62Characters[(int)remainder];

            // copy quotient to parts
            for (var i = 0; i < counter; i ++)
                parts[i] = quotient[i];

            // count the remaining parts
            partsLength = counter;
        }

        // add 0 padding
        for (var i = prefixLength; i < destLength; i ++)
            dest[i] = '0';

        // add prefix
        //_ = prefix[prefixLength - 1]; // opt

        for (var i = 0; i < prefixLength; i ++)
            dest[i] = prefix[i];

        return new string(dest);
    }
}
