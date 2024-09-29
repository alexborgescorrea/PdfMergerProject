using PdfMerger.Pdf.Matchers.Base;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types;

internal class BooleanProcessor : IProcessor
{
    public static readonly BooleanProcessor Instance = new();
    private const int MaxIdentifierLength = 5;
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader)
    {
        var chunk = await reader.ChunkAsync(MaxIdentifierLength);
        var index = BooleanMatcher.Instance.Match(chunk.Span);
        if (index == -1)
            return false;

        return await reader.MoveAsync(index);
    }
}