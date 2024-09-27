using System.Text;

namespace PdfMerger.Pdf.Mergers;

public class PdfContext : IDisposable, IAsyncDisposable
{
    private static readonly byte[] HeaderPdf = [0x25, 0x50, 0x44, 0x46, 0x2d];
    private readonly Stream _stream;
    private readonly byte[] _bytesBuffer = new byte[50];
    private Memory<byte> _buffer = Memory<byte>.Empty;
    private int _currentBufferIndex = 0;
    

    public PdfContext(Stream stream)
    {
        _stream = stream;
    }
    
    public async Task<bool> BeginReadPdf()
    {
        await FindAndMoveAsync(HeaderPdf);
        return await MoveAsync(HeaderPdf.Length) && await FindSeparatorAndMoveAsync();
    }
    
    public async Task<PdfTokenType> NextTokenAsync()
    {
        while (true)
        {
            for (var i = _currentBufferIndex; i < _buffer.Length; i++, _currentBufferIndex++)
            {
                var value = (char)_buffer.Span[i];
                if (value == '(')
                    return PdfTokenType.StartString;

                if (value == '%')
                    return PdfTokenType.StartComment;

                if (value == '<')
                    return PdfTokenType.StartHexadecimal;

                if (value is > '0' and < '9')
                    return PdfTokenType.StartNumber;
            }

            if (!await ReadNextBytesToBufferAsync())
                return PdfTokenType.None;
        }
    }
    
    private async Task<bool> FindSeparatorAndMoveAsync()
    {
        while (true)
        {
            for (var i = _currentBufferIndex; i < _buffer.Length; i++, _currentBufferIndex++)
            {
                var value = (char)_buffer.Span[i];
                if (value is (' ' or '\n' or '\t' or '%' or '<' or '(' or '[' or '{'))
                    return true;
            }

            if (!await ReadNextBytesToBufferAsync())
                return false;
        }
    }
    
    private async Task<bool> FindAndMoveAsync(Memory<byte> filter)
    {
        while (true)
        {
            var index = _buffer.Span[_currentBufferIndex..].IndexOf(filter.Span);
            if (index > -1)
            {
                _currentBufferIndex = index;
                return true;
            }

            if (!await ReadNextBytesToBufferAsync(filter.Length))
                return false;
        }
    }

    private async Task<bool> MoveAsync(int numBytes)
    {
        if (CanMove(numBytes))
        {
            _currentBufferIndex += numBytes;
            
            Console.WriteLine($"Move => {Encoding.UTF8.GetString(_buffer.Span[_currentBufferIndex..])}");
            
            return true;
        }

        if (!await ReadNextBytesToBufferAsync(numBytes))
            return false;
        
        _currentBufferIndex += numBytes;
        
        Console.WriteLine($"Move => {Encoding.UTF8.GetString(_buffer.Span[_currentBufferIndex..])}");
        
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
        
        Console.WriteLine(Encoding.UTF8.GetString(_buffer.Span));
        
        return num > 0;
    }

    public void Dispose()
    {
        _stream?.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _stream.DisposeAsync();
    }
}