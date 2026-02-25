using MSharp.Errors;

namespace MSharp.Lexing;

public sealed class Lexer
{
    private static readonly Dictionary<string, TokenType> Keywords = new(StringComparer.Ordinal)
    {
        ["has"] = TokenType.Has,
        ["value"] = TokenType.Value,
        ["means"] = TokenType.Means,
        ["say"] = TokenType.Say,
        ["do"] = TokenType.Do,
        ["and"] = TokenType.And,
        ["use"] = TokenType.Use,
        ["set"] = TokenType.Set,
        ["to"] = TokenType.To,
        ["if"] = TokenType.If,
        ["else"] = TokenType.Else,
        ["while"] = TokenType.While
    };

    private readonly string _source;
    private readonly List<Token> _tokens = [];

    private int _index;
    private int _line = 1;
    private int _column = 1;

    public Lexer(string source)
    {
        _source = source ?? string.Empty;
    }

    public IReadOnlyList<Token> Tokenize()
    {
        while (!IsAtEnd())
        {
            SkipWhitespaceAndComments();
            if (IsAtEnd())
            {
                break;
            }

            var startLine = _line;
            var startColumn = _column;
            var c = Peek();

            switch (c)
            {
                case ';':
                    Advance();
                    AddToken(TokenType.Semicolon, ";", startLine, startColumn);
                    break;
                case ',':
                    Advance();
                    AddToken(TokenType.Comma, ",", startLine, startColumn);
                    break;
                case '{':
                    Advance();
                    AddToken(TokenType.LeftBrace, "{", startLine, startColumn);
                    break;
                case '}':
                    Advance();
                    AddToken(TokenType.RightBrace, "}", startLine, startColumn);
                    break;
                case '"':
                    ReadString();
                    break;
                default:
                    if (IsIdentifierStart(c))
                    {
                        ReadIdentifierOrKeyword();
                    }
                    else if (IsNumberStart())
                    {
                        ReadNumber();
                    }
                    else
                    {
                        throw new LexerException($"Unexpected character '{c}'.", startLine, startColumn);
                    }
                    break;
            }
        }

        _tokens.Add(new Token(TokenType.EndOfFile, string.Empty, _line, _column));
        return _tokens;
    }

    private void SkipWhitespaceAndComments()
    {
        while (!IsAtEnd())
        {
            if (char.IsWhiteSpace(Peek()))
            {
                Advance();
                continue;
            }

            if (Peek() == '/' && PeekNext() == '/')
            {
                while (!IsAtEnd() && Peek() != '\n')
                {
                    Advance();
                }
                continue;
            }

            break;
        }
    }

    private void ReadString()
    {
        var startLine = _line;
        var startColumn = _column;
        Advance(); // opening quote

        var chars = new List<char>();
        while (!IsAtEnd())
        {
            var c = Advance();
            if (c == '"')
            {
                AddToken(TokenType.String, new string(chars.ToArray()), startLine, startColumn);
                return;
            }

            if (c == '\\')
            {
                if (IsAtEnd())
                {
                    throw new LexerException("Unterminated string literal.", startLine, startColumn);
                }

                var escaped = Advance();
                chars.Add(escaped switch
                {
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    '"' => '"',
                    '\\' => '\\',
                    _ => escaped
                });
                continue;
            }

            if (c == '\n')
            {
                throw new LexerException("Unterminated string literal.", startLine, startColumn);
            }

            chars.Add(c);
        }

        throw new LexerException("Unterminated string literal.", startLine, startColumn);
    }

    private void ReadIdentifierOrKeyword()
    {
        var startLine = _line;
        var startColumn = _column;
        var startIndex = _index;

        Advance();
        while (!IsAtEnd() && IsIdentifierPart(Peek()))
        {
            Advance();
        }

        var text = _source[startIndex.._index];
        if (Keywords.TryGetValue(text, out var keywordType))
        {
            AddToken(keywordType, text, startLine, startColumn);
        }
        else
        {
            AddToken(TokenType.Identifier, text, startLine, startColumn);
        }
    }

    private void ReadNumber()
    {
        var startLine = _line;
        var startColumn = _column;
        var startIndex = _index;

        if (Peek() == '-')
        {
            Advance();
        }

        var sawDigit = false;
        while (!IsAtEnd() && char.IsDigit(Peek()))
        {
            sawDigit = true;
            Advance();
        }

        if (!IsAtEnd() && Peek() == '.')
        {
            Advance();
            while (!IsAtEnd() && char.IsDigit(Peek()))
            {
                sawDigit = true;
                Advance();
            }
        }

        if (!sawDigit)
        {
            throw new LexerException("Invalid number literal.", startLine, startColumn);
        }

        var text = _source[startIndex.._index];
        AddToken(TokenType.Number, text, startLine, startColumn);
    }

    private bool IsNumberStart()
    {
        if (char.IsDigit(Peek()))
        {
            return true;
        }

        return Peek() == '-' && char.IsDigit(PeekNext());
    }

    private bool IsIdentifierStart(char c) => char.IsLetter(c) || c == '_';
    private bool IsIdentifierPart(char c) => char.IsLetterOrDigit(c) || c == '_';

    private char Peek() => _index < _source.Length ? _source[_index] : '\0';
    private char PeekNext() => _index + 1 < _source.Length ? _source[_index + 1] : '\0';

    private char Advance()
    {
        var c = _source[_index++];
        if (c == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }

        return c;
    }

    private bool IsAtEnd() => _index >= _source.Length;

    private void AddToken(TokenType type, string lexeme, int line, int column)
        => _tokens.Add(new Token(type, lexeme, line, column));
}
