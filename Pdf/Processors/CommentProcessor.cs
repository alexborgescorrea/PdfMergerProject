using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

internal class CommentProcessor : IProcessor
{
    public static CommentProcessor Instance { get; } = new();

    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        if (reader.Value != '%')
            return false;
        
        await reader.NextLineAsync();

        return true;
    }
}