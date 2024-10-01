using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors;

internal interface IProcessor
{
    Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer);
}