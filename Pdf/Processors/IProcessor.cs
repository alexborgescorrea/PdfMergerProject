using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

internal interface IProcessor
{
    Task<bool> ProcessAsync(PdfContext context, PdfReader2 reader);
}