using System.IO.Compression;
using System.Text;
using PdfMerger.Pdf.Matchers.Base;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Structs;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Types;

internal class ObStmProcessor : IProcessor
{
    public static readonly ObStmProcessor Instance = new ObStmProcessor();
    private static readonly byte[] StreamToken = "stream"u8.ToArray();
    private static readonly byte[] StreamLfToken = "stream\n"u8.ToArray();
    private static readonly byte[] StreamCrLfToken = "stream\r\n"u8.ToArray();
    private static readonly byte[] StartStreamToken = "\nstream\n"u8.ToArray();
    private static readonly byte[] EndToken = "\nendstream"u8.ToArray();
    private static readonly byte[] DeflateHeader = [0x78, 0x9C];
    
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
        
        await using var readerDeflate = await CreateReaderDeflateAsync(reader, writer);
        _intenalReader.UpdateStream(readerDeflate);

        var numbers = await ProcessXRefAsync(context, _intenalReader);
        await WriteAsync(context, writer, numbers);
            
        return await reader.MoveAsync(EndToken.Length) && 
               await reader.NextTokenAsync();
    }
    
    private async Task WriteAsync(PdfContext context, PdfWriter writer, int[] numbers)
    {
        await using var xrefStream = CreateTempFileStream();
        await using var objStream = CreateTempFileStream();
        await using var objWriter = new PdfWriter(objStream);
        
        for (var i = 0; i < numbers.Length; i++)
        {
            await xrefStream.WriteAsync(Encoding.ASCII.GetBytes($"{numbers[i]} {objStream.Position} "));
            await TypesProcessor.Instance.ProcessAsync(context, _intenalReader, objWriter);
        }

        xrefStream.Seek(0, SeekOrigin.Begin);
        objStream.Seek(0, SeekOrigin.Begin);
        
        await using var deflate = new DeflateStream(writer.Stream, CompressionMode.Compress, true);

        await writer.WriteAsync(StartStreamToken);
        await writer.WriteAsync(DeflateHeader);
        await xrefStream.CopyToAsync(deflate);
        await objStream.CopyToAsync(deflate);
        await deflate.FlushAsync();
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
        await writer.WriteAndMoveAtAsync(reader, tempFile, EndToken, false);
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
}