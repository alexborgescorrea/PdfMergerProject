using PdfMerger.Pdf.Matchers;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Objs;

internal class EndObjProcessor : IProcessor
{
    public static readonly EndObjProcessor Instance = new();
    private const int MaxIdentifierLength = 255;

    public Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        return MoveNextTokenAsync(reader);
    }

    private async Task<bool> MoveNextTokenAsync(PdfReader2 reader)
    {
        var chunk = await reader.ChunkAsync(MaxIdentifierLength);
        var index = EndObjMatcher.Instance.Match(chunk.Span);
        if (index == -1)
            return false;

        return await reader.MoveAsync(index);
    }
}