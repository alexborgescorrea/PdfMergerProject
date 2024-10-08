namespace PdfMerger.Pdf.Structs
{
    internal readonly record struct PdfCatalog
    {
        public PdfReferenceValue Obj { get; init; }
        public PdfReferenceValue Pages { get; init; }
    }
}
