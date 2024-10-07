using PdfMerger.Pdf.Matchers.Base;
using PdfMerger.Pdf.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Span<char> part = stackalloc char[10];

            var numberEndIndex = NumberMatcher.Instance.Match(chunk) + 1;
            var spaceEndIndex = numberEndIndex + SpaceMatcher.Instance.Match(chunk[numberEndIndex..]) + 1;
            var generationEndIndex = spaceEndIndex + NumberMatcher.Instance.Match(chunk[spaceEndIndex..]) + 1;

            var num = Encoding.ASCII.GetChars(chunk[..numberEndIndex], part);
            var number = int.Parse(part[..num]);

            num = Encoding.ASCII.GetChars(chunk[spaceEndIndex..generationEndIndex], part);
            var generation = int.Parse(part[..num]);

            return new(number, generation);
        }
    }
}
