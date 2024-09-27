namespace PdfMerger.Pdf.Writers;

public static class HeaderWriter
{
    private static byte[] _pdfHeader = "%PDF-1.7"u8.ToArray();
        
    public static Task WriterAsync(Stream stream)
    {
        return stream.WriteAsync(_pdfHeader, 0, _pdfHeader.Length);
    }
}