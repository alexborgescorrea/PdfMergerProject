using PdfMerger.Pdf.Matchers.Base;

namespace PdfMerger.Pdf.Matchers;

internal class NullMatcher : IMatcher
{
    public static readonly NullMatcher Instance = new();
    
    private static readonly IMatcher[] Matchers = 
    [
        new SequenceMatcher("null"), 
        DelimiterMatcher.Instance
    ];

    private NullMatcher() {}

    public int Match(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length > 0 && bytes[0] == 0)
            return 0;
        
        return Matcher.Match(Matchers, bytes);
    }
}