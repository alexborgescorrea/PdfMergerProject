using System.Buffers;
using System.Text;
using PdfMerger.Exceptions;
using PdfMerger.Pdf.Mergers;

namespace PdfMerger.Pdf.Readers;

public class PdfReader : IDisposable, IAsyncDisposable
{
    private static readonly byte[] HeaderPdfFilter = [0x25, 0x50, 0x44, 0x46, 0x2d];
    private static readonly byte[] NewLinePdfFilter = [10];
    private static readonly SearchValues<byte> SeparatorsPdfFilter = SearchValues.Create([(byte)' ', (byte)'\n', (byte)'\t', (byte)'%', (byte)'<', (byte)'(', (byte)'[', (byte)'{']);
    private static readonly SearchValues<byte> SpacesPdfFilter = SearchValues.Create([(byte)' ', (byte)'\n', (byte)'\t']);
    private readonly Stream _stream;
    private readonly byte[] _bytesBuffer = new byte[50];
    private Memory<byte> _buffer = Memory<byte>.Empty;
    private int _currentBufferIndex;
    

    public PdfReader(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        _stream = stream;
    }

    public byte SingleValue() => _buffer.Span[_currentBufferIndex];
    
    public async Task<bool> BeginReadPdfAsync()
    {
        return await FindAndMoveAsync(HeaderPdfFilter) &&
               await MoveAsync(HeaderPdfFilter.Length) && 
               await FindAnyAndMoveAsync(SeparatorsPdfFilter);
    }

    public Task<bool> NextLineAsync() => FindAndMoveAsync(NewLinePdfFilter);

    public bool CurrentTokenIsSpace() => SpacesPdfFilter.Contains(SingleValue());
    
    public async Task NextNonSpaceValueOrThrowAsync()
    {
        await IndexOfAnyExceptAndMoveOrThrowAsync(SpacesPdfFilter);
    }

    public async Task FindAnyAndMoveOrThrowAsync(SearchValues<byte> filters)
    {
        if (await FindAnyAndMoveAsync(filters))
            return;
        
        ThrowHelper.ThrowInvalidPdf();
    }
    
    public async Task<bool> FindAnyAndMoveAsync(SearchValues<byte> filters)
    {
        while (true)
        {
            var index = _buffer.Span[_currentBufferIndex..].IndexOfAny(filters);
            if (index != -1)
            {
                _currentBufferIndex += index;
                return true;
            }

            if (!await ReadNextBytesToBufferAsync())
                return false;
        }
    }

    public async Task IndexOfAnyExceptAndMoveOrThrowAsync(SearchValues<byte> filters)
    {
        if (await IndexOfAnyExceptAndMoveAsync(filters))
            return;
        
        ThrowHelper.ThrowInvalidPdf();
    }
    
    public async Task<bool> IndexOfAnyExceptAndMoveAsync(SearchValues<byte> filters)
    {
        while (true)
        {
            var index = _buffer.Span[_currentBufferIndex..].IndexOfAnyExcept(filters);
            if (index != -1)
            {
                _currentBufferIndex += index;
                return true;
            }

            if (!await ReadNextBytesToBufferAsync())
                return false;
        }
    }
    
    public async Task<bool> FindAndMoveAsync(byte[] filter)
    {
        while (true)
        {
            var index = _buffer.Span[_currentBufferIndex..].IndexOf(filter);
            if (index > -1)
            {
                _currentBufferIndex += index;
                return true;
            }

            if (!await ReadNextBytesToBufferAsync(filter.Length))
                return false;
        }
    }

    public async Task FindAndMoveOrThrowAsync(byte filter)
    {
        if (await FindAndMoveAsync(filter))
            return;
        
        ThrowHelper.ThrowInvalidPdf();
    }
    
    public async Task<bool> FindAndMoveAsync(byte filter)
    {
        while (true)
        {
            var index = _buffer.Span[_currentBufferIndex..].IndexOf(filter);
            if (index > -1)
            {
                _currentBufferIndex += index;
                return true;
            }

            if (!await ReadNextBytesToBufferAsync())
                return false;
        }
    }

    public async Task MoveOrThrowAsync(int numBytes = 1)
    {
        if (await MoveAsync(numBytes))
            return;
        
        ThrowHelper.ThrowInvalidPdf();
    }

    public async Task<byte> PeekAsync()
    {
        var bytes = await PeekAsync(1);
        if (bytes.Length == 0)
            return 0;
        
        return bytes.Span[0]; 
    }
    
    public async Task<Memory<byte>> PeekAsync(byte numBytes)
    {
        if (!CanMove(numBytes))
            await ReadNextBytesToBufferAsync(_buffer.Length - _currentBufferIndex);

        var startIndex = _currentBufferIndex + 1;
        var endIndex = startIndex + numBytes;
        
        return _buffer[startIndex..endIndex];
    }
    
    public async Task<bool> MoveAsync(int numBytes)
    {
        if (CanMove(numBytes))
        {
            _currentBufferIndex += numBytes;
            return true;
        }

        if (!await ReadNextBytesToBufferAsync(numBytes))
            return false;
        
        _currentBufferIndex += numBytes;
        return true;
    }

    private bool CanMove(int numBytes) => _currentBufferIndex + numBytes < _buffer.Length;
    
    private async Task<bool> ReadNextBytesToBufferAsync(int keepLastBytes = 0)
    {
        if (keepLastBytes > 0 && _buffer.Length > 0)
            _buffer[^keepLastBytes..].CopyTo(_buffer);
        
        var num = await _stream.ReadAsync(_bytesBuffer, keepLastBytes, _bytesBuffer.Length - keepLastBytes);
        var numKeep = num + keepLastBytes;
        _buffer = new Memory<byte>(_bytesBuffer, 0, numKeep);
        _currentBufferIndex = 0;
        
        return num > 0;
    }

    public string Debug()
    {
        var text = Encoding.UTF8.GetString(_buffer.Span[_currentBufferIndex..]);
        Console.WriteLine(text);
        
        return text;
    }

    public void Dispose()
    {
        _stream.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _stream.DisposeAsync();
    }
}