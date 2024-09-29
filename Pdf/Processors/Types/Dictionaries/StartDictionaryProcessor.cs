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
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader)
    {
        var chunk = await reader.ChunkAsync(2);
        if (!chunk.Span.SequenceEqual(Tokens))
            return false;

        return true;
    }
}