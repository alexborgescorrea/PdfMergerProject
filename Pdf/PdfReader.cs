using System.Buffers;

namespace PdfMerger.Pdf;

public class PdfReader
{
    private const int BufferSize = 1024;
    private const byte NewLine = 13;
    
    public async Task<PdfStruct> ReadAsync(Stream stream)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
        try
        {
            return await ReadInternalAsync(stream, buffer);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private async Task<PdfStruct> ReadInternalAsync(Stream stream, Memory<byte> buffer)
    {
        var pdfStruct = new PdfStruct();
        while (true)
        {
            var num = await stream.ReadAsync(buffer);
            if (num == 0)
                return pdfStruct;
            
            ResolveLines(buffer.Span);
        }
    }

    private static void ResolveLines(Span<byte> buffer)
    {
        while (true)
        {
            var index = buffer.IndexOf(NewLine);
            if (index < 0)
                return;

            ProcessLine(buffer[..(index)]);   
        }
    }

    private static void ProcessLine(Span<byte> line)
    {
        
    }

    private static void LoadHeader()
    {
        
    }
}