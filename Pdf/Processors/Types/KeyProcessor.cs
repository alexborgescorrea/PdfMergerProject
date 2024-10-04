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
    private static readonly byte[] PagesToken = "Pages"u8.ToArray();
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (reader.Value != '/')
            return false;
        
        if (!await reader.MoveAsync(1))
            return false;

        await UpdateContext(context, reader);
        
        await writer.WriteStartNameAsync();
        var result =  await writer.WriteAndMoveAtDelimiterAsync(reader) && 
                      await ProcessorGroup.ProcessAsync(context, reader, writer);

        ResetContext(context);
        
        return result;
    }
    
    private async Task UpdateContext(PdfContext context, PdfReader reader)
    {
        if (await reader.StartWithAsync(TypeToken))
        {
            context.Scope = context.Scope with
            {
                IsTypeKey = true,
                IsPagesType = false
            };
            return;
        }
        
        if (await reader.StartWithAsync(PagesToken))
        {
            context.Scope = context.Scope with
            {
                IsTypeKey = false,
                IsPagesType = true,
            };
            return;
        }
    }

    private void ResetContext(PdfContext context)
    {
        context.Scope = context.Scope with
        {
            IsTypeKey = false,
            IsPagesType = false
        };
    }
}