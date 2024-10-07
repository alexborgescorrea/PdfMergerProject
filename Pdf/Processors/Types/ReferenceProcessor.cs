using System.Text;
using PdfMerger.Pdf.Matchers;
using PdfMerger.Pdf.Matchers.Base;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Structs;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types;

internal class ReferenceProcessor : IProcessor
{
    public static readonly ReferenceProcessor Instance = new();
    private const int MaxIdentifierLength = 255;
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        var chunk = await reader.ChunkAsync(MaxIdentifierLength);
        var index = ReferenceMatcher.Instance.Match(chunk.Span);
        if (index == -1)
            return false;

        chunk = chunk[..index];
        var reference = chunk.ExtractValueReference() + context.BaseReference;
        UpdateContext(context, reference);
        
        await writer.WriteReferenceValueAsync(reference);
        
        return await reader.MoveAsync(index);
    }

    private void UpdateContext(PdfContext context, PdfReferenceValue reference)
    {
        if (context.Scope.Level != 1)
            return;

        if (context.Scope.KeyName == KeyName.Pages)
            context.Scope = context.Scope with { Pages = reference };        
    }
}