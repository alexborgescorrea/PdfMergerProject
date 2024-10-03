using PdfMerger.Pdf.Matchers;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types;

internal class BooleanProcessor : IProcessor
{
    public static readonly BooleanProcessor Instance = new();
    private const int MaxIdentifierLength = 6;
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, IPdfWriter writer)
    {
        var chunk = await reader.ChunkAsync(MaxIdentifierLength);
        var index = BooleanMatcher.Instance.Match(chunk.Span);
        if (index == -1)
            return false;

        await writer.WriteBooleanAsync(chunk.Span[0]);

        return await reader.MoveAsync(index);
    }
}