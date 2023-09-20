using SplManeger.Logic.Packet;
using SplManeger.Logic.Packet.FieldConverters;
using SplManeger.Tests.Logic.Utils;

namespace SplManeger.Tests.Logic.Packet;

public class From_hex_conversion : TestClass
{
    [Fact]
    public void Numeric()
    {
        var converter = new NumericConverter();

        var value = converter.FromHex("01", "A4");

        value.ToString().Should().Be("420");
    }
    [Fact]
    public void Reversed()
    {
        var converter = new NumericConverter();
        var reversed = new PacketFieldConverter<long>(converter, 0, 2).ReversedBytes();

        var value = reversed.FromHex("A4", "01");

        value.ToString().Should().Be("420");
    }

    [Fact]
    public void Date()
    {
        var converter = new DateConverter("M/dd", new());

        var value = converter.FromHex("3B", "9D", "CF", "68"); // 9/11 in Unix seconds

        value.ToString().Should().Be("9/11");
    }

    [Fact]
    public void Date_command()
    {
        //"date command", 0, 2, 
        DateCommandConverter converter = new(new("M/dd HH:mm:ss", new()), new());


        var result1 = converter.FromHex("NN", "69");//'now' (UTC). numbers shouldn't matter
        result1.Should().Be("now");

        var result2 = converter.FromHex("N+", "1C");//'now' plus 28 sec
        result2.Should().Be("now+28");

        var result3 = converter.FromHex("N-", "20");//'now' minus 32 sec
        result3.Should().Be("now-32");
    }

    [Fact]
    public void Date_from_command()
    {
        ComputedDateConverter converter = new(new("M/dd HH:mm:ss", new()), FrozenClock.Default, new());


        var result1 = converter.FromHex("NN", "69");//current time (UTC). '69' shouldn't do anything
        result1!.ToString().Should().Be("9/11 13:46:32"); // the clock is frozen at this time

        var result2 = converter.FromHex("N+", "1C");//current time plus 28 sec
        result2!.ToString().Should().Be("9/11 13:47:00");

        var result3 = converter.FromHex("N-", "20");//current time minus 32 sec
        result3!.ToString().Should().Be("9/11 13:46:00");
    }

    [Fact]
    public void Enumic_map()
    {
        var converter = new EnumicConverter(on_off_map, new());

        var value = converter.FromHex("01");
        value.Should().Be("ON");
    }
}