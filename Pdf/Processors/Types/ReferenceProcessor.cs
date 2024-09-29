using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types;

internal class ReferenceProcessor : IProcessor
{
    public static readonly ReferenceProcessor Instance = new();
    
    public Task<bool> ProcessAsync(PdfContext context, PdfReader reader)
    {
        throw new NotImplementedException();
    }
}