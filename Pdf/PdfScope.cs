namespace PdfMerger.Pdf;

internal readonly record struct PdfScope
{
    public int Level { get; init; }
    public bool IsTypeKey { get; init; }
    public bool IsCatalogType { get; init; }
    public bool IsPagesType { get; init; }
    public PdfReferenceValue Pages { get; init; }
    public PdfReferenceValue PageLabels { get; init; }
}