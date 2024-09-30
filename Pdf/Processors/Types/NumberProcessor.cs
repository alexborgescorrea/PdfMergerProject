using PdfMerger.Pdf.Matchers;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types;

internal class NumberProcessor : IProcessor
{
    public static readonly NumberProcessor Instance = new();
    private const int MaxIdentifierLength = 255;
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        var chunk = await reader.ChunkAsync(MaxIdentifierLength);
        var index = NumberTypeMatcher.Instance.Match(chunk.Span);
        if (index == -1)
            return false;

        return await reader.MoveAsync(index);
    }
}