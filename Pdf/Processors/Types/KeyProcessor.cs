using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Structs;
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
    private static readonly byte[] PageToken = "Page"u8.ToArray();
    private static readonly byte[] ParentToken = "Parent"u8.ToArray();    

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
        var keyName = await ResolveKeyNameAsync(context, reader);
        context.Scope = context.Scope with
        {
            KeyName = keyName
        };

        if (context.Scope.KeyName == KeyName.PagesParent)
            context.Scope = context.Scope with { HasPagesParent = true };
    }

    private static async Task<KeyName> ResolveKeyNameAsync(PdfContext context, PdfReader reader)
    {
        if (context.Scope.Level != 1)
            return KeyName.None;

        if (await reader.StartWithAsync(TypeToken))
            return KeyName.Type;

        if (await reader.StartWithAsync(PagesToken))
            return KeyName.Pages;

        if (await reader.StartWithAsync(PageToken))
            return KeyName.Page;

        if (await reader.StartWithAsync(ParentToken))
            return KeyName.PagesParent;        

        return KeyName.None;
    }

    private void ResetContext(PdfContext context)
    {
        context.Scope = context.Scope with
        {
            KeyName = KeyName.None
        };
    }
}