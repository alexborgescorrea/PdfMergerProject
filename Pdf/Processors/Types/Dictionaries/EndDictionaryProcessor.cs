using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Structs;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types.Dictionaries;

internal class EndDictionaryProcessor : IProcessor
{
    public static readonly EndDictionaryProcessor Instance = new();
    
    private static readonly byte[] Tokens = [(byte)'>', (byte)'>'];

    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        var chunk = await reader.ChunkAsync(2);
        if (chunk.Span.SequenceEqual(Tokens) && await reader.MoveAsync(2))
        {
            await WriteExtraPropertiesInObjAsync(context, reader, writer);
            await writer.WriteEndDictionaryAsync();
            return true;
        }

        return false;
    }

    private static ValueTask WriteExtraPropertiesInObjAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (context.Scope.Level != 1)
            return ValueTask.CompletedTask;

        if (context.Scope.ObjType != ObjType.Pages)
            return ValueTask.CompletedTask;

        if (context.Scope.HasPagesParent)
            return ValueTask.CompletedTask;

        return writer.WritePagesParent();
    }
}