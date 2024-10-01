using System.Text;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors.Types;

internal class StreamProcessor : IProcessor
{
    private int _count;
    public static readonly StreamProcessor Instance = new();
    private static readonly byte[] StreamLfToken = "stream\n"u8.ToArray();
    private static readonly byte[] StreamCrLfToken = "stream\r\n"u8.ToArray();
    private static readonly byte[] EndToken = "\nendstream"u8.ToArray();
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        if (!await reader.StartWithAsync(StreamLfToken) && !await reader.StartWithAsync(StreamCrLfToken))
            return false;

        context.PdfWriter.WriteNewLine();
        await context.PdfWriter.WriteAsync(Encoding.ASCII.GetBytes($"{(++_count).ToString()}-"));
        return await context.PdfWriter.WriteAndMoveAtIndexOfAsync(reader, EndToken) && 
               await reader.NextTokenAsync();
    }
}