using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Structs;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types;

internal class NameProcessor : IProcessor
{
    public static readonly NameProcessor Instance = new();
    private static readonly byte[] CatalogToken = "Catalog"u8.ToArray();
    private static readonly byte[] PagesToken = "Pages"u8.ToArray();
    private static readonly byte[] PageToken = "Page"u8.ToArray();
    private static readonly byte[] ObjStmToken = "ObjStm"u8.ToArray();

    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (reader.Value != '/')
            return false;
        
        if (!await reader.MoveAsync(1))
            return false;

        await UpdateContext(context, reader);
        
        await writer.WriteStartNameAsync();
        return await writer.WriteAndMoveAtDelimiterAsync(reader);
    }

    private async Task UpdateContext(PdfContext context, PdfReader reader)
    {
        if (context.Scope.Level != 1)
            return;

        if (!context.Scope.IsTypeKey)
            return;

        var objType = await ResolveObjTypeAsync(reader);
        context.Scope = context.Scope with { ObjType = objType };
    }

    private static async Task<ObjType> ResolveObjTypeAsync(PdfReader reader)
    {
        if (await reader.IsTokenAsync(CatalogToken))
            return ObjType.Catalog;

        if (await reader.IsTokenAsync(PagesToken))
            return ObjType.Pages;

        if (await reader.IsTokenAsync(PageToken))
            return ObjType.Page;
        
        if (await reader.IsTokenAsync(ObjStmToken))
            return ObjType.ObjStm;

        return ObjType.None;
    }
}