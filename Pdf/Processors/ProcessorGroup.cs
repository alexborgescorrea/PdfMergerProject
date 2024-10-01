using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

internal class ProcessorGroup
{
    private readonly IProcessor[] _processors;
    private readonly IProcessor? _endProcessor;
    
    public ProcessorGroup(IProcessor[] processors, IProcessor? endProcessor = null)
    {
        _processors = processors;
        _endProcessor = endProcessor;
    }
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader)
    {
        while (true)
        {
            var sucess = false;
            foreach (var processor in _processors)
            {
                sucess = await processor.ProcessAsync(context, reader);
                if (sucess)
                {
                    if (_endProcessor is null || processor == _endProcessor)
                        return true;
                    
                    break;
                }
            }

            if (sucess)
                continue;
            
            if (_endProcessor is null)
                return false;

            if (!await reader.NextTokenAsync())
                return false;
        }
    }
}