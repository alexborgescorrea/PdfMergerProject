namespace PdfMerger.Pdf.Matchers.Base;

internal class SpaceMatcher : IMatcher
{
    public static readonly SpaceMatcher Instance = new();

    private SpaceMatcher() { }
    
    public int Match(ReadOnlySpan<byte> bytes)
    {
        var index = bytes.IndexOfAnyExcept(PdfConstants.SpacesSearchValues);
        if (index == -1)
            return index;

        return index - 1;
    }
}