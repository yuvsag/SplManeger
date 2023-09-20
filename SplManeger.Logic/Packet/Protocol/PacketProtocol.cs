using System.Collections.Specialized;
using SplManeger.Logic.Packet.FieldConverters;

namespace SplManeger.Logic.Packet.Protocol;

public class PacketProtocol
{
    public record Field(string Name, string? Description,IPacketFieldConverter Converter)
    {
        public PacketField FromHex(string[] hexBytes)
        {
            return new PacketField(Name, Converter.FromHex(hexBytes));
        }
    }



    public Field[] Fields{ private get; init; } = null!;
    public Packet Decode(string hex)
    {
        var hexBytes = hex.Split(' ');
        var packetSections = new List<PacketSection>();

        //Data fields
        var packetFields = new List<PacketField>();
        foreach (var field in Fields)
        {
            packetFields.Add(field.FromHex(hexBytes));
        }
        packetSections.Add(new("Data", packetFields.ToArray()));

        return new()
        {
            Sections = packetSections.ToArray()
        };
    }
}