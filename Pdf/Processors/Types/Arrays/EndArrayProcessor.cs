using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types.Arrays;

internal class EndArrayProcessor : IProcessor
{
    public static readonly EndArrayProcessor Instance = new();
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (reader.Value != ']')
            return false;
            
        await writer.WriteEndArrayAsync();
        
        return await reader.NextTokenAsync();
    }
}