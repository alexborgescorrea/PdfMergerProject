using PdfMerger.Pdf.Processors.Types.Arrays;
using PdfMerger.Pdf.Processors.Types.Dictionaries;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types;

internal class TypesProcessor : IProcessor
{
    public static readonly TypesProcessor Instance = new();
    
    private static readonly ProcessorGroup ProcessorGroup = new 
    (
        [ 
            ReferenceProcessor.Instance,
            StartDictionaryProcessor.Instance, 
            NameProcessor.Instance,
            NumberProcessor.Instance,
            StringProcessor.Instance,
            StartArrayProcessor.Instance,
            BooleanProcessor.Instance, 
            NullProcessor.Instance,
            StreamProcessor.Instance,
            HexadecimalProcessor.Instance 
        ]
    );
    
    public Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        return ProcessorGroup.ProcessAsync(context, reader, writer);
    }
}