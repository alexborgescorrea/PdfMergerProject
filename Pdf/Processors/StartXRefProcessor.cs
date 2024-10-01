using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors;

internal class StartXRefProcessor : IProcessor
{
    public static StartXRefProcessor Instance { get; } = new();
    private static readonly byte[] StartXrefToken = "startxref"u8.ToArray();
    private static readonly byte[] EofToken = "%%EOF"u8.ToArray();
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (!await reader.StartWithAsync(StartXrefToken))
            return false;
        
        return await reader.FindAndMoveAsync(EofToken) && 
               await reader.MoveAsync(EofToken.Length);
    }
}