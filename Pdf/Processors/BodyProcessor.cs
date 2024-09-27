using System.Buffers;
using PdfMerger.Exceptions;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

internal class BodyProcessor
{
    private static readonly CommentProcessor CommentProcessor = new ();
    private static readonly ProcessorGroup ProcessorGroup = new ([ CommentProcessor.Instance, StringProcessor.Instance, HexadecimalProcessor.Instance ]);
    private static readonly SearchValues<byte> SearchValues = System.Buffers.SearchValues.Create(ProcessorGroup.Processors.SelectMany(p => p.Tokens).ToArray());
    

    public Task ProcessAsync(PdfReader reader)
    {
        return ProcessorGroup.ProcessAsync(reader);
    }
}
