using System.Buffers;
using System.Text;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Structs;

namespace PdfMerger.Pdf.Writers;

internal class PdfWriter
{
    private static readonly byte[] PdfHeader = "%PDF-2.0"u8.ToArray();
    private static readonly byte[] BitString = [0x0D, 0x25, 0xE2, 0xE3, 0xCF, 0xD3];
    private static readonly byte[] EndObj = "\nendobj"u8.ToArray();
    private static readonly byte[] StartDictionary = "\n<<"u8.ToArray();
    private static readonly byte[] EndDictionary = "\n>>"u8.ToArray();
    private static readonly byte[] StartName = "\n/"u8.ToArray();
    private static readonly byte[] StartArray = " ["u8.ToArray();
    private static readonly byte[] EndArray = " ]"u8.ToArray();
    private static readonly byte[] TrueValue = " true"u8.ToArray();
    private static readonly byte[] FalseValue = " false"u8.ToArray();
    private static readonly byte[] NullValue = " null"u8.ToArray();
    private static readonly byte[] NullByteValue = [0x20, 0];
    
    private readonly Stream _stream;
    private readonly CatalogWriter _catalogWriter;

    public PdfWriter(Stream stream)
    {
        _stream = stream;
        _catalogWriter = new(stream);
    }

    public long Position => _stream.Position;
    
    public Task WriteHeaderAsync()
    {
        return _stream.WriteAsync(PdfHeader, 0, PdfHeader.Length);
    }

    public Task WriteBitStringAsync()
    {
        return _stream.WriteAsync(BitString, 0, BitString.Length);
    }
    
    public void WriteNewLine()
    {
        _stream.WriteByte(PdfConstants.NewLine);
    }
    
    public ValueTask WriteLineAsync(ReadOnlyMemory<byte> bytes)
    {
        WriteNewLine();
        return WriteAsync(bytes);
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
    
    public async Task<bool> WriteAndMoveAtAsync(PdfReader reader, Memory<byte> filter)
    {
        var indexKeepLastBytes = 0;
        do
        {
            var index = reader.IndexOfInBuffer(filter);
            if (index != -1)
            {
                await reader.CopyBufferToAsync(_stream, indexKeepLastBytes, index + (filter.Length - indexKeepLastBytes));
                await reader.MoveAsync(index);
                return true;
            }

            if (await reader.CopyAllBufferToAsync(_stream, indexKeepLastBytes))
                indexKeepLastBytes = filter.Length;
        } 
        while (await reader.ReadNextBytesToBufferAsync(filter.Length));

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

    public ValueTask WriterTrailerAsync(PdfXRefItem root, IReadOnlyList<PdfXRefItem> references)
    {
        return WriteAsync(Encoding.ASCII.GetBytes($"\ntrailer\n<</Root {root}/Size {references.Count + 1}>>"));
    }
    
    public ValueTask WriteStartXRefAsync(long xrefOffset)
    {
        return WriteAsync(Encoding.ASCII.GetBytes($"\nstartxref\n{xrefOffset}\n%%EOF"));
    }

    public ValueTask<PdfXRefItem> WriteObjCatalogAsync(PdfCatalog catalog)
    {
        return _catalogWriter.WriteObjCatalogAsync(catalog);
    }

    public Task FlushAsync()
    {
        return _stream.FlushAsync();
    }
}