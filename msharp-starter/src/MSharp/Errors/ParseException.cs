namespace MSharp.Errors;

public sealed class ParseException : MSharpException
{
    public ParseException(string message, int line, int column)
        : base(message, line, column)
    {
    }
}
