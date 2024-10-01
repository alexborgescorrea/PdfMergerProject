using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

internal class StartXRefProcessor : IProcessor
{
    public static StartXRefProcessor Instance { get; } = new();
    private static readonly byte[] StartXrefToken = "startxref"u8.ToArray();
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        if (!await reader.StartWithAsync(StartXrefToken))
            return false;

        return false;
    }
}