using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types;

internal class NullProcessor : IProcessor
{
    public static readonly NullProcessor Instance = new();
    
    public Task<bool> ProcessAsync(PdfContext context, PdfReader reader)
    {
        throw new NotImplementedException();
    }
}