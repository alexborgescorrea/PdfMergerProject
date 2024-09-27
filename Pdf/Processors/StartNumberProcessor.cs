using System.Text;
using System.Text.RegularExpressions;
using PdfMerger.Pdf.Readers;

namespace PdfMerger.Pdf.Processors;

public class StartNumberProcessor : IProcessor
{
    private static readonly byte[] Tokenss = [ (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9' ];
    public async Task ProcessAsync(PdfReader reader, byte value)
    {
        var objName = await TryGetObjAsync(reader, value);
    }

    private async Task<string> TryGetObjAsync(PdfReader reader, byte value)
    {
        var regex = new Regex("");
        var values = await reader.PeekAsync(50);
        regex.Match();
        var part = 1;
        var sb = new StringBuilder();
        sb.Append(reader.SingleValue());
        
        while (true)
        {
            await reader.MoveOrThrowAsync();

            var singleValue = reader.SingleValue();
            if (part == 1 && IsNumber(singleValue))
            {
                sb.Append(singleValue);
                await reader.MoveOrThrowAsync();
                continue;
            }

            if (part == 1 && reader.CurrentTokenIsSpace())
            {
                part++;
                sb.Append(' ');
                await reader.NextNonSpaceValueOrThrowAsync();
                continue;
            }

            if (part == 2 && IsNumber(singleValue))
            {
                sb.Append(singleValue);
                await reader.MoveOrThrowAsync();
                continue;
            }
            
            if (part == 3 && IsObj(singleValue))
            {
                sb.Append(singleValue);
                await reader.MoveOrThrowAsync();
                continue;
            }
        }
    }

    public bool CanProcess(byte value) => IsNumber(value);

    public byte[] Tokens => Tokenss;
    
    private static bool IsNumber(byte value) => ((char)value) is > '0' and < '9';
    
    private static bool IsObj(byte value) => ((char)value) is > '0' and < '9';
}