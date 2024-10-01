using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types.Arrays;

internal class StartArrayProcessor : IProcessor
{
    public static readonly StartArrayProcessor Instance = new();
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
        
        return await reader.NextTokenAsync() && await ProcessorGroup.ProcessAsync(context, reader, writer);
    }
}