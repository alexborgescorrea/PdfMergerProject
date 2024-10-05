using PdfMerger.Pdf.Structs;

namespace PdfMerger.Pdf;

internal class PdfContext
{
    public PdfXRefItem Root { get; set; }
    public PdfScope Scope { get; set; }
    public List<PdfCatalog> Catalogs { get; } = new(10);
    public List<PdfXRefItem> References { get; } = new(100);
}