using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

internal class CommentProcessor : IProcessor
{
    private const byte Token = (byte)'%';
    private static readonly byte[] Tokenss = [Token];
    public static CommentProcessor Instance { get; } = new();
    
    public Task ProcessAsync(PdfReader reader, byte value) => reader.NextLineAsync();
    
    public bool CanProcess(byte value) => value == Token;
    public byte[] Tokens => Tokenss;
}