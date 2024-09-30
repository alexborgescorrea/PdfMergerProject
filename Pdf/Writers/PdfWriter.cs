namespace PdfMerger.Pdf.Writers;

internal class PdfWriter
{
    private static readonly byte[] PdfHeader = "%PDF-2.0"u8.ToArray();
    
    private readonly Stream _stream;

    public PdfWriter(Stream stream)
    {
        _stream = stream;
    }

    public Task WriterHeaderAsync()
    {
        return _stream.WriteAsync(PdfHeader, 0, PdfHeader.Length);
    }
    
    public ValueTask WriterAsync(Memory<byte> bytes)
    {
        return _stream.WriteAsync(bytes);
    }
}