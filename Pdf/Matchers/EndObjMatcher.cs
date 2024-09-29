using PdfMerger.Pdf.Matchers.Base;

namespace PdfMerger.Pdf.Matchers;

internal class EndObjMatcher : IMatcher
{
    public static readonly EndObjMatcher Instance = new();
    
    private static readonly IMatcher[] Matchers = 
    [
        EndDelimiterMatcher.Instance, 
        new SequenceMatcher("endobj"), 
        StartDelimiterMatcher.Instance
    ];

    private EndObjMatcher() {}
    
    public int Match(ReadOnlySpan<byte> bytes) => Matcher.Match(Matchers, bytes);
}