using System.Text;
using PdfMerger.Pdf.Matchers;
using PdfMerger.Pdf.Matchers.Base;
using PdfMerger.Pdf.Readers;
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
        UpdateContext(context, chunk.Span);
        
        await writer.WriteLineAsync(chunk);
        
        return await reader.MoveAsync(index);
    }

    private void UpdateContext(PdfContext context, ReadOnlySpan<byte> chuck)
    {
        if (context.Scope.IsPagesType)
            context.Scope = context.Scope with { Pages = ExtractValueReference(chuck) };
    }
    
    private PdfReferenceValue ExtractValueReference(ReadOnlySpan<byte> chuck)
    {
        Span<char> part = stackalloc char[10];
        
        var numberEndIndex = NumberMatcher.Instance.Match(chuck) + 1;
        var spaceEndIndex = numberEndIndex + SpaceMatcher.Instance.Match(chuck[numberEndIndex..]) + 1;
        var generationEndIndex = spaceEndIndex + NumberMatcher.Instance.Match(chuck[spaceEndIndex..]) + 1;
        
        var num = Encoding.ASCII.GetChars(chuck[..numberEndIndex], part);
        var number = int.Parse(part[..num]);
        
        num = Encoding.ASCII.GetChars(chuck[spaceEndIndex..generationEndIndex], part);
        var generation = int.Parse(part[..num]);
        
        return new(number, generation);
    }
}