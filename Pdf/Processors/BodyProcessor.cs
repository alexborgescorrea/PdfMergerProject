using PdfMerger.Pdf.Processors.Objs;
using PdfMerger.Pdf.Processors.Types;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

internal class BodyProcessor
{
    private static readonly CommentProcessor CommentProcessor = new ();
    private static readonly ProcessorGroup ProcessorGroup = new([CommentProcessor.Instance, StartObjProcessor.Instance, StringProcessor.Instance, HexadecimalProcessor.Instance, XRefProcessor.Instance], XRefProcessor.Instance);
    public async Task ProcessAsync(PdfContext context, PdfReader reader)
    {
        await ProcessorGroup.ProcessAsync(context, reader);
    }
}
