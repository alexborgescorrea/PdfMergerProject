using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors;

internal class CommentProcessor : IProcessor
{
    public static CommentProcessor Instance { get; } = new();

    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, IPdfWriter writer)
    {
        if (reader.Value != '%')
            return false;
        
        await reader.NextLineAsync();

        return true;
    }
}