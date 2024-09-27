using PdfMerger.Pdf.Mergers;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

internal interface IProcessor
{
    Task ProcessAsync(PdfReader reader, byte value);
    bool CanProcess(byte value);
    
    byte[] Tokens { get; }
}