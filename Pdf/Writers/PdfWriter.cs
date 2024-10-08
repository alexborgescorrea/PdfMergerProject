using System.Buffers;
using System.Text;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Structs;

namespace PdfMerger.Pdf.Writers;

internal class PdfWriter : IDisposable, IAsyncDisposable
{
    private static readonly byte[] PdfHeader = "%PDF-2.0"u8.ToArray();
    private static readonly byte[] BitString = [0x0D, 0x25, 0xE2, 0xE3, 0xCF, 0xD3];    
    private static readonly byte[] EndObj = "\nendobj"u8.ToArray();
    private static readonly byte[] StartDictionary = "<<"u8.ToArray();
    private static readonly byte[] EndDictionary = ">>"u8.ToArray();
    private static readonly byte[] StartName = "/"u8.ToArray();
    private static readonly byte[] StartArray = " ["u8.ToArray();
    private static readonly byte[] EndArray = " ]"u8.ToArray();
    private static readonly byte[] TrueValue = " true"u8.ToArray();
    private static readonly byte[] FalseValue = " false"u8.ToArray();
    private static readonly byte[] NullValue = " null"u8.ToArray();
    private static readonly byte[] NullByteValue = [0x20, 0];    

    private readonly Stream _stream;
    private readonly CatalogWriter _catalogWriter;
    private readonly PagesWriter _pagesWriter;

    public PdfWriter(Stream stream)
    {
        _stream = stream;
        _catalogWriter = new(stream);
        _pagesWriter = new(stream);
    }

    public long Position => _stream.Position;

    public Stream Stream => _stream;
    
    public Task CopyFromAsync(Stream stream)
    {
        return stream.CopyToAsync(_stream);
    }
    
    public Task WriteHeaderAsync()
    {
        return _stream.WriteAsync(PdfHeader, 0, PdfHeader.Length);
    }

    public Task WriteBitStringAsync()
    {
        return _stream.WriteAsync(BitString, 0, BitString.Length);
    }
    public void WriteSpace()
    {
        _stream.WriteByte(0x20);
    }
    
    public ValueTask WriteWithSpaceAsync(ReadOnlyMemory<byte> bytes)
    {
        WriteSpace();
        return WriteAsync(bytes);
    }
    
    public void WriteNewLine()
    {
        _stream.WriteByte(PdfConstants.NewLine);
    }

    public async ValueTask WriteStartObjAsync(PdfReferenceValue obj)
    {
        WriteNewLine();
        await WriteAsync(obj.GetObjBytes());
        WriteNewLine();
    }

    public ValueTask WriteLineAsync(ReadOnlyMemory<byte> bytes)
    {
        WriteNewLine();
        return WriteAsync(bytes);
    }

    public ValueTask WriteReferenceValueAsync(PdfReferenceValue reference)
    {
        WriteSpace();
        return WriteAsync(reference.GetReferenceBytes());
    }

    public ValueTask WriteAsync(ReadOnlyMemory<byte> bytes)
    {
        return _stream.WriteAsync(bytes);
    }
    
    public ValueTask WriteEndObjAsync()
    {
        return _stream.WriteAsync(EndObj);
    }
    
    public ValueTask WriteStartDictionaryAsync()
    {
        return _stream.WriteAsync(StartDictionary);
    }
    
    public ValueTask WriteEndDictionaryAsync()
    {
        return _stream.WriteAsync(EndDictionary);
    }
    
    public ValueTask WriteStartNameAsync()
    {
        return _stream.WriteAsync(StartName);
    }
    
    public ValueTask WriteStartArrayAsync()
    {
        return _stream.WriteAsync(StartArray);
    }
    
    public ValueTask WriteEndArrayAsync()
    {
        return _stream.WriteAsync(EndArray);
    }

    public ValueTask WriteBooleanAsync(byte value)
    {
        return _stream.WriteAsync(value == 't' ? TrueValue : FalseValue);
    }
    
    public ValueTask WriteNullByteAsync()
    {
        return _stream.WriteAsync(NullByteValue);
    }
    
    public ValueTask WriteNullAsync()
    {
        return _stream.WriteAsync(NullValue);
    }
    
    public async Task<bool> WriteAndMoveAtDelimiterAsync(PdfReader reader)
    {
        do
        {
            var index = reader.IndexOfDelimiterInBuffer();
            if (index != -1)
            {
                await reader.CopyBufferToAsync(_stream, 0, index);
                await reader.MoveAsync(index);
                return true;
            }
            
            await reader.CopyAllBufferToAsync(_stream);
        } 
        while (await reader.ReadNextBytesToBufferAsync());

        return false;
    }
    
    public async Task<bool> WriteAndMoveAtAsync(PdfReader reader, byte filter)
    {
        do
        {
            var index = reader.IndexOfInBuffer(filter);
            if (index != -1)
            {
                await reader.CopyBufferToAsync(_stream, 0, index + 1);
                await reader.MoveAsync(index);
                return true;
            }

            await reader.CopyAllBufferToAsync(_stream);
        } 
        while (await reader.ReadNextBytesToBufferAsync());

        return false;
    }
    
    public Task<bool> WriteAndMoveAtAsync(PdfReader reader, Memory<byte> filter)
    {
        return WriteAndMoveAtAsync(reader, _stream, filter);
    }
    
    public Task<bool> WriteAndMoveAtAsync(PdfReader reader, Memory<byte> filter1, Memory<byte> filter2)
    {
        return WriteAndMoveAtAsync(reader, _stream, filter1, filter2);
    }
    
    public async Task<bool> WriteAndMoveAtAsync(PdfReader reader, Stream stream, Memory<byte> filter, bool copyAtEndFilter = true)
    {
        var lengthCopy = copyAtEndFilter ? filter.Length : 0;
        var indexKeepLastBytes = 0;
        do
        {
            var index = reader.IndexOfInBuffer(filter);
            if (index != -1)
            {
                await reader.CopyBufferToAsync(stream, indexKeepLastBytes, index + (lengthCopy - indexKeepLastBytes));
                await reader.MoveAsync(index);
                return true;
            }

            if (await reader.CopyAllBufferToAsync(stream, indexKeepLastBytes))
                indexKeepLastBytes = filter.Length;
        } 
        while (await reader.ReadNextBytesToBufferAsync(filter.Length));

        return false;
    }
    
    public async Task<bool> WriteAndMoveAtAsync(PdfReader reader, Stream stream, Memory<byte> filter1, Memory<byte> filter2)
    {
        var indexKeepLastBytes = 0;
        do
        {
            var index = reader.IndexOfInBuffer(filter1, filter2);
            if (index != -1)
            {
                await reader.CopyBufferToAsync(stream, indexKeepLastBytes, index + (filter1.Length - indexKeepLastBytes));
                await reader.MoveAsync(index);
                return true;
            }

            if (await reader.CopyAllBufferToAsync(stream, indexKeepLastBytes))
                indexKeepLastBytes = filter1.Length;
        } 
        while (await reader.ReadNextBytesToBufferAsync(filter1.Length));

        return false;
    }
    
    public async Task<bool> WriteAndMoveAtIndexOfAnyAsync(PdfReader reader, SearchValues<byte> filter)
    {
        do
        {
            var index = reader.IndexOfAnyInBuffer(filter);
            if (index != -1)
            {
                await reader.CopyBufferToAsync(_stream, 0, index + 1);
                await reader.MoveAsync(index);
                return true;
            }

            await reader.CopyAllBufferToAsync(_stream);
        } 
        while (await reader.ReadNextBytesToBufferAsync());

        return false;
    }
    
    public async Task<bool> CopyAndMovieAsync(PdfReader reader, int length)
    {
        var chunk = await reader.ChunkAsync(length);
        await WriteAsync(chunk);

        return await reader.MoveAsync(length);
    } 

    public async Task<long> WriteReferencesAsync(IReadOnlyList<PdfXRefItem> references)
    {
        var xrefOffset = _stream.Position + 1;
        await WriteAsync(Encoding.ASCII.GetBytes($"\nxref\n0 {references.Count}"));
        await WriteAsync(Encoding.ASCII.GetBytes($"\r\n0000000000 65535 f"));
        var buffer = new byte[20];
        foreach (var reference in references)
        {
            var value = $"\r\n{reference.Position.ToString().PadLeft(10, '0')} {reference.Obj.Generation.ToString().PadLeft(5, '0')} n";
            Encoding.ASCII.GetBytes(value, buffer);
            await _stream.WriteAsync(buffer);
        }

        return xrefOffset;
    }

    public ValueTask WriterTrailerAsync(IReadOnlyList<PdfXRefItem> references)
    {
        return WriteAsync(Encoding.ASCII.GetBytes($"\ntrailer\n<</Root 1 0 R/Size {references.Count + 1}>>"));
    }
    
    public ValueTask WriteStartXRefAsync(long xrefOffset)
    {
        return WriteAsync(Encoding.ASCII.GetBytes($"\nstartxref\n{xrefOffset}\n%%EOF"));
    }

    public ValueTask<PdfXRefItem> WriteObjPagesAsync(PdfContext context)
    {
        return _pagesWriter.WriteObjPagesAsync(context);
    }

    public ValueTask WritePagesParent()
    {
        return _pagesWriter.WritePagesParentAsync();
    }

    public ValueTask<PdfXRefItem> WriteObjCatalogAsync()
    {
        return _catalogWriter.WriteObjCatalogAsync();
    }

    public Task FlushAsync()
    {
        return _stream.FlushAsync();
    }

    public void Dispose()
    {
        _stream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _stream.DisposeAsync();
    }
}