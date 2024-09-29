using System.Text;

namespace PdfMerger.Pdf.Matchers.Base;

internal class OrMatcher : IMatcher
{
    private readonly byte[] _value1;
    private readonly byte[] _value2;

    public OrMatcher(byte[] value1, byte[] value2)
    {
        _value1 = value1;
        _value2 = value2;
    }
    
    public OrMatcher(string value1, string value2) 
        : this(Encoding.ASCII.GetBytes(value1), Encoding.ASCII.GetBytes(value2))
    {
    }

    public int Match(ReadOnlySpan<byte> bytes)
    {
        if (bytes.StartsWith(_value1))
            return _value1.Length -1;
        
        if (bytes.StartsWith(_value2))
            return _value2.Length -1;

        return -1;
    }
}