using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Structs;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types;

internal class StreamProcessor : IProcessor
{
    public static readonly StreamProcessor Instance = new();
    private static readonly byte[] StreamLfToken = "stream\n"u8.ToArray();
    private static readonly byte[] StreamCrLfToken = "stream\r\n"u8.ToArray();
    private static readonly byte[] EndStream1Token = "\nendstream"u8.ToArray();
    private static readonly byte[] EndStream2Token = "\rendstream"u8.ToArray();
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (context.Scope.ObjType == ObjType.ObjStm)
            return false;
        
        if (!await reader.StartWithAsync(StreamLfToken) && !await reader.StartWithAsync(StreamCrLfToken))
            return false;

        writer.WriteNewLine();
        return await writer.WriteAndMoveAtAsync(reader, EndStream1Token, EndStream2Token) && 
               await reader.NextTokenAsync();
    }
}