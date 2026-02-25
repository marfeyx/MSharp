namespace MSharp.Lexing;

public sealed record Token(TokenType Type, string Lexeme, int Line, int Column)
{
    public override string ToString() => $"{Type} '{Lexeme}' ({Line}:{Column})";
}
