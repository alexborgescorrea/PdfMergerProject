using PdfMerger.Pdf.Structs;
using System.Runtime.CompilerServices;
using System.Text;

namespace PdfMerger.Pdf.Writers;

internal class CatalogWriter
{
    private readonly Stream _stream;
    private static readonly byte[] StartObjCatalog = "\n1 0 obj\n<</Type /Catalog "u8.ToArray();
    private static readonly byte[] EndObjCatalog = " >>\nendobj"u8.ToArray();
    private static readonly byte[] Pages = "/Pages "u8.ToArray();
    private static readonly byte[] PageLabels = "/PageLabels "u8.ToArray();
    private static readonly byte[] Names = "/Names "u8.ToArray();

    public CatalogWriter(Stream stream)
    {
        _stream = stream;
    }
    
    private ValueTask WriteStartObjAsync()
    {
        return _stream.WriteAsync(StartObjCatalog);
    }

    private ValueTask WriteEndObjAsync()
    {
        return _stream.WriteAsync(EndObjCatalog);
    }

    private ValueTask WritePagesAsync()
    {
        return _stream.WriteAsync(Pages);
    }

    public async ValueTask<PdfXRefItem> WriteObjCatalogAsync(PdfCatalog catalog)
    {
        var position = _stream.Position + 1;
        await WriteStartObjAsync();
        await WritePagesAsync();
        await _stream.WriteAsync(catalog.Pages.ToArray());
        await WriteEndObjAsync();
        return new PdfXRefItem(new(1, 0), position, position);
    }
}