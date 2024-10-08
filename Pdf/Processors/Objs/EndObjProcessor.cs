using PdfMerger.Pdf.Matchers;
using PdfMerger.Pdf.Processors.Types;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Structs;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Objs;

internal class EndObjProcessor : IProcessor
{
    public static readonly EndObjProcessor Instance = new();
    private const int MaxIdentifierLength = 255;
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        var chunk = await reader.ChunkAsync(MaxIdentifierLength);
        var index = EndObjMatcher.Instance.Match(chunk.Span);
        if (index == -1)
            return false;
        
        if (context.Scope.ObjType != ObjType.ObjStm)
            await writer.WriteEndObjAsync();
        
        return await reader.MoveAsync(index);
    }
}