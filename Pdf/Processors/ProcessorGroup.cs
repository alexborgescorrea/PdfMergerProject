using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

internal class ProcessorGroup
{
    private readonly IProcessor? _endProcessor;
    
    public ProcessorGroup(IProcessor[] processors, IProcessor? endProcessor = null)
    {
        Processors = processors;
        _endProcessor = endProcessor;
    }
    
    public IReadOnlyList<IProcessor> Processors { get; }

    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader)
    {
        while (true)
        {
            foreach (var processor in Processors)
            {
                if (await processor.ProcessAsync(context, reader))
                {
                    if (_endProcessor is null || processor == _endProcessor)
                        return true;
                    
                    break;
                }
            }

            if (!await reader.MoveAsync(1))
                return false;
        }
    }
}