﻿using PdfMerger.Pdf.Processors;
using PdfMerger.Pdf.Readers;

const string basePath = @"C:\Desenv\Pessoal\PdfMergerProject\Samples\";
var sourcePath1 = Path.Combine(basePath, "exemplo1.pdf");
var sourcePath2 = Path.Combine(basePath, "exemplo2.pdf");
var destinationPath = Path.Combine(basePath, "resultado.pdf");

await using var sourceStream1 = new FileStream(sourcePath1, FileMode.Open, FileAccess.Read);
await using var sourceStream2 = new FileStream(sourcePath2, FileMode.Open, FileAccess.Read);
await using var destinationStream = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write);

await new PdfProcessor().ProcessAsync(new PdfReader(sourceStream1));

Console.WriteLine("Finalizado!!!!");
