using PdfMerger.Pdf.Processors.Objs;
using PdfMerger.Pdf.Processors.Types;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

internal class BodyProcessor : IProcessor
{
    private static readonly ProcessorGroup ProcessorGroup = new([CommentProcessor.Instance, StartObjProcessor.Instance, StringProcessor.Instance, HexadecimalProcessor.Instance, XRefProcessor.Instance], XRefProcessor.Instance);
    public Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        return ProcessorGroup.ProcessAsync(context, reader);
    }
}
