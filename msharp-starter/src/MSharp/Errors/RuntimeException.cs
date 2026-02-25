namespace MSharp.Errors;

public sealed class RuntimeException : MSharpException
{
    public RuntimeException(string message, int line, int column)
        : base(message, line, column)
    {
    }
}
