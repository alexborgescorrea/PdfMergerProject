using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types;

internal class HexadecimalProcessor : IProcessor 
{
    public static readonly HexadecimalProcessor Instance = new();
    private static readonly byte[] Tokens = [(byte)'<', (byte)'<'];
    private static readonly byte GreaterThanToken = (byte)'>';
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        if (reader.Value != '<')
            return false;
        
        var chunk = await reader.ChunkAsync(2);
        if (chunk.Span.SequenceEqual(Tokens))
            return false;

        context.PdfWriter.WriteNewLine();
        return await context.PdfWriter.WriteAndMoveAtIndexOfAsync(reader, GreaterThanToken) && 
               await reader.NextTokenAsync();
    }
}