using System.IO;
using System.Numerics;

namespace PdfMerger.Pdf.Structs;

internal readonly record struct PdfReferenceValue(int Number, int Generation)
{
    public override string ToString() => $"{Number} {Generation} R";
    public byte[] GetReferenceBytes()
    {
        Span<byte> referenceBytes = stackalloc byte[23];
        var currentIndex = ReadBytes(referenceBytes);

        referenceBytes[currentIndex++] = (byte)' ';
        referenceBytes[currentIndex++] = (byte)'R';

        return referenceBytes[..currentIndex].ToArray();
    }

    public byte[] GetObjBytes()
    {
        Span<byte> referenceBytes = stackalloc byte[26];
        var currentIndex = ReadBytes(referenceBytes);

        referenceBytes[currentIndex++] = (byte)' ';
        referenceBytes[currentIndex++] = (byte)'o';
        referenceBytes[currentIndex++] = (byte)'b';
        referenceBytes[currentIndex++] = (byte)'j';

        return referenceBytes[..currentIndex].ToArray();
    }

    public int ReadBytes(Span<byte> referenceBytes)
    {
        var currentIndex = 0;
        Span<byte> bytes = stackalloc byte[10];
        if (!Number.TryFormat(bytes, out int bytesWritten))
            throw new InvalidOperationException("Failed to write reference number");
        
        bytes[..bytesWritten].CopyTo(referenceBytes);

        currentIndex = bytesWritten;
        referenceBytes[currentIndex++] = (byte)' ';

        if (!Generation.TryFormat(bytes, out bytesWritten))
            throw new InvalidOperationException("Failed to write reference generation");

        bytes[..bytesWritten].CopyTo(referenceBytes[currentIndex++..]);

        return currentIndex;
    }

    public static PdfReferenceValue operator +(PdfReferenceValue reference, int number)
    {
        return reference with
        {
            Number = reference.Number + number
        };
    }

}