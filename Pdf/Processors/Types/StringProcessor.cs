using System.Buffers;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types;

internal class StringProcessor : IProcessor
{
    public static readonly StringProcessor Instance = new();
    
    private const byte BackSlashToken = 0x5C;
    private static readonly SearchValues<byte> NextTokensSearchValues = SearchValues.Create([(byte)')', BackSlashToken]);
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (reader.Value != '(')
            return false;
        
        writer.WriteSpace();
        while (true)
        {
            if (!await writer.WriteAndMoveAtIndexOfAnyAsync(reader, NextTokensSearchValues))
                return false;

            if (reader.Value == ')')
                return await reader.NextTokenAsync();

            if (!await reader.MoveAsync(1))
                return false;
            
            if (!await writer.CopyAndMovieAsync(reader, 2))
                return false;
        }
    }
}