namespace PdfMerger.Pdf.Matchers.Base;

internal interface IMatcher
{
    int Match(ReadOnlySpan<byte> bytes);
}