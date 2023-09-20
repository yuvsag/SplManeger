namespace SplManeger.Logic.Packet.FieldConverters;


public static class ConvertersExtentions
{
    /// <summary>
    /// Applies a reverse to the hex array before/after conversion
    /// </summary>
    public static IPacketFieldConverter ReversedBytes(this IPacketFieldConverter converter)
    {
        return new Reverser(converter);
    }

    public class Reverser : IPacketFieldConverter
    {
        private readonly IPacketFieldConverter _converter;
        public Reverser(IPacketFieldConverter converter)
        {
            _converter = converter;
        }

        public object FromHex(params string[] hexBytes)
        {
            string[] reversedBytes = hexBytes.Reverse().ToArray();
            return _converter.FromHex(reversedBytes);
        }

        public string[] ToHex(object value)
        {
            return _converter.ToHex(value).Reverse().ToArray();
        }
    }
}
