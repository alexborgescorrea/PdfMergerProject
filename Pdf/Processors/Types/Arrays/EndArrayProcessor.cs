using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types.Arrays;

internal class EndArrayProcessor : IProcessor
{
    public static readonly EndArrayProcessor Instance = new();
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        if (reader.Value == ']')
            return await reader.NextTokenAsync();

        return false;
    }
}