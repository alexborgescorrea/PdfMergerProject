using System.Buffers;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Writers;

internal interface IPdfWriter
{
    public long Position { get; }
    ValueTask WriteEndObjAsync();
    ValueTask WriteLineAsync(ReadOnlyMemory<byte> bytes);
    ValueTask WriteStartArrayAsync();
    ValueTask WriteEndArrayAsync();
    ValueTask WriteStartDictionaryAsync();
    ValueTask WriteEndDictionaryAsync();
    ValueTask WriteBooleanAsync(byte value);
    void WriteNewLine();
    Task<bool> WriteAndMoveAtAsync(PdfReader reader, byte filter);
    Task<bool> WriteAndMoveAtAsync(PdfReader reader, Memory<byte> filter);
    ValueTask WriteStartNameAsync();
    Task<bool> WriteAndMoveAtDelimiterAsync(PdfReader reader);
    ValueTask WriteNullAsync();
    ValueTask WriteNullByteAsync();
    Task<bool> WriteAndMoveAtIndexOfAnyAsync(PdfReader reader, SearchValues<byte> filter);
    Task<bool> CopyAndMovieAsync(PdfReader reader, int length);
    Task WriteHeaderAsync();
    Task WriteBitStringAsync();
    Task<long> WriteReferencesAsync(IReadOnlyList<PdfReference> references);
    ValueTask WriterTrailerAsync(PdfReference root, IReadOnlyList<PdfReference> references);
    ValueTask WriteStartXRefAsync(long xrefOffset);
    Task FlushAsync();
}