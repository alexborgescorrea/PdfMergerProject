namespace PdfMerger.Pdf.Matchers.Base;

internal class StartDelimiterMatcher : IMatcher
{
    public static readonly StartDelimiterMatcher Instance = new();
    
    public int Match(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0)
            return -1;
        
        return PdfConstants.StartDelimiters.Contains(bytes[0]) ? 0 : -1;
    }
}