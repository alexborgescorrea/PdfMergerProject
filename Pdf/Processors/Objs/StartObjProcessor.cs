﻿using PdfMerger.Pdf.Matchers;
using PdfMerger.Pdf.Matchers.Base;
using PdfMerger.Pdf.Processors.Types;
using PdfMerger.Pdf.Readers;
using PdfMerger.Pdf.Writers;

namespace PdfMerger.Pdf.Processors.Objs;

internal class StartObjProcessor : IProcessor
{
    public static readonly StartObjProcessor Instance = new();
    private const int MaxIdentifierLength = 255;
    private static readonly ProcessorGroup ProcessorGroup = new 
    (
        [ 
            CommentProcessor.Instance, 
            TypesProcessor.Instance, 
            EndObjProcessor.Instance 
        ], 
        EndObjProcessor.Instance
    );
    
    public async Task<bool> ProcessAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        if (!await ProcessInternalAsync(context, reader, writer))
            return false;
        
        return await ProcessorGroup.ProcessAsync(context, reader, writer);
    }
    
    private async Task<bool> ProcessInternalAsync(PdfContext context, PdfReader reader, PdfWriter writer)
    {
        var originalPosition = reader.Position;
        var chunk = await reader.ChunkAsync(MaxIdentifierLength);
        var index = StartObjMatcher.Instance.Match(chunk.Span);
        if (index == -1)
            return false;

        chunk = chunk[..index];
        await writer.WriteLineAsync(chunk);
        
        AddReference(context, reader, writer, originalPosition, chunk.Span);
        
        return await reader.MoveAsync(index);
    }

    private static void AddReference(PdfContext context, PdfReader reader, PdfWriter writer, long originalPosition, ReadOnlySpan<byte> chunk)
    {
        var index = NumberMatcher.Instance.Match(chunk) + 1;
        var number = int.Parse(chunk[..index]);
        
        chunk = chunk[++index..];
        
        index = NumberMatcher.Instance.Match(chunk) + 1;
        var generation = int.Parse(chunk[..index]);
        
        context.References.Add(new(number, generation, writer.Position, originalPosition));
    }
}