using PdfMerger.Pdf.Matchers;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types;

internal class ReferenceProcessor : IProcessor
{
    public static readonly ReferenceProcessor Instance = new();
    private const int MaxIdentifierLength = 255;
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        var chunk = await reader.ChunkAsync(MaxIdentifierLength);
        var index = ReferenceMatcher.Instance.Match(chunk.Span);
        if (index == -1)
            return false;

        await context.PdfWriter.WriterLineAsync(chunk[..index]);
        
        return await reader.MoveAsync(index);
    }
}