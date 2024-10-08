using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Footer;

internal class XRefProcessor : IProcessor
{
    public static readonly XRefProcessor Instance = new();
    private static readonly byte[] Token = "xref"u8.ToArray();
    private static readonly byte[] TrailerToken = "trailer"u8.ToArray();
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (!await reader.IsTokenAsync(Token))
            return false;
        
        return await reader.FindAndMoveAsync(TrailerToken);
    }
}