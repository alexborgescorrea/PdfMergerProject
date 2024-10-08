using PdfMerger.Pdf.Matchers.Base;
using PdfMerger.Pdf.Structs;
using System.Text;
using PdfMerger.Pdf;

namespace PdfMerger.Helpers
{
    internal static class ChunkExtensions
    {
        public static PdfReferenceValue ExtractValueReference(this Memory<byte> chunk)
        {
            return ExtractValueReference(chunk.Span);
        }

        public static PdfReferenceValue ExtractValueReference(this ReadOnlyMemory<byte> chunk)
        { 
            return ExtractValueReference(chunk.Span);
        }

        public static PdfReferenceValue ExtractValueReference(this ReadOnlySpan<byte> chunk)
        {
            var numberEndIndex = NumberMatcher.Instance.Match(chunk) + 1;
            var spaceEndIndex = numberEndIndex + SpaceMatcher.Instance.Match(chunk[numberEndIndex..]) + 1;
            var generationEndIndex = spaceEndIndex + NumberMatcher.Instance.Match(chunk[spaceEndIndex..]) + 1;

            var number = int.Parse(chunk[..numberEndIndex]);
            var generation = int.Parse(chunk[spaceEndIndex..generationEndIndex]);

            return new(number, generation);
        }
    }
}
