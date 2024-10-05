using PdfMerger.Exceptions;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Structs;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors;

internal class PdfProcessor
{
    private readonly HeaderProcessor _headerProcessor = new();
    private readonly FileProcessor _fileProcessor = new(); 
        
    public async Task ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (!await _headerProcessor.ProcessAsync(context, reader, writer))
            ThrowHelper.ThrowInvalidPdf();
        
        await _fileProcessor.ProcessAsync(context, reader, writer);
        var mainCatalog = context.Catalogs.First();
        var objCatalog = await writer.WriteObjCatalogAsync(mainCatalog);
        context.References.Add(objCatalog);
        var xrefOffset = await writer.WriteReferencesAsync(context.References);
        await writer.WriterTrailerAsync(context.Root, context.References);
        await writer.WriteStartXRefAsync(xrefOffset);
        await writer.FlushAsync();
    }
}