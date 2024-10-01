using PdfMerger.Exceptions;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors;

internal class PdfProcessor
{
    private readonly HeaderProcessor _headerProcessor = new();
    private readonly BodyProcessor _bodyProcessor = new(); 
        
    public async Task ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (!await _headerProcessor.ProcessAsync(context, reader, writer))
            ThrowHelper.ThrowInvalidPdf();
        
        await _bodyProcessor.ProcessAsync(context, reader, writer);
        await writer.WriterReferencesAsync(context.References);
        await writer.FlushAsync();
    }
}