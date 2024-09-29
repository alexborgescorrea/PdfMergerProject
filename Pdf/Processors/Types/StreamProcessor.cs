using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types;

internal class StreamProcessor : IProcessor
{
    public static readonly StreamProcessor Instance = new();
    
    public Task<bool> ProcessAsync(PdfContext context, PdfReader reader)
    {
        throw new NotImplementedException();
    }
}