namespace PdfMerger.Pdf.Matchers.Base;

internal class BooleanMatcher : IMatcher
{
    public static readonly BooleanMatcher Instance = new();
    
    private static readonly IMatcher[] Matchers = 
    [
        DelimiterMatcher.Instance, 
        new OrMatcher("true", "false"), 
        DelimiterMatcher.Instance
    ];

    private BooleanMatcher() {}
    
    public int Match(ReadOnlySpan<byte> bytes) => Matcher.Match(Matchers, bytes);
}