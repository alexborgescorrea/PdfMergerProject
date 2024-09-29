namespace PdfMerger.Pdf.Matchers.Base;

internal static class Matcher
{
    public static int Match(IMatcher[] matchers, ReadOnlySpan<byte> bytes)
    {
        var index = 0;
        for (var i = 0; i < matchers.Length; i++)
        {
            var matcher = matchers[i];
            var nextIndex = matcher.Match(bytes);
            if (nextIndex == -1)
                return -1;

            nextIndex++;
            index += nextIndex;
            
            if (i == matchers.Length - 1)
                return index -1;
            
            if (nextIndex == bytes.Length)
                return -1;
            
            bytes = bytes[nextIndex..];
        }

        return -1;
    }
}