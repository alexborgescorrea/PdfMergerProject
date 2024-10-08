using PdfMerger.Pdf.Matchers;
using PdfMerger.Pdf.Processors.Types;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Structs;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Objs;

internal class ObjProcessor : IProcessor
{
    public static readonly ObjProcessor Instance = new();
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
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (!await ProcessInternalAsync(context, reader, writer))
            return false;
        
        var scope = context.Scope;
        var result = await ProcessorGroup.ProcessAsync(context, reader, writer);
        UpdateContext(context);        
        context.Scope = scope;
        return result;
    }    

    private async Task<bool> ProcessInternalAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        var originalPosition = reader.Position;
        var chunk = await reader.ChunkAsync(MaxIdentifierLength);
        var index = StartObjMatcher.Instance.Match(chunk.Span);
        if (index == -1)
            return false;

        chunk = chunk[..index];
        var obj = chunk.ExtractValueReference();
        if (obj.Number > context.LargestObjNumer)
            context.LargestObjNumer = obj.Number;

        obj += context.BaseReference;
        await writer.WriteStartObjAsync(obj);

        context.References.Add(new(obj, writer.Position, originalPosition));        

        return await reader.MoveAsync(index);
    }

    private static void UpdateContext(PdfContext context)
    {
        UpdateCatalogContext(context);
        UpdatePageCountContext(context);
    }

    private static void UpdateCatalogContext(PdfContext context)
    {
        if (context.Scope.ObjType != ObjType.Catalog)
            return;

        var reference = context.References[^1];
        context.Root = reference;
        context.Catalogs.Add(new()
        {
            Obj = reference.Obj,
            Pages = context.Scope.Pages
        });
    }

    private static void UpdatePageCountContext(PdfContext context)
    {
        if (context.Scope.ObjType == ObjType.Page)
            context.PagesCount++;
    }
}