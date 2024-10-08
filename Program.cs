using System.Text;
using PdfMerger.Pdf;
using PdfMerger.Pdf.Processors;using PdfMerger.Pdf.Writers;

const string basePath = @"C:\Desenv\Pessoal\PdfMergerProject\Samples";
var referencesPath = Path.Combine(basePath, "referencias.json");
var sourcePath1 = Path.Combine(basePath, "2 exemplo1.pdf");
var sourcePath2 = Path.Combine(basePath, "Fitness Tip 4.pdf");
var sourcePath3 = Path.Combine(basePath, "ISO_32000-2_sponsored-ec2.pdf");
var sourcePath4 = Path.Combine(basePath, "pdf24_merged.pdf");
var sourcePath5 = Path.Combine(basePath, "pdfServico_57952bf8ca7af_24-07-2016_17-58-32.pdf");
var sourcePath6 = Path.Combine(basePath, "manual_alimentacao_nutricao.pdf");
var destinationPath = Path.Combine(basePath, "resultado.pdf");

await using var referencesStream = new FileStream(referencesPath, FileMode.Truncate, FileAccess.Write);
await using var sourceStream1 = new FileStream(sourcePath1, FileMode.Open, FileAccess.Read);
await using var sourceStream2 = new FileStream(sourcePath2, FileMode.Open, FileAccess.Read);
await using var sourceStream3 = new FileStream(sourcePath3, FileMode.Open, FileAccess.Read);
await using var sourceStream4 = new FileStream(sourcePath4, FileMode.Open, FileAccess.Read);
await using var sourceStream5 = new FileStream(sourcePath5, FileMode.Open, FileAccess.Read);
await using var sourceStream6 = new FileStream(sourcePath6, FileMode.Open, FileAccess.Read);
await using var destinationStream = File.Exists(destinationPath) ? new FileStream(destinationPath, FileMode.Truncate, FileAccess.ReadWrite) 
                                                                 : new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

var files = Directory.GetFiles(Path.Combine(basePath, "massa-dados-pdfs/teste"));
var streams = files.Select(file => (Stream)new FileStream(file, FileMode.Open, FileAccess.Read)).ToArray();
try
{
    var context = new PdfContext();
    await using var writer = new PdfWriter(destinationStream);
    await new PdfProcessor().ProcessAsync(context, writer, streams.OrderBy(p => ((FileStream)p).Name).ToArray());       
}
finally
{
    foreach (var stream in streams)
    {
        stream.Dispose();
    }
}

Console.WriteLine();
Console.WriteLine("Finalizado!!!");

// var context = new PdfContext();
// //await new PdfProcessor().ProcessAsync(context, new PdfWriter(destinationStream), sourceStream1, sourceStream5, sourceStream6, sourceStream2);
// await new PdfProcessor().ProcessAsync(context, new PdfWriter(destinationStream), sourceStream2);
//
// //await JsonSerializer.SerializeAsync(referencesStream, context.References, new JsonSerializerOptions { WriteIndented = true });
//
// await using var stream1 = await RemoverEspacosAsync(sourceStream5);
// await AdicionarEnterAsync(stream1);
//
// await using var stream2 = await RemoverEspacosAsync(destinationStream);
// await AdicionarEnterAsync(stream2);
//
// Console.WriteLine("Finalizado!!!!");
//
// async Task<FileStream> RemoverEspacosAsync(FileStream src)
// {
//     src.Seek(0, SeekOrigin.Begin);
//     var path = $"{basePath}/Sem espaço/{Path.GetFileName(src.Name)}.txt";
//     var file = File.Exists(path) ? new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite) 
//                                    : new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
//     var buffer = new byte[1024];
//     while (true)
//     {
//         var read = await src.ReadAsync(buffer, 0, buffer.Length);
//         if (read == 0)
//         {
//             await src.FlushAsync();
//             return file;
//         }
//
//         for (int i = 0; i < read; i++)
//         {
//             if (!PdfConstants.Spaces.Contains(buffer[i]))
//             {
//                 file.WriteByte(buffer[i]);
//             }
//         }
//     }
// }
//
// async Task AdicionarEnterAsync(FileStream src)
// {
//     src.Seek(0, SeekOrigin.Begin);
//     var path = $"{basePath}/Sem espaço/{Path.GetFileName(src.Name)}.txt";
//     await using var file = File.Exists(path) ? new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite) 
//                                              : new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
//     var buffer = new byte[1024];
//     while (true)
//     {
//         var read = await src.ReadAsync(buffer, 0, buffer.Length);
//         if (read == 0)
//         {
//             await src.FlushAsync();
//             return;
//         }
//
//         for (int i = 0; i < read; i++)
//         {
//             file.WriteByte(buffer[i]);
//             await file.WriteAsync("\n"u8.ToArray());
//         }
//     }
// }
