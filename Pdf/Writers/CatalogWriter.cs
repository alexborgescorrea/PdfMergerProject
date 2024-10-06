using PdfMerger.Pdf.Structs;
using System.Runtime.CompilerServices;
using System.Text;

namespace PdfMerger.Pdf.Writers;

internal class CatalogWriter
{
    private readonly Stream _stream;
    private static readonly byte[] ObjCatalog = "\n1 0 obj\n<</Type/Catalog /Pages 2 0 R>>\nendobj"u8.ToArray();

    public CatalogWriter(Stream stream)
    {
        _stream = stream;
    }
    
    public async ValueTask<PdfXRefItem> WriteObjCatalogAsync()
    {
        var position = _stream.Position + 1;        
        await _stream.WriteAsync(ObjCatalog);

        return new(new(1, 0), position, position);
    }
}