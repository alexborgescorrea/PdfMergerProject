using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf;

internal class PdfContext
{
    public PdfContext(PdfWriter pdfWriter)
    {
        PdfWriter = pdfWriter;
    }
    
    public PdfWriter PdfWriter { get; }
}