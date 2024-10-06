using PdfMerger.Pdf.Structs;

namespace PdfMerger.Pdf;

internal class PdfContext
{
    public int BaseReference { get; set; } = 2;
    public PdfXRefItem Root { get; set; }
    public PdfScope Scope { get; set; }
    public int PagesCount { get; set; }
    public List<PdfCatalog> Catalogs { get; } = new(10);
    public List<PdfXRefItem> References { get; } = new(100);    
}