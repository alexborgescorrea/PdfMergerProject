namespace PdfMerger.Pdf.Mergers;

public class BodyMerger
{
    public async Task MergeAsync(Stream[] sources, Stream destination)
    {
        foreach (var source in sources)
        {
            await using var pdfContext = new PdfContext(source);

            await pdfContext.BeginReadPdf();
            var token = await pdfContext.NextTokenAsync();
            token.ToString();
        }
    }
}