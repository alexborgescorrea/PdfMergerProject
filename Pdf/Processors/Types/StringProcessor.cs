using System.Buffers;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types;

internal class StringProcessor : IProcessor
{
    public static readonly StringProcessor Instance = new();
    
    private const byte BackSlashToken = 0x5C;
    private static readonly SearchValues<byte> NextTokensSearchValues = SearchValues.Create([(byte)')', BackSlashToken]);
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        if (reader.Value != '(')
            return false;
        
        while (true)
        {
            if (!await reader.FindAnyAndMoveAsync(NextTokensSearchValues))
                return false;

            if (reader.Value == ')')
                return true;

            if (!await reader.MoveAsync(2))
                return false;
        }
    }
}