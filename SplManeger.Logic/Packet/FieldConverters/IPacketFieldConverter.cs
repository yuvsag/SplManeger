namespace SplManeger.Logic.Packet.FieldConverters;

public interface IPacketFieldConverter<T>
{
    T FromHex(params string[] hexBytes);

    string[] ToHex(T value);
}
public interface IPacketFieldConverter : IPacketFieldConverter<object> { }

public class PacketFieldConverter<T> : IPacketFieldConverter
    where T : notnull
{

    private readonly IPacketFieldConverter<T> _field;
    private readonly int _start;
    private readonly int _length;
    public PacketFieldConverter(IPacketFieldConverter<T> field, int start, int length)
    {
        _field = field;
        _start = start;
        _length = length;
    }

    public object FromHex(string[] hexBytes)
    {
        var hexNeeded = hexBytes.Skip(_start).Take(_length).ToArray();
        return _field.FromHex(hexNeeded);
    }

    public string[] ToHex(object value)
    {
        if (value is not T proper)
            throw new ArgumentException($"value should be of type {typeof(T).Name}");
        return _field.ToHex(proper);
    }
}