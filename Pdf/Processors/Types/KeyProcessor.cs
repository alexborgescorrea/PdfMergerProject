using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types;

internal class KeyProcessor : IProcessor
{
    public static readonly KeyProcessor Instance = new();
    private static readonly ProcessorGroup ProcessorGroup = new 
    (
        [ 
            TypesProcessor.Instance,
            CommentProcessor.Instance 
        ]
    );
    private static readonly byte[] TypeToken = "Type"u8.ToArray();
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, IPdfWriter writer)
    {
        if (reader.Value != '/')
            return false;
        
        if (!await reader.MoveAsync(1))
            return false;

        await UpdateContext(context, reader);
        
        await writer.WriteStartNameAsync();
        return await writer.WriteAndMoveAtDelimiterAsync(reader) 
            && await ProcessorGroup.ProcessAsync(context, reader, writer);
    }
    
    private async Task UpdateContext(PdfContext context, PdfReader reader)
    {
        var isType = await reader.StartWithAsync(TypeToken);
        context.Scope = context.Scope with { IsType = isType };
    }
}