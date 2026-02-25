namespace MSharp.Errors;

public abstract class MSharpException : Exception
{
    protected MSharpException(string message, int line, int column)
        : base(message)
    {
        Line = line;
        Column = column;
    }

    public int Line { get; }
    public int Column { get; }

    public virtual string ToDisplayString() => $"{Message} (line {Line}, col {Column})";
}
