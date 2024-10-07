using PdfMerger.Exceptions;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Structs;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors;

internal class PdfProcessor
{
    private readonly HeaderProcessor _headerProcessor = new();
    private readonly FileProcessor _fileProcessor = new();

    public async Task ProcessAsync(PdfContext context, PdfWriter writer, params Stream[] streams)
    {
        var reader = new PdfReader(null!);                
        foreach (var stream in streams)
        {
            context.BaseReference += context.LargestObjNumer;
            context.LargestObjNumer = 0;
            reader.UpdateStream(stream);
            await ProcessInternalAsync(context, reader, writer);
            stream.Seek(0, SeekOrigin.Begin);
        }

        var pagesXRefItem = await writer.WriteObjPagesAsync(context);
        var catalogXRefItem = await writer.WriteObjCatalogAsync();
        context.References.Add(pagesXRefItem);
        context.References.Add(catalogXRefItem);
        var xrefOffset = await writer.WriteReferencesAsync(context.References);
        await writer.WriterTrailerAsync(context.References);
        await writer.WriteStartXRefAsync(xrefOffset);
        await writer.FlushAsync();
    }

    private async Task ProcessInternalAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (!await _headerProcessor.ProcessAsync(context, reader, writer))
            ThrowHelper.ThrowInvalidPdf();

        await _fileProcessor.ProcessAsync(context, reader, writer);        
    }
}