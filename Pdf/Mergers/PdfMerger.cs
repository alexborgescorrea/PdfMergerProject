using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Mergers;

public class PdfMerger
{
    public async Task MergeAsync(Stream[] sources, Stream destination)
    {
            
        await HeaderWriter.WriterAsync(destination);
        await new BodyMerger().MergeAsync(sources, destination);
    }
}