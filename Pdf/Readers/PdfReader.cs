using System.Buffers;
using System.Text;
using PdfMerger.Exceptions;

namespace PdfMerger.Pdf.Readers;

internal class PdfReader : IDisposable, IAsyncDisposable
{
    private static readonly byte[] NewLinePdfFilter = [10];
    private static readonly SearchValues<byte> StartDelimitersSearchValues = SearchValues.Create(PdfConstants.StartDelimiters);
    private static readonly SearchValues<byte> DelimitersSearchValues = SearchValues.Create(PdfConstants.Delimiters);
    private static readonly SearchValues<byte> SpacesSearchValues = SearchValues.Create(PdfConstants.Spaces);
    private readonly Stream _stream;
    private readonly byte[] _bytesBuffer = new byte[300];
    private Memory<byte> _buffer = Memory<byte>.Empty;
    private int _currentBufferIndex;
    

    public PdfReader(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        _stream = stream;
    }

    public byte Value => _buffer.Span[_currentBufferIndex];
    
    public async Task<bool> BeginReadPdfAsync()
    {
        if (!await FindAndMoveAsync(PdfConstants.PdfHeaderSignature))
            return false;
        
        await MoveAsync(PdfConstants.PdfHeaderSignature.Length);
        await NextStartDelimiterOrThrowAsync();
        await NextNonSpaceValueOrThrowAsync();
        return true;
    }
    
    public Task<bool> NextLineAsync() => FindAndMoveAsync(NewLinePdfFilter);

    public bool CurrentTokenIsSpace() => SpacesSearchValues.Contains(Value);
    
    public async Task NextNonSpaceValueOrThrowAsync()
    {
        await IndexOfAnyExceptAndMoveOrThrowAsync(SpacesSearchValues);
    }
    
    public async Task NextStartDelimiterOrThrowAsync()
    {
        await FindAnyAndMoveAsync(StartDelimitersSearchValues);
    }
    
    public async Task<bool> NextDelimiterAsync()
    {
        return await FindAnyAndMoveAsync(DelimitersSearchValues);
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
    
    public async Task<Memory<byte>> ChunkAsync(int numBytes)
    {
        if (!CanMove(numBytes))
            await ReadNextBytesToBufferAsync(_buffer.Length - _currentBufferIndex);

        var endIndex = _currentBufferIndex + numBytes;
        
        return _buffer[_currentBufferIndex..endIndex];
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