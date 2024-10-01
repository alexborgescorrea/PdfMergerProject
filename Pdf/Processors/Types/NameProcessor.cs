using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types;

internal class NameProcessor : IProcessor
{
    public static readonly NameProcessor Instance = new();
    
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        if (reader.Value != '/')
            return false;
        
        if (!await reader.MoveAsync(1))
            return false;

        await context.PdfWriter.WriteStartNameAsync();
        return await context.PdfWriter.WriteAndMoveAtDelimiterAsync(reader);
    }
}