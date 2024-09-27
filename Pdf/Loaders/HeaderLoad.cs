// using System.Text;
//
// namespace Teste_PdfMerge.Pdf.Loaders;
//
// public static class HeaderLoad
// {
//     private static byte[] _headerBytes = "%PDF-"u8.ToArray();
//     private static byte[] _headerWithNemLineBytes = "/n%PDF-"u8.ToArray();
//     
//     public static async Task<bool> Load(Span<byte> line)
//     {
//         while (true)
//         {
//             var num = await stream.ReadAsync(buffer);
//             if (num == 0)
//                 return false;
//             
//             if (buffer.Span.SequenceEqual(_headerBytes))
//             {
//                 pdfStruct.Header = Encoding.UTF8.GetString(buffer.Span);
//                 return true;
//             }
//         }
//     }
// }