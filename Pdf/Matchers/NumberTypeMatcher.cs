using PdfMerger.Pdf.Matchers.Base;

namespace PdfMerger.Pdf.Matchers;

internal class NumberTypeMatcher : IMatcher
{
    public static readonly NumberTypeMatcher Instance = new();
    
    private static readonly IMatcher[] Matchers = 
    [
        SignalNumberMatcher.Instance,  
        DelimiterMatcher.Instance
    ];

    private NumberTypeMatcher() {}
    
    public int Match(ReadOnlySpan<byte> bytes) => Matcher.Match(Matchers, bytes);
}