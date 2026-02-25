namespace MSharp.Errors;

public sealed class LexerException : MSharpException
{
    public LexerException(string message, int line, int column)
        : base(message, line, column)
    {
    }
}
