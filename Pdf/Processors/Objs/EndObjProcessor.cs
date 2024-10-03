using PdfMerger.Pdf.Matchers;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Objs;

internal class EndObjProcessor : IProcessor
{
    public static readonly EndObjProcessor Instance = new();
    private const int MaxIdentifierLength = 255;
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, IPdfWriter writer)
    {
        var chunk = await reader.ChunkAsync(MaxIdentifierLength);
        var index = EndObjMatcher.Instance.Match(chunk.Span);
        if (index == -1)
            return false;
        
        await writer.WriteEndObjAsync();
        return await reader.MoveAsync(index);
    }
}