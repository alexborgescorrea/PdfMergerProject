using System.Text;

namespace PdfMerger.Helpers;

internal static class MemoryExtensions
{
    public static string Dump(this Memory<byte> bytes)
    {
        var value = Encoding.ASCII.GetString(bytes.Span);
        Console.WriteLine(value);
        return value;
    }
}