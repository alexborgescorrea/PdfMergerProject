using PdfMerger.Exceptions;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

internal class PdfProcessor
{
    private readonly HeaderProcessor _headerProcessor = new();
    private readonly BodyProcessor _bodyProcessor = new(); 
        
    public async Task ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        if (!await _headerProcessor.ProcessAsync(context, reader))
            ThrowHelper.ThrowInvalidPdf();
        
        await _bodyProcessor.ProcessAsync(context, reader);
    }
}