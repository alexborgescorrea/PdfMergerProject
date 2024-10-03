using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types;

internal class NameProcessor : IProcessor
{
    public static readonly NameProcessor Instance = new();
    private static readonly byte[] CatalogToken = "Catalog"u8.ToArray();
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, IPdfWriter writer)
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
        if (context.Scope.IsType && context.Scope.Level == 1 && await reader.StartWithAsync(CatalogToken))
            context.Scope = context.Scope with { IsCatalog = true };
    }
}