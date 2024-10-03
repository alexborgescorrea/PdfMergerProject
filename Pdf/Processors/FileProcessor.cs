using PdfMerger.Pdf.Processors.Footer;
using PdfMerger.Pdf.Processors.Objs;
using PdfMerger.Pdf.Processors.Types;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors;

internal class FileProcessor : IProcessor
{
    private static readonly ProcessorGroup ProcessorGroup = new
    (
        [
            CommentProcessor.Instance, 
            ObjProcessor.Instance, 
            StringProcessor.Instance, 
            HexadecimalProcessor.Instance, 
            FooterProcessor.Instance,
            StartXRefProcessor.Instance
        ], 
        XRefProcessor.Instance
    );
    public Task<bool> ProcessAsync(PdfContext context, PdfReader reader, IPdfWriter writer)
    {
        return ProcessorGroup.ProcessAsync(context, reader, writer);
    }
}
