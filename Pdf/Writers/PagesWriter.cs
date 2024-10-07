using PdfMerger.Pdf.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfMerger.Pdf.Writers
{
    internal class PagesWriter
    {
        private static readonly byte[] PagesParent = "\n/Parent 2 0 R"u8.ToArray();
        private static readonly byte[] StartObjPages = "\n<2 0 obj\n<</Type/Pages /Kids["u8.ToArray();
        private static readonly byte[] CountPages = "]/Count "u8.ToArray();
        private static readonly byte[] EndObjPages = ">>\nendobj"u8.ToArray();

        private readonly Stream _stream;

        public PagesWriter(Stream stream)
        {
            _stream = stream;
        }

        public async ValueTask<PdfXRefItem> WriteObjPagesAsync(PdfContext context)
        {            
            var position = _stream.Position + 1;
            await _stream.WriteAsync(StartObjPages);

            foreach (var catalog in context.Catalogs)
            {
                await _stream.WriteAsync(catalog.Pages.GetReferenceBytes());
                _stream.WriteByte((byte)' ');
            }

            await _stream.WriteAsync(CountPages);
            await WritePagesCount(context);
            await _stream.WriteAsync(EndObjPages);

            return new(new(9999, 0), position, position);
        }

        private ValueTask WritePagesCount(PdfContext context)
        {
            Span<byte> bytes = stackalloc byte[10];
            if (context.PagesCount.TryFormat(bytes, out var numBytes))
                return _stream.WriteAsync(bytes[..numBytes].ToArray()); 

            throw new InvalidOperationException("Failed to write pages count");            
        }

        public ValueTask WritePagesParentAsync()
        {
            return _stream.WriteAsync(PagesParent);
        }
    }
}
