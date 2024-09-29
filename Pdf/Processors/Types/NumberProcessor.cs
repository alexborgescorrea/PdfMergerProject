using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types;

internal class NumberProcessor : IProcessor
{
    public static readonly NumberProcessor Instance = new();
    
    public Task<bool> ProcessAsync(PdfContext context, PdfReader reader)
    {
        throw new NotImplementedException();
    }
}