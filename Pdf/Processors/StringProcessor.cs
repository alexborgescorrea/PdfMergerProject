using System.Buffers;
using PdfMerger.Exceptions;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

internal class StringProcessor : IProcessor
{
    public static readonly StringProcessor Instance = new();
    
    private const byte Token = (byte)'(';
    private const byte BackSlashToken = 0x5C;
    private static readonly byte[] Tokenss = [Token];
    private static readonly SearchValues<byte> NextTokensSearchValues = SearchValues.Create([(byte)')', BackSlashToken]);
    
    public async Task ProcessAsync(PdfReader reader, byte value)
    {
        while (true)
        {
            await reader.FindAnyAndMoveOrThrowAsync(NextTokensSearchValues);

            if (reader.SingleValue() == ')')
                return;

            await reader.MoveOrThrowAsync(2);
        }
    }

    public bool CanProcess(byte value) => value == Token;

    public byte[] Tokens => Tokenss;
}