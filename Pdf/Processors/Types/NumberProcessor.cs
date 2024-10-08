using PdfMerger.Pdf.Matchers;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Structs;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types;

internal class NumberProcessor : IProcessor
{
    public static readonly NumberProcessor Instance = new();
    private const int MaxIdentifierLength = 255;
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        try
        {
            var chunk = await reader.ChunkAsync(MaxIdentifierLength);
            var index = NumberTypeMatcher.Instance.Match(chunk.Span);
            if (index == -1)
                return false;

            chunk = chunk[..index];
            UpdateContext(context, chunk.Span);
            await writer.WriteWithSpaceAsync(chunk);
            return await reader.MoveAsync(index);
        }
        catch
        {
            throw;
        }
    }
    
    private void UpdateContext(PdfContext context, ReadOnlySpan<byte> chunk)
    {
        if (context.Scope.Level != 1)
            return;

        if (context.Scope.KeyName == KeyName.ObjStmN)
        {
            if (!int.TryParse(chunk, out var objStmN))
                return;
            
            context.Scope = context.Scope with
            {
                ObjStm = context.Scope.ObjStm with { N = objStmN }
            };

            return;
        }
        
        if (context.Scope.KeyName == KeyName.ObjStmFirst)
        {
            if (!long.TryParse(chunk, out var objStmFirst))
                return;
            
            context.Scope = context.Scope with
            {
                ObjStm = context.Scope.ObjStm with { First = objStmFirst }
            };

            return;
        }
    }
}