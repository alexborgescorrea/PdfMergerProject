using PdfMerger.Pdf.Processors.Types.Dictionaries;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Footer;

internal class TrailerProcessor : IProcessor
{
    public static readonly TrailerProcessor Instance = new();
    private static readonly ProcessorGroup ProcessorGroup = new 
    (
        [ 
            DictionaryProcessor.Instance, 
            CommentProcessor.Instance,
        ], 
        DictionaryProcessor.Instance
    );
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        return await reader.NextTokenAsync() &&
               await ProcessorGroup.ProcessAsync(context, reader, writer);
    }
}