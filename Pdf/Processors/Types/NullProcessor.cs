using PdfMerger.Pdf.Matchers;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types;

internal class NullProcessor : IProcessor
{
    public static readonly NullProcessor Instance = new();
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (reader.Value == 0)
            return await reader.MoveAsync(1);
        
        var chunk = await reader.ChunkAsync(5);
        var index = NullMatcher.Instance.Match(chunk.Span);
        if (index == -1)
            return false;

        if (index > 0)
            await writer.WriteNull();
        else
            await writer.WriteNullByte();
        
        return await reader.MoveAsync(index);
    }
}