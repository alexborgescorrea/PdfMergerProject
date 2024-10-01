using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types.Dictionaries;

internal class StartDictionaryProcessor : IProcessor
{
    public static readonly StartDictionaryProcessor Instance = new();
    
    private static readonly ProcessorGroup ProcessorGroup = new 
    (
        [ 
            CommentProcessor.Instance, 
            TypesProcessor.Instance, 
            EndDictionaryProcessor.Instance 
        ], 
        EndDictionaryProcessor.Instance
    );
    private static readonly byte[] Tokens = [(byte)'<', (byte)'<'];
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        if (await reader.StartWithAsync(Tokens) && await reader.MoveAsync(2))
        {
            await context.PdfWriter.WriteStartDictionaryAsync();
            return await ProcessorGroup.ProcessAsync(context, reader);
        }

        return false;
    }
}