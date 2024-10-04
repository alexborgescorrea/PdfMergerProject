using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types.Dictionaries;

internal class DictionaryProcessor : IProcessor
{
    public static readonly DictionaryProcessor Instance = new();
    
    private static readonly ProcessorGroup ProcessorGroup = new 
    (
        [ 
            CommentProcessor.Instance, 
            KeyProcessor.Instance, 
            EndDictionaryProcessor.Instance 
        ], 
        EndDictionaryProcessor.Instance
    );
    private static readonly byte[] Tokens = [(byte)'<', (byte)'<'];
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (await reader.StartWithAsync(Tokens) && await reader.MoveAsync(2))
        {
            await writer.WriteStartDictionaryAsync();
            
            context.Scope = context.Scope with { Level = context.Scope.Level + 1 };
            var result = await ProcessorGroup.ProcessAsync(context, reader, writer);
            context.Scope = context.Scope with { Level = context.Scope.Level - 1 };
            
            return result;
        }

        return false;
    }
}