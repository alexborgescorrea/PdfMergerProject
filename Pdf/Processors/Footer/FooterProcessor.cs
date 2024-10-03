using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Footer;

internal class FooterProcessor : IProcessor
{
    public static readonly FooterProcessor Instance = new();
    private static readonly ProcessorGroup ProcessorGroup = new 
    (
        [ 
            TrailerProcessor.Instance, 
            StartXRefProcessor.Instance,
        ] 
    );
    
    private readonly PdfWriter _dummyWriter = new(new DummyStream());
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, IPdfWriter writer)
    {
        if (!await XRefProcessor.Instance.ProcessAsync(context, reader, writer))
            return false;
        
        return await ProcessorGroup.ProcessAsync(context, reader, _dummyWriter);
    }
}