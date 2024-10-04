namespace PdfMerger.Pdf.Writers;

internal class CatalogWriter
{
    private readonly Stream _stream;
    private static readonly byte[] StartObjCatalog = "<<1 0 obj\n/Type /Catalog"u8.ToArray();
    private static readonly byte[] Pages = "/Pages "u8.ToArray();
    private static readonly byte[] PageLabels = "/PageLabels "u8.ToArray();
    private static readonly byte[] Names = "/Names "u8.ToArray();

    public CatalogWriter(Stream stream)
    {
        _stream = stream;
    }
    
    public ValueTask WriteStartObjAsync()
    {
        return _stream.WriteAsync(StartObjCatalog);
    }
    
    public ValueTask WritePagesAsync()
    {
        return _stream.WriteAsync(StartObjCatalog);
    }
}