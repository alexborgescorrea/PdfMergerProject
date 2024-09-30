using PdfMerger.Pdf.Matchers;
using PdfMerger.Pdf.Processors.Types;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Objs;

internal class StartObjProcessor : IProcessor
{
    public static readonly StartObjProcessor Instance = new();
    private const int MaxIdentifierLength = 255;
    private static readonly ProcessorGroup ProcessorGroup = new 
    (
        [ 
            CommentProcessor.Instance, 
            TypesProcessor.Instance, 
            EndObjProcessor.Instance 
        ], 
        EndObjProcessor.Instance
    );
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        if (!await ProcessInternalAsync(context, reader))
            return false;
        
        return await ProcessorGroup.ProcessAsync(context, reader);
    }
    
    private async Task<bool> ProcessInternalAsync(PdfContext context, PdfReader2 reader)
    {
        var chunk = await reader.ChunkAsync(MaxIdentifierLength);
        var index = StartObjMatcher.Instance.Match(chunk.Span);
        if (index == -1)
            return false;
        
        await context.PdfWriter.WriterLineAsync(chunk.Slice(0, index));

        return await reader.MoveAsync(index);
    }
}