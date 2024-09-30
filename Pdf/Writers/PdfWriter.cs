using PdfMerger.Pdf.Readers;

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
    
    private readonly Stream _stream;

    public PdfWriter(Stream stream)
    {
        _stream = stream;
    }

    public Task WriterHeaderAsync()
    {
        return _stream.WriteAsync(PdfHeader, 0, PdfHeader.Length);
    }

    public Task WriterBiStringAsync()
    {
        return _stream.WriteAsync(BitString, 0, BitString.Length);
    }
    
    public void WriteNewLine()
    {
        _stream.WriteByte(PdfConstants.NewLine);
    }
    
    public ValueTask WriterLineAsync(ReadOnlyMemory<byte> bytes)
    {
        WriteNewLine();
        return WriterAsync(bytes);
    }
    
    public ValueTask WriterAsync(ReadOnlyMemory<byte> bytes)
    {
        return _stream.WriteAsync(bytes);
    }
    
    public ValueTask WriterEndObjAsync()
    {
        return _stream.WriteAsync(EndObj);
    }
    
    public ValueTask WriterStartDictionaryAsync()
    {
        return _stream.WriteAsync(StartDictionary);
    }
    
    public ValueTask WriterEndDictionaryAsync()
    {
        return _stream.WriteAsync(EndDictionary);
    }
    
    public ValueTask WriterStartNameAsync()
    {
        return _stream.WriteAsync(StartName);
    }
    
    public ValueTask WriterStartArrayAsync()
    {
        return _stream.WriteAsync(StartArray);
    }
    
    public ValueTask WriterEndArrayAsync()
    {
        return _stream.WriteAsync(EndArray);
    }

    public ValueTask WriterBooleanAsync(byte value)
    {
        return _stream.WriteAsync(value == 't' ? TrueValue : FalseValue);
    }
    
    public async Task<bool> WriterAndMoveAtDelimiterAsync(PdfReader2 reader)
    {
        do
        {
            var index = reader.IndexOfDelimiterInBuffer();

            if (index != -1)
            {
                await WriterAsync(reader.Buffer[..index]);
                return true;
            }
        } 
        while (await reader.ReadNextBytesToBufferAsync());

        return false;
    }
}