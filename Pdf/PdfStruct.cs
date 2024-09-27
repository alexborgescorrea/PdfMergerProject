namespace PdfMerger.Pdf;

public class PdfStruct
{
    public PdfStruct()
    {
        BodyItems = new();
        PdfCrossReferenceTableItems = new();
    }
    
    public string Header { get; set; }
    public List<PdfBodyItem> BodyItems { get; set; }
    public List<PdfCrossReferenceTableItem> PdfCrossReferenceTableItems { get; set; }
    
}