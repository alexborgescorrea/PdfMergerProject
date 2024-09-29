using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types;

internal class HexadecimalProcessor : IProcessor 
{
    public static readonly HexadecimalProcessor Instance = new();
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader)
    {
        if (reader.Value !='<')
            return false;
        
        var nextToken = await reader.PeekAsync();
        if (nextToken =='<')
            return false;
            
        await reader.FindAndMoveOrThrowAsync((byte)'>');

        return true;
    }
}