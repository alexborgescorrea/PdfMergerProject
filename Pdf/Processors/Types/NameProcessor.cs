using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types;

internal class NameProcessor : IProcessor
{
    public static readonly NameProcessor Instance = new();
    
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (reader.Value != '/')
            return false;
        
        if (!await reader.MoveAsync(1))
            return false;

        await writer.WriteStartNameAsync();
        return await writer.WriteAndMoveAtDelimiterAsync(reader);
    }
}