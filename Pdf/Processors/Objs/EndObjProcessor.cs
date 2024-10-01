using System.Text;
using PdfMerger.Pdf.Matchers;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Objs;

internal class EndObjProcessor : IProcessor
{
    public static readonly EndObjProcessor Instance = new();
    private const int MaxIdentifierLength = 255;
    private int _count;
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        var chunk = await reader.ChunkAsync(MaxIdentifierLength);
        var index = EndObjMatcher.Instance.Match(chunk.Span);
        if (index == -1)
            return false;
        
        await context.PdfWriter.WriteEndObjAsync();
        await context.PdfWriter.WriteAsync(Encoding.ASCII.GetBytes($"-{++_count}"));//TODO: Remover
        // if (_count == 2526)
        //     _count.ToString();
             
        return await reader.MoveAsync(index);
    }
}