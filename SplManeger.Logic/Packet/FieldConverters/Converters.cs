using SplManeger.Logic.Utils;

namespace SplManeger.Logic.Packet.FieldConverters;

public class NumericConverter : IPacketFieldConverter<long>
{
    public NumericConverter()
    {
    }

    public long FromHex(params string[] hexBytes)
    {
        string bytesJoined = string.Join("", hexBytes);
        int hexBase = 16;
        return Convert.ToInt64(bytesJoined, hexBase);
    }

    public string[] ToHex(long value)
    {
        string hexBytesJoined = value.ToString("X");

        string[] hexBytesSplit = hexBytesJoined.Chunk(2).Select(c => new string(c)).ToArray();
        return hexBytesSplit;
    }
}



public class EnumicConverter : IPacketFieldConverter<string>
{
    private readonly Dictionary<string, int> _enumMap;
    private readonly NumericConverter _numConverter;

    public EnumicConverter(Dictionary<string, int> enumMap, NumericConverter numConverter)
    {
        _enumMap = enumMap;
        _numConverter = numConverter;
    }

    public string FromHex(params string[] hexBytes)
    {
        int value = (int)_numConverter.FromHex(hexBytes);

        if (!_enumMap.ContainsValue(value))
            throw new ArgumentException("The map provided does not contain this key");

        return _enumMap.First(x => x.Value == value).Key;
    }

    public string[] ToHex(string value)
    {
        throw new NotImplementedException();
    }
}



public class DateConverter : IPacketFieldConverter<TimeDisplay>
{
    private readonly string _dateFormat;
    private readonly NumericConverter _numConverter;

    public DateConverter(string dateFormat, NumericConverter numConverter)
    {
        _dateFormat = dateFormat;
        _numConverter = numConverter;
    }

    public TimeDisplay FromHex(params string[] hexBytes)
    {
        var unixSeconds = _numConverter.FromHex(hexBytes);
        var asDate = DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
        return new TimeDisplay(asDate, _dateFormat);
    }

    public string[] ToHex(TimeDisplay value)
    {
        throw new NotImplementedException();
    }
}



public class DateCommandConverter : IPacketFieldConverter<string>
{
    private readonly DateConverter _converter;
    private readonly NumericConverter _numConverter;

    public DateCommandConverter(DateConverter converter, NumericConverter numConverter)
    {
        _converter = converter;
        _numConverter = numConverter;
    }

    public string FromHex(params string[] hexBytes)
    {
        string firstByte = hexBytes[0];

        if (!firstByte.StartsWith("N"))
            return _converter.FromHex(hexBytes).ToString();

        var restOfBytes = hexBytes.Skip(1).ToArray();
        var bytesAsSeconds = _numConverter.FromHex(restOfBytes);

        return firstByte switch
        {
            "NN" => "now",
            "N+" => $"now+{bytesAsSeconds}",
            "N-" => $"now-{bytesAsSeconds}",
            _ => throw new ArgumentException("first byte does not follow convention"),
        };
    }

    public string[] ToHex(string value)
    {
        throw new NotImplementedException();
    }
}


/// <summary>
/// Convrts a date command (from dbHex or user input) to a regular date, computing the command.
/// </summary>
public class ComputedDateConverter : IPacketFieldConverter<TimeDisplay>
{
    private readonly DateConverter _converter;
    private readonly NumericConverter _numConverter;
    private readonly IAppClock _clock;

    public ComputedDateConverter(DateConverter converter, IAppClock clock, NumericConverter numConverter)
    {
        _converter = converter;
        _clock = clock;
        _numConverter = numConverter;
    }


    //Cases:
    //1. FA BB CD 44 {just the date in Unix} -> Date
    //2. NN 00 00 00 -> Date(now)
    //3. N+ 00 AB 34 -> Date(now plus some secs)
    //4. N- 00 AB 34 -> Date(now minus some secs)

    public TimeDisplay FromHex(params string[] hexBytes)
    {
        string firstByte = hexBytes[0];

        //case 1
        if (!firstByte.StartsWith("N"))
            return _converter.FromHex(hexBytes);

        var restOfBytes = hexBytes.Skip(1).ToArray();
        var bytesAsSeconds = _numConverter.FromHex(restOfBytes);
        var secondsAsSpan = TimeSpan.FromSeconds(bytesAsSeconds);

        //case 2 as base
        var now = _clock.UtcNow();

        // case 3
        if (firstByte == "N+")
            now = now.Add(secondsAsSpan);
        //case 4
        if (firstByte == "N-")
            now = now.Subtract(secondsAsSpan);

        var nowAsHex = _numConverter.ToHex(now.ToUnixTimeSeconds());
        return _converter.FromHex(nowAsHex);
    }

    //Cases:
    //"now"
    //"now+{seconds}"
    //"now-{seconds}"
    //"25/04/2001" (short date)
    //"25/04/2001 14:54:23" (long date)
    // will all return a date in unix seconds as hex array

    public string[] ToHex(TimeDisplay value)
    {
        throw new NotImplementedException();
    }
}