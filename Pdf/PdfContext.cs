using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf;

internal class PdfContext
{
    public List<PdfReference> References { get; } = new(5000);
}