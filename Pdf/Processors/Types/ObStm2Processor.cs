using System.IO.Compression;
using System.Text;
using PdfMerger.Pdf.Matchers.Base;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Structs;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types;

internal class ObStm2Processor : IProcessor
{
    public static readonly ObStm2Processor Instance = new ObStm2Processor();
    private static readonly byte[] StreamToken = "stream"u8.ToArray();
    private static readonly byte[] StreamLfToken = "stream\n"u8.ToArray();
    private static readonly byte[] StreamCrLfToken = "stream\r\n"u8.ToArray();
    private static readonly byte[] StartStreamToken = "\nstream\n"u8.ToArray();
    private static readonly byte[] EndStream1Token = "\nendstream"u8.ToArray();
    private static readonly byte[] EndStream2Token = "\rendstream"u8.ToArray();
    
    private readonly PdfReader _intenalReader = new();
    
    private const int BufferSize = 1024;
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (context.Scope.ObjType == ObjType.ObjStm && await reader.IsTokenAsync(StreamToken))
            return await ProcessInternalAsync(context, reader, writer);
        
        return false;
    }

    private async Task<bool> ProcessInternalAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (!await MoveStartStreamAsync(reader))
            return false;

        await writer.WriteAsync(StartStreamToken);
        await writer.WriteAsync(EndStream1Token);
        await writer.WriteEndObjAsync();
        
        await using var readerDeflate = await CreateReaderDeflateAsync(reader, writer);
        _intenalReader.UpdateStream(readerDeflate);

        var numbers = await ProcessXRefAsync(context, _intenalReader);
        await WriteAsync(context, writer, numbers);
            
        return await reader.MoveAsync(EndStream1Token.Length) && 
               await reader.NextTokenAsync();
    }
    
    private async Task WriteAsync(PdfContext context, PdfWriter writer, int[] numbers)
    {
        for (var i = 0; i < numbers.Length; i++)
        {
            var scope = context.Scope;
            
            var obj = new PdfReferenceValue(numbers[i], 0);
            context.References.Add(new(obj, writer.Position, 0));
            await writer.WriteStartObjAsync(obj);
            await TypesProcessor.Instance.ProcessAsync(context, _intenalReader, writer);
            await writer.WriteEndObjAsync();
            
            UpdatePageCountContext(context);
            context.Scope = scope;
        }
    }
    
    private static async Task<int[]> ProcessXRefAsync(PdfContext context, PdfReader reader)
    {
        var numebers = new int[context.Scope.ObjStm.N];
        for (var i = 0; i < context.Scope.ObjStm.N; i++)
        {
            var chunk = await reader.ChunkAsync(100);
            var index = ExtranctXRef(context, chunk.Span, out numebers[i]);
            if (!await reader.MoveAsync(index))
                return numebers;
                
            await reader.NextNonSpaceValueAsync();
        }
        
        return numebers;
    }

    private static int ExtranctXRef(PdfContext context, ReadOnlySpan<byte> chunk, out int number)
    {
        var numberEndIndex = NumberMatcher.Instance.Match(chunk) + 1;
        var spaceEndIndex = numberEndIndex + SpaceMatcher.Instance.Match(chunk[numberEndIndex..]) + 1;
        var offsetEndIndex = spaceEndIndex + NumberMatcher.Instance.Match(chunk[spaceEndIndex..]) + 1;

        number = int.Parse(chunk[..numberEndIndex]) + context.BaseReference;

        return offsetEndIndex;
    }
    
    private static async Task<Stream> CreateReaderDeflateAsync(PdfReader reader, PdfWriter writer)
    {
        var tempFile = CreateTempFileStream();
        await writer.WriteAndMoveAtAsync(reader, tempFile, EndStream1Token, EndStream2Token);
        tempFile.Seek(0, SeekOrigin.Begin);
        return new DeflateStream(tempFile, CompressionMode.Decompress);
    }
    
    private static Stream CreateTempFileStream()
    {
        return new FileStream(Path.GetTempFileName(), 
                              FileMode.Create, 
                              FileAccess.ReadWrite, 
                              FileShare.Delete, 
                              BufferSize, 
                              FileOptions.Asynchronous | FileOptions.DeleteOnClose | FileOptions.SequentialScan);
    }    
    
    private static async Task<bool> MoveStartStreamAsync(PdfReader reader)
    {
        if (await reader.StartWithAsync(StreamLfToken))
            return await reader.MoveAsync(StreamLfToken.Length + 2);
        
        if (await reader.StartWithAsync(StreamCrLfToken))
            return await reader.MoveAsync(StreamCrLfToken.Length + 2);

        return false;
    }
    
    private static void UpdatePageCountContext(PdfContext context)
    {
        if (context.Scope.ObjType == ObjType.Page)
            context.PagesCount++;
    }
}