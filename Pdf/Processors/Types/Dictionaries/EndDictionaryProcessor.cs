using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types.Dictionaries;

internal class EndDictionaryProcessor : IProcessor
{
    public static readonly EndDictionaryProcessor Instance = new();
    
    private static readonly byte[] Tokens = [(byte)'>', (byte)'>'];

    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, IPdfWriter writer)
    {
        var chunk = await reader.ChunkAsync(2);
        if (chunk.Span.SequenceEqual(Tokens) && await reader.MoveAsync(2))
        {
            await writer.WriteEndDictionaryAsync();
            return true;
        }

        return false;
    }
}