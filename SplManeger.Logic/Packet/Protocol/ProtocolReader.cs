using SplManeger.Logic.Packet.FieldConverters;
using SplManeger.Logic.Utils;
using System.ComponentModel;
using System.Text.Json;

namespace SplManeger.Logic.Packet.Protocol;

public static class ProtocolReader
{
    public const string DateFormat = "dd/MM/yyyy";
    public const string DatetimeFormat = "dd/MM/yyyy HH:mm:ss";

    public static PacketProtocol FromJson(string json)
    {
        Schema schema = JsonSerializer.Deserialize<Schema>(json)!;

        int bitStart = 0;
        var numConverter = new NumericConverter();
        var dateConverter = new DateConverter(DateFormat, numConverter);
        var datetimeConverter = new DateConverter(DatetimeFormat, numConverter);

        Schema.Protocol protocol = schema.protocols[0];

        var protocolFields = new List<PacketProtocol.Field>();
        foreach (var dataField in protocol.dataFields)
        {
            int length = dataField.length;
            IPacketFieldConverter converter;
            switch(dataField.type)
            {
                case "int" when dataField.options is not null:
                    var enumicConverter = new EnumicConverter(dataField.options, numConverter);
                    converter = new PacketFieldConverter<string>(enumicConverter, bitStart, length); 
                break;
                case "int":
                    converter = new PacketFieldConverter<long>(numConverter, bitStart, length); 
                break;
                case "date": 
                    converter = new PacketFieldConverter<TimeDisplay>(dateConverter, bitStart, length);
                break;
                case "datetime": 
                    converter = new PacketFieldConverter<TimeDisplay>(datetimeConverter, bitStart, length);
                break;

                default: throw new Exception("TODO");
            }
            var protocolField = new PacketProtocol.Field(dataField.name, dataField.desc, converter);
            protocolFields.Add(protocolField);

            bitStart += length;
        }
        return new PacketProtocol
        {
            Fields = protocolFields.ToArray()
        };
    }

    public class Schema
    {
        public record Segment(string name, int length);
        public record Unit(string name, float a, float b = 0, string symbol = "");
        public record Protocol(int[] id, string name, DataField[] dataFields);
        public record DataField(string name, string? desc, string type, int length, string? unit, Dictionary<string, int>? options);


        public Segment[] segments { get; set; } = null!;
        public Protocol[] protocols { get; set; } = null!;
        public Unit[] units { get; set; } = null!;

    }
}
