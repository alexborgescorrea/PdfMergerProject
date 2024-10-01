using System.Text.Json;
using PdfMerger.Pdf;
using PdfMerger.Pdf.Processors;
using PdfMerger.Pdf.Readers;

const string basePath = @"C:\Desenv\Pessoal\PdfMergerProject\Samples\";
var referencesPath = Path.Combine(basePath, "referencias.json");
var sourcePath1 = Path.Combine(basePath, "Fitness Tip 4.pdf");
var sourcePath2 = Path.Combine(basePath, "exemplo4.pdf");
var destinationPath = Path.Combine(basePath, "resultado.pdf");

await using var referencesStream = new FileStream(referencesPath, FileMode.Truncate, FileAccess.Write);
await using var sourceStream1 = new FileStream(sourcePath1, FileMode.Open, FileAccess.Read);
await using var sourceStream2 = new FileStream(sourcePath2, FileMode.Open, FileAccess.Read);
await using var destinationStream = File.Exists(destinationPath) ? new FileStream(destinationPath, FileMode.Truncate, FileAccess.Write) 
                                                                 : new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write);

var context = new PdfContext();
await new PdfProcessor().ProcessAsync(context, new(sourceStream1), new(destinationStream));

await JsonSerializer.SerializeAsync(referencesStream, context.References, new JsonSerializerOptions { WriteIndented = true });

Console.WriteLine("Finalizado!!!!");
