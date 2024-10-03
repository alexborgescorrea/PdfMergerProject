namespace PdfMerger.Pdf;

internal readonly record struct PdfScope
{
    public int Level { get; init; }
    public bool IsType { get; init; }
    public bool IsCatalog { get; init; }
}