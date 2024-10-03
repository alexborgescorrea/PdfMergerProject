using PdfMerger.Pdf.Matchers;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types;

internal class ReferenceProcessor : IProcessor
{
    public static readonly ReferenceProcessor Instance = new();
    private const int MaxIdentifierLength = 255;
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, IPdfWriter writer)
    {
        var chunk = await reader.ChunkAsync(MaxIdentifierLength);
        var index = ReferenceMatcher.Instance.Match(chunk.Span);
        if (index == -1)
            return false;

        await writer.WriteLineAsync(chunk[..index]);
        
        return await reader.MoveAsync(index);
    }
}