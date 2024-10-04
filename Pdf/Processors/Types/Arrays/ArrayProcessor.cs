using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types.Arrays;

internal class ArrayProcessor : IProcessor
{
    public static readonly ArrayProcessor Instance = new();
    private static readonly ProcessorGroup ProcessorGroup = new 
    (
        [ 
            CommentProcessor.Instance, 
            TypesProcessor.Instance, 
            EndArrayProcessor.Instance 
        ], 
        EndArrayProcessor.Instance
    );
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (reader.Value != '[')
            return false;

        await writer.WriteStartArrayAsync();
        
        context.Scope = context.Scope with { Level = context.Scope.Level + 1 };
        var result = await reader.NextTokenAsync() && 
                     await ProcessorGroup.ProcessAsync(context, reader, writer);
        context.Scope = context.Scope with { Level = context.Scope.Level - 1 };
        return result;
    }
}