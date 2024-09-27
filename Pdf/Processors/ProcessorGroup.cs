using System.Buffers;
using PdfMerger.Exceptions;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

internal class ProcessorGroup
{
    private readonly SearchValues<byte> _searchValues;
    public ProcessorGroup(IProcessor[] processors)
    {
        Processors = processors;
        _searchValues = SearchValues.Create(processors.SelectMany(p => p.Tokens).ToArray());
    }
    
    public IReadOnlyList<IProcessor> Processors { get; }

    public async Task<bool> ProcessAsync(PdfReader reader)
    {
        await reader.FindAnyAndMoveOrThrowAsync(_searchValues);
        var value = reader.SingleValue();
        var processor = ChoiceProcessor(value);
        if (processor is null)
            return false;
            
        await processor.ProcessAsync(reader, value);

        return true;
    }
    
    private IProcessor? ChoiceProcessor(byte value)
    {
        foreach (var processo in Processors)
        {
            if (processo.CanProcess(value))
                return processo;
        }

        return null;
    }
}