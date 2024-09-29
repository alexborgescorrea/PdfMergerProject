namespace PdfMerger.Pdf.Matchers.Base;

internal class EndDelimiterMatcher : IMatcher
{
    public static readonly EndDelimiterMatcher Instance = new();
    
    public int Match(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0)
            return -1;
        
        return PdfConstants.EndDelimiters.Contains(bytes[0]) ? 0 : -1;
    }
}