using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

internal class XRefProcessor : IProcessor
{
    public static readonly XRefProcessor Instance = new();
    
    public Task<bool> ProcessAsync(PdfContext context, PdfReader reader)
    {
        return Task.FromResult(false);
    }
}