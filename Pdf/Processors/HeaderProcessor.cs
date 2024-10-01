using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

internal class HeaderProcessor : IProcessor
{
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        if (!await reader.FindAndMoveAsync(PdfConstants.PdfHeaderSignature))
            return false;
        
        await context.PdfWriter.WriteHeaderAsync();
        await context.PdfWriter.WriteBiStringAsync();
        
        return await reader.NextTokenAsync();
    }
}
