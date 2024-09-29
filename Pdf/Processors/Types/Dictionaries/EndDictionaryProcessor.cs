using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types.Dictionaries;

internal class EndDictionaryProcessor : IProcessor
{
    public static readonly EndDictionaryProcessor Instance = new();
    
    private static readonly byte[] Tokens = [(byte)'>', (byte)'>'];

    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader)
    {
        var chunk = await reader.ChunkAsync(2);

        return chunk.Span.SequenceEqual(Tokens) &&
               await reader.MoveAsync(2);
    }
}