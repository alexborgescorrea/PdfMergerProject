namespace PdfMerger.Pdf.Matchers.Base;

internal class NumberMatcher : IMatcher
{
    public static readonly NumberMatcher Instance = new();
    
    private NumberMatcher() {}
    
    public int Match(ReadOnlySpan<byte> bytes)
    {
        var index = -1;
        for (var i = 0; i < bytes.Length; i++)
        {
            if (bytes[i] is < 0x2f or > 0x3A)
                return index;

            index++;
        }

        return index;
    }
}