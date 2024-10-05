using System.IO;

namespace PdfMerger.Pdf.Structs;

internal readonly record struct PdfReferenceValue(int Number, int Generation)
{
    public override string ToString() => $"{Number} {Generation} R";
    public byte[] ToArray() 
    {
        var currentIndex = 0;
        Span<byte> bytes = stackalloc byte[10];
        if (!Number.TryFormat(bytes, out int bytesWritten))
            throw new InvalidOperationException("Failed to write reference number");

        Span<byte> referenceBytes = stackalloc byte[23];
        bytes[..bytesWritten].CopyTo(referenceBytes);

        currentIndex = bytesWritten;
        referenceBytes[currentIndex++] = (byte)' ';

        if (!Generation.TryFormat(bytes, out bytesWritten))
            throw new InvalidOperationException("Failed to write reference generation");

        bytes[..bytesWritten].CopyTo(referenceBytes[currentIndex++..]);

        referenceBytes[currentIndex++] = (byte)' ';
        referenceBytes[currentIndex++] = (byte)'R';

        return referenceBytes[..currentIndex].ToArray();
    }

}