namespace PdfMerger.Pdf;

internal readonly record struct PdfReference(int ObjectNumber, int GenerationNumber, long Position, long OriginalPosition)
{
    public string Name => $"{ObjectNumber} {GenerationNumber} obj";
    public override string ToString() => $"{ObjectNumber} {GenerationNumber} R";
}