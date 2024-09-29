using PdfMerger.Pdf.Matchers.Base;

namespace PdfMerger.Pdf.Matchers;

internal class StartObjMatcher : IMatcher
{
    public static readonly StartObjMatcher Instance = new();
    
    private static readonly IMatcher[] Matchers = 
    [
        EndDelimiterMatcher.Instance,
        NumberMatcher.Instance, 
        SpaceMatcher.Instance, 
        NumberMatcher.Instance, 
        SpaceMatcher.Instance, 
        new SequenceMatcher("obj"), 
        StartDelimiterMatcher.Instance
    ];

    private StartObjMatcher() {}
    
    public int Match(ReadOnlySpan<byte> bytes) => Matcher.Match(Matchers, bytes);
}