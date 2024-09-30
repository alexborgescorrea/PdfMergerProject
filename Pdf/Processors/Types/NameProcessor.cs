using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types;

internal class NameProcessor : IProcessor
{
    public static readonly NameProcessor Instance = new();
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        if (reader.Value == '/')
            return await reader.MoveAsync(1) && await reader.NextTokenAsync();

        return false;
    }
}