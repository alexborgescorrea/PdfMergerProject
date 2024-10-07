using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfMerger.Pdf.Structs
{
    internal readonly struct PdfCatalog
    {
        public PdfReferenceValue Obj { get; init; }
        public PdfReferenceValue Pages { get; init; }
    }
}
