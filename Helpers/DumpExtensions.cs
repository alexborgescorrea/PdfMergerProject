using System.Text;

namespace PdfMerger.Helpers;

internal static class DumpExtensions
{
    public static string Dump(this ReadOnlyMemory<byte> bytes)
    {
        var value = Encoding.ASCII.GetString(bytes.Span);
        Console.WriteLine(value);
        return value;
    }

    public static string Dump(this ReadOnlySpan<byte> bytes)
    {
        var value = Encoding.ASCII.GetString(bytes);
        Console.WriteLine(value);
        return value;
    }
}