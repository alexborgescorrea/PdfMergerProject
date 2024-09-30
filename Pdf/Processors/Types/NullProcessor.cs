using PdfMerger.Pdf.Matchers;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types;

internal class NullProcessor : IProcessor
{
    public static readonly NullProcessor Instance = new();
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        if (reader.Value == 0)
            return await reader.MoveAsync(1);
        
        var chunk = await reader.ChunkAsync(5);
        var index = NullMatcher.Instance.Match(chunk.Span);
        if (index == -1)
            return false;
            
        return await reader.MoveAsync(index);
    }
}