namespace PdfMerger.Pdf.Matchers.Base;

internal class SignalNumberMatcher : IMatcher
{
    public static readonly SignalNumberMatcher Instance = new();
    private const byte DotToken = (byte)'.';
    private const byte PlusToken = (byte)'+';
    private const byte MinusToken = (byte)'-';
    private const byte ZeroToken = (byte)'0';
    private const byte NineToken = (byte)'9';
    
    private SignalNumberMatcher() {}
    
    public int Match(ReadOnlySpan<byte> bytes)
    {
        var dot = false;
        var index = bytes[0] is PlusToken or MinusToken ? 0 : -1;

        for (var i = index + 1; i < bytes.Length; i++)
        {
            if (!dot && bytes[i] == DotToken)
            {
                dot = true;
                index++;
                continue;
            }
            
            if (bytes[i] is < ZeroToken or > NineToken)
                return index;

            index++;
        }

        return index;
    }
}