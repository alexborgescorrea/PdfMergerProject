using System.Buffers;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors;

internal class XRefProcessor : IProcessor
{
    public static readonly XRefProcessor Instance = new();
    private static readonly byte[] Token = "xref"u8.ToArray();
    private static readonly byte[] TrailerToken = "trailer"u8.ToArray();
    private static readonly SearchValues<byte> FilterSearchValues = SearchValues.Create([(byte)'t', (byte)'%']);
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (!await reader.StartWithAsync(Token))
            return false;

        while (await reader.FindAnyAndMoveAsync(FilterSearchValues))
        {
            if (await reader.StartWithAsync(TrailerToken))
                return true;

            if (!await reader.NextLineAsync())
                return false;
        }

        return false;
    }
}