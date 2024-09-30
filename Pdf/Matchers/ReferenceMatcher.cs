using PdfMerger.Pdf.Matchers.Base;

namespace PdfMerger.Pdf.Matchers;

internal class ReferenceMatcher : IMatcher
{
    public static readonly ReferenceMatcher Instance = new();
    
    private static readonly IMatcher[] Matchers = 
    [
        NumberMatcher.Instance,  
        SpaceMatcher.Instance,
        NumberMatcher.Instance,
        SpaceMatcher.Instance,
        new SequenceMatcher("R"),
        DelimiterMatcher.Instance 
    ];

    private ReferenceMatcher() {}
    
    public int Match(ReadOnlySpan<byte> bytes) => Matcher.Match(Matchers, bytes);
}