namespace PdfMerger.Pdf.Structs;

internal readonly record struct PdfXRefItem(PdfReferenceValue Obj, long Position, long OriginalPosition)
{
    public string Name => $"{Obj.Number} {Obj.Generation} obj";
    public override string ToString() => Obj.ToString();
}