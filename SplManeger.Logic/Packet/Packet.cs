namespace SplManeger.Logic.Packet;

public record Packet
{
    public PacketSection[] Sections { private get; init; } = null!;
    public PacketSection? Section(string name)
    {
        return Sections?.FirstOrDefault(s => s.Name == name);
    }
}

public class PacketSection
{
    public string Name { get; init; }
    public PacketField[] Fields { private get; init; }

    public PacketSection(string name, PacketField[] fields)
    {
        Name = name;
        Fields = fields;
    }
    public object? Field(string name)
    {
        return Fields?.FirstOrDefault(f => f.Name == name)?.Value;
    }
}
public record PacketField(string Name, object Value);