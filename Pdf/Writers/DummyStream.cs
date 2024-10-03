namespace PdfMerger.Pdf.Writers;

internal class DummyStream : Stream
{
    public override void Flush()
    {
        
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return 0;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return 0;
    }

    public override void SetLength(long value)
    {
        
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = new CancellationToken())
    {
        return ValueTask.CompletedTask;;
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public override void WriteByte(byte value)
    {
        
    }

    public override bool CanRead { get; }
    public override bool CanSeek { get; }
    public override bool CanWrite { get; }
    public override long Length { get; }
    public override long Position { get; set; }
}