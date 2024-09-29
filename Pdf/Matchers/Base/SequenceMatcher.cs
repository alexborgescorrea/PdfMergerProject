using System.Text;

namespace PdfMerger.Pdf.Matchers.Base;

internal class SequenceMatcher : IMatcher
{
    private readonly byte[] _value;

    public SequenceMatcher(byte[] value)
    {
        _value = value;
    }
    
    public SequenceMatcher(string value) 
        : this(Encoding.ASCII.GetBytes(value))
    {
    }

    public int Match(ReadOnlySpan<byte> bytes)
    {
        if (bytes.StartsWith(_value))
            return _value.Length -1;

        return -1;
    }
}