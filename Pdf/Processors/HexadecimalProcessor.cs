using System.Buffers;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

public class HexadecimalProcessor : IProcessor 
{
    public static readonly HexadecimalProcessor Instance = new();
    
    private const byte Token = (byte)'<';
    private static readonly byte[] Tokenss = [Token, (byte)'>'];
    
    public async Task ProcessAsync(PdfReader reader, byte value)
    {
        var nextToken = await reader.PeekAsync();
        if (nextToken == Token)
            return;
            
        await reader.FindAndMoveOrThrowAsync((byte)'>');
    }

    public bool CanProcess(byte value) => value == Token;
    public byte[] Tokens => Tokenss;
}