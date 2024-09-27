using PdfMerger.Exceptions;
using PdfMerger.Pdf.Mergers;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

internal class PdfProcessor
{
    private readonly BodyProcessor _bodyProcessor = new(); 
        
    public async Task ProcessAsync(PdfReader reader)
    {
        if (!await reader.BeginReadPdfAsync())
            ThrowHelper.ThrowInvalidPdf();
        
        await _bodyProcessor.ProcessAsync(reader);
    }
}