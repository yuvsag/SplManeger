using FluentAssertions;
using SplManeger.Logic.Packet.Protocol;
using P = SplManeger.Logic.Packet;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SplManeger.Tests.Logic.Protocol;

public class Json_reading
{
    [Fact]
    public void Protocol_from_json()
    {
        string json = File.ReadAllText("./Protocol/test.protocol.json");
        P.Protocol.PacketProtocol protocol = ProtocolReader.FromJson(json);
        P.Packet p = protocol.Decode("01 A4 FF 3B 9D CF 68");
    }
}
