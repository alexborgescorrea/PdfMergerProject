using PdfMerger.Pdf.Readers;

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
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        if (reader.Value != '[')
            return false;

        await context.PdfWriter.WriteStartArrayAsync();
        
        return await reader.NextTokenAsync() && await ProcessorGroup.ProcessAsync(context, reader);
    }
}