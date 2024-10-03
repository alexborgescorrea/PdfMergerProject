using System.Buffers;

namespace PdfMerger.Pdf;

internal static class PdfConstants
{
    public static byte[] PdfHeaderSignature = [0x25, 0x50, 0x44, 0x46, 0x2d];
    public static byte[] Spaces = [0x00, 0x09, 0x0A, 0x0C, 0x0D, 0x20, 0x40];
    public static byte[] DelimitersWithoutSpaces = [(byte)'(', (byte)'<', (byte)'[', (byte)'{', (byte)'/', (byte)'%', (byte)')', (byte)'>', (byte)']', (byte)'}'];
    public static byte[] Delimiters = [..Spaces, ..DelimitersWithoutSpaces];
    public static byte[] StartDelimiters = [..Spaces, (byte)'(', (byte)'<', (byte)'[', (byte)'{', (byte)'/', (byte)'%'];
    public static byte[] EndDelimiters = [..Spaces, (byte)')', (byte)'>', (byte)']', (byte)'}', (byte)'/', (byte)'%'];
    public static byte[] Numbers = [(byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9'];
    public static byte NewLine = 10;
    
    public static SearchValues<byte> SpacesSearchValues = SearchValues.Create(Spaces);
}