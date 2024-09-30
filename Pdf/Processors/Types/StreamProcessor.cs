using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types;

internal class StreamProcessor : IProcessor
{
    public static readonly StreamProcessor Instance = new();
    private static readonly byte[] StreamToken = "stream\n"u8.ToArray();
    private static readonly byte[] EndToken = "\nendstream"u8.ToArray();
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        if (!await reader.StartWithAsync(StreamToken))
            return false;

        return await reader.FindAndMoveAsync(EndToken);
    }
}