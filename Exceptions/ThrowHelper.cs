using System.Diagnostics.CodeAnalysis;

namespace PdfMerger.Exceptions;

internal static class ThrowHelper
{
    [DoesNotReturn]
    public static void ThrowInvalidPdf() => throw new InvalidOperationException("Invalid PDF file.");
}