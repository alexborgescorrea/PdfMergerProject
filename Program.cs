using System.Text.Json;
using PdfMerger.Pdf;
using PdfMerger.Pdf.Processors;using PdfMerger.Pdf.Writers;

const string basePath = @"C:\Users\paula.mattos\Desktop\Teste\PdfMergerProject\Samples";
var referencesPath = Path.Combine(basePath, "referencias.json");
var sourcePath1 = Path.Combine(basePath, "30150625-criacao-de-pdf-a.pdf");
var sourcePath2 = Path.Combine(basePath, "teste.pdf");
var sourcePath3 = Path.Combine(basePath, "Manual_das_normas_ABNT_Biblioteca_Ministro_Moreira_Alves_2021.pdf");
var sourcePath4 = Path.Combine(basePath, "Arquivo 100 paginas.pdf");
var destinationPath = Path.Combine(basePath, "resultado.pdf");

await using var referencesStream = new FileStream(referencesPath, FileMode.Truncate, FileAccess.Write);
await using var sourceStream1 = new FileStream(sourcePath1, FileMode.Open, FileAccess.Read);
await using var sourceStream2 = new FileStream(sourcePath2, FileMode.Open, FileAccess.Read);
await using var sourceStream3 = new FileStream(sourcePath3, FileMode.Open, FileAccess.Read);
await using var sourceStream4 = new FileStream(sourcePath4, FileMode.Open, FileAccess.Read);
await using var destinationStream = File.Exists(destinationPath) ? new FileStream(destinationPath, FileMode.Truncate, FileAccess.ReadWrite) 
                                                                 : new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

var context = new PdfContext();
await new PdfProcessor().ProcessAsync(context, new PdfWriter(destinationStream), sourceStream1, sourceStream2, sourceStream3, sourceStream4);

//await JsonSerializer.SerializeAsync(referencesStream, context.References, new JsonSerializerOptions { WriteIndented = true });

//await using var stream1 = await RemoverEspacosAsync(sourceStream1);
//await AdicionarEnterAsync(stream1);

//await using var stream2 = await RemoverEspacosAsync(destinationStream);
//await AdicionarEnterAsync(stream2);

Console.WriteLine("Finalizado!!!!");

async Task<FileStream> RemoverEspacosAsync(FileStream src)
{
    src.Seek(0, SeekOrigin.Begin);
    var path = $"{basePath}/Sem espaço/{Path.GetFileName(src.Name)}.txt";
    var file = File.Exists(path) ? new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite) 
                                   : new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    var buffer = new byte[1024];
    while (true)
    {
        var read = await src.ReadAsync(buffer, 0, buffer.Length);
        if (read == 0)
        {
            await src.FlushAsync();
            return file;
        }

        for (int i = 0; i < read; i++)
        {
            if (!PdfConstants.Spaces.Contains(buffer[i]))
            {
                file.WriteByte(buffer[i]);
            }
        }
    }
}

async Task AdicionarEnterAsync(FileStream src)
{
    src.Seek(0, SeekOrigin.Begin);
    var path = $"{basePath}/Sem espaço/{Path.GetFileName(src.Name)}.txt";
    await using var file = File.Exists(path) ? new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite) 
                                             : new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    var buffer = new byte[1024];
    while (true)
    {
        var read = await src.ReadAsync(buffer, 0, buffer.Length);
        if (read == 0)
        {
            await src.FlushAsync();
            return;
        }

        for (int i = 0; i < read; i++)
        {
            file.WriteByte(buffer[i]);
            await file.WriteAsync("\n"u8.ToArray());
        }
    }
}
