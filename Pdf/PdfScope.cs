using PdfMerger.Pdf.Structs;

namespace PdfMerger.Pdf;

internal readonly record struct PdfScope
{
    public int Level { get; init; }
    public KeyName KeyName { get; init; }
    public ObjType ObjType { get; init; }        
    public PdfReferenceValue Pages { get; init; }    
    public PdfReferenceValue PageLabels { get; init; }
    public bool HasPagesParent { get; init; }
    public bool IsTypeKey => KeyName == KeyName.Type;
}