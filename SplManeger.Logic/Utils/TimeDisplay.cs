namespace SplManeger.Logic.Utils;


/// <summary>
/// DateTimeOffset with a specific date forma.
/// The format is used via ToString
/// </summary>
public readonly struct TimeDisplay
{
    private readonly DateTimeOffset _value;
    private readonly string _format;
    internal TimeDisplay(DateTimeOffset value, string format)
    {
        _value = value;
        _format = format;
    }
    public override string ToString()
    {
        return _value.ToString(_format);
    }
}
