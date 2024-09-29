namespace PdfMerger.Pdf.Matchers.Base;

internal class DelimiterMatcher : IMatcher
{
    public static readonly DelimiterMatcher Instance = new();
    
    public int Match(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0)
            return -1;
        
        return PdfConstants.Delimiters.Contains(bytes[0]) ? 0 : -1;
    }
}