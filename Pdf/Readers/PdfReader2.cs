using System.Buffers;
using System.Text;

namespace PdfMerger.Pdf.Readers;

internal class PdfReader2
{
    private readonly Stream _stream;
    private readonly byte[] _bytesBuffer = new byte[300];
    
    private static readonly SearchValues<byte> SpacesSearchValues = SearchValues.Create(PdfConstants.Spaces);
    private static readonly SearchValues<byte> DelimitersSearchValues = SearchValues.Create(PdfConstants.Delimiters);
    
    private Memory<byte> _buffer;
    private int _currentBufferIndex;

    public PdfReader2(Stream stream)
    {
        _stream = stream;
    }

    public Memory<byte> Buffer => _buffer[_currentBufferIndex..];
    public byte Value => _buffer.Span[_currentBufferIndex];
    
    public async Task<bool> NextTokenAsync()
    {
        if (IsSpace())
            return await NextNonSpaceValueAsync();
        
        if (IsDelimiterWithoutSpaces() && await MoveAsync(1))
            return await NextNonSpaceValueAsync();
        
        if (await NextDelimiterAsync())
        {
            if (IsSpace())
                return await NextNonSpaceValueAsync();
            
            return true;
        }

        return false;
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

        if (_buffer.Length < numBytes)
        {
            _currentBufferIndex += _buffer.Length - 1;
            return false;
        }

        _currentBufferIndex += numBytes;
        
        return true;
    }

    public async Task<bool> StartWithAsync(Memory<byte> filter)
    {
        var chunk = await ChunkAsync(filter.Length);
        return chunk.Span.StartsWith(filter.Span);
    }
    
    public async Task<Memory<byte>> ChunkAsync(int numBytes)
    {
        if (!CanMove(numBytes))
            await ReadNextBytesToBufferAsync(_buffer.Length - _currentBufferIndex);

        if (_buffer.Length <= numBytes)
            return _buffer;
        
        var endIndex = _currentBufferIndex + numBytes;
        
        return _buffer[_currentBufferIndex..endIndex];
    }

    private async Task<bool> NextNonSpaceValueAsync()
    {
        return await FindAnyExceptAndMoveAsync(SpacesSearchValues);
    }
    
    public async Task<bool> NextDelimiterAsync()
    {
        return await FindAnyAndMoveAsync(DelimitersSearchValues);
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

            if (!await ReadNextBytesToBufferAsync(1))
                return false;
        }
    }
    
    public async Task<bool> FindAndMoveAsync(Memory<byte> filter)
    {
        while (true)
        {
            var index = _buffer.Span[_currentBufferIndex..].IndexOf(filter.Span);
            if (index > -1)
            {
                _currentBufferIndex += index;
                return true;
            }

            if (!await ReadNextBytesToBufferAsync(filter.Length))
                return false;
        }
    }
    
    public Task<bool> NextLineAsync() => FindAndMoveAsync(PdfConstants.NewLine);
    
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
    
    private async Task<bool> FindAnyExceptAndMoveAsync(SearchValues<byte> filters)
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
    
    private bool IsSpace() => PdfConstants.Spaces.Contains(Value);
    public bool IsDelimiterSpaces() => PdfConstants.Delimiters.Contains(Value);
    private bool IsDelimiterWithoutSpaces() => PdfConstants.DelimitersWithoutSpaces.Contains(Value);
    
    private bool CanMove(int numBytes) => _currentBufferIndex + numBytes < _buffer.Length;
    
    public async Task<bool> ReadNextBytesToBufferAsync(int keepLastBytes = 0)
    {
        if (keepLastBytes > 0 && _buffer.Length > 0)
            _buffer[^keepLastBytes..].CopyTo(_buffer);
        
        var num = await _stream.ReadAsync(_bytesBuffer, keepLastBytes, _bytesBuffer.Length - keepLastBytes);
        var numKeep = num + keepLastBytes;
        _buffer = new Memory<byte>(_bytesBuffer, 0, numKeep);
        _currentBufferIndex = 0;
        
        return num > 0;
    }

    public int IndexOfDelimiterInBuffer()
    {
        return _buffer.Span[_currentBufferIndex..].IndexOfAny(DelimitersSearchValues);
    }
    
    public string Debug()
    {
        var text = Encoding.UTF8.GetString(_buffer.Span[_currentBufferIndex..]);
        Console.WriteLine(text);
        
        return text;
    }
}