using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors;

internal class HeaderProcessor : IProcessor
{
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (!await reader.FindAndMoveAsync(PdfConstants.PdfHeaderSignature))
            return false;
        
        await writer.WriteHeaderAsync();
        await writer.WriteBitStringAsync();

        await reader.MoveAsync(PdfConstants.PdfHeaderSignature.Length);
        return await reader.NextTokenAsync();
    }
}
