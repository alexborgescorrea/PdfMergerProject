using PdfMerger.Pdf.Matchers.Base;

namespace PdfMerger.Pdf.Matchers;

internal class BooleanMatcher : IMatcher
{
    public static readonly BooleanMatcher Instance = new();
    
    private static readonly IMatcher[] Matchers = 
    [
        new OrMatcher("true", "false"), 
        DelimiterMatcher.Instance
    ];

    private BooleanMatcher() {}
    
    public int Match(ReadOnlySpan<byte> bytes) => Matcher.Match(Matchers, bytes);
}