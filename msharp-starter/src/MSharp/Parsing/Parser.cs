using System.Globalization;
using MSharp.Ast;
using MSharp.Errors;
using MSharp.Lexing;

namespace MSharp.Parsing;

public sealed class Parser
{
    private readonly IReadOnlyList<Token> _tokens;
    private int _current;

    public Parser(IReadOnlyList<Token> tokens)
    {
        _tokens = tokens;
    }

    public IReadOnlyList<Stmt> ParseProgram()
    {
        var statements = new List<Stmt>();
        while (!IsAtEnd())
        {
            statements.Add(ParseStatement());
        }

        return statements;
    }

    private Stmt ParseStatement()
    {
        if (Match(TokenType.LeftBrace))
        {
            return ParseBlockStatement(Previous());
        }

        if (Match(TokenType.Say))
        {
            return ParseSayStatement(Previous());
        }

        if (Check(TokenType.Do))
        {
            var call = ParseCallExpression();
            RequireSemicolon("method call");
            return new CallStmt(call.StartToken, call);
        }

        if (Match(TokenType.Set))
        {
            return ParseSetStatement(Previous());
        }

        if (Match(TokenType.If))
        {
            return ParseIfStatement(Previous());
        }

        if (Match(TokenType.While))
        {
            return ParseWhileStatement(Previous());
        }

        if (Check(TokenType.Identifier))
        {
            return ParseIdentifierLedStatement();
        }

        var token = Peek();
        throw new ParseException($"Unexpected token '{token.LexemeOrType()}'.", token.Line, token.Column);
    }

    private Stmt ParseIdentifierLedStatement()
    {
        var identifier = Consume(TokenType.Identifier, "Expected identifier.");

        if (Match(TokenType.Has))
        {
            Consume(TokenType.Value, "Expected keyword 'value' after 'has'.");
            var numberToken = Consume(TokenType.Number, "Expected numeric literal after 'has value'.");
            if (!double.TryParse(numberToken.Lexeme, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
            {
                throw new ParseException($"Invalid number literal '{numberToken.Lexeme}'.", numberToken.Line, numberToken.Column);
            }

            RequireSemicolon("numeric assignment");
            return new NumberAssignmentStmt(identifier, identifier.Lexeme, new NumberLiteralExpr(numberToken, number));
        }

        if (Match(TokenType.Means))
        {
            var stringToken = Consume(TokenType.String, "Expected string literal after 'means'. Text values must use double quotes.");
            RequireSemicolon("text assignment");
            return new StringAssignmentStmt(identifier, identifier.Lexeme, new StringLiteralExpr(stringToken, stringToken.Lexeme));
        }

        throw new ParseException(
            $"Undefined variable '{identifier.Lexeme}'. Standalone identifiers are not valid statements.",
            identifier.Line,
            identifier.Column);
    }

    private SayStmt ParseSayStatement(Token sayToken)
    {
        var expr = ParseExpression();
        RequireSemicolon("say statement");
        return new SayStmt(sayToken, expr);
    }

    private SetFromCallStmt ParseSetStatement(Token setToken)
    {
        var identifier = Consume(TokenType.Identifier, "Expected variable name after 'set'.");
        Consume(TokenType.To, "Expected keyword 'to' after variable name.");

        if (!Check(TokenType.Do))
        {
            var bad = Peek();
            throw new ParseException("Expected method call after 'set <name> to'. Use: set x to do method and use ...;", bad.Line, bad.Column);
        }

        var call = ParseCallExpression();
        RequireSemicolon("set statement");
        return new SetFromCallStmt(setToken, identifier.Lexeme, call);
    }

    private IfStmt ParseIfStatement(Token ifToken)
    {
        var condition = ParseExpression();
        var thenBlock = ParseRequiredBlock("if");
        BlockStmt? elseBlock = null;

        if (Match(TokenType.Else))
        {
            elseBlock = ParseRequiredBlock("else");
        }

        return new IfStmt(ifToken, condition, thenBlock, elseBlock);
    }

    private WhileStmt ParseWhileStatement(Token whileToken)
    {
        var condition = ParseExpression();
        var body = ParseRequiredBlock("while");
        return new WhileStmt(whileToken, condition, body);
    }

    private BlockStmt ParseRequiredBlock(string keyword)
    {
        var brace = Consume(TokenType.LeftBrace, $"Expected '{{' to start {keyword} block.");
        return ParseBlockStatement(brace);
    }

    private BlockStmt ParseBlockStatement(Token openingBrace)
    {
        var statements = new List<Stmt>();
        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            statements.Add(ParseStatement());
        }

        Consume(TokenType.RightBrace, "Expected '}' to close block.");
        return new BlockStmt(openingBrace, statements);
    }

    private Expr ParseExpression()
    {
        if (Check(TokenType.Number))
        {
            var token = Advance();
            if (!double.TryParse(token.Lexeme, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            {
                throw new ParseException($"Invalid number literal '{token.Lexeme}'.", token.Line, token.Column);
            }

            return new NumberLiteralExpr(token, value);
        }

        if (Check(TokenType.String))
        {
            var token = Advance();
            return new StringLiteralExpr(token, token.Lexeme);
        }

        if (Check(TokenType.Identifier))
        {
            var token = Advance();
            return new VariableExpr(token, token.Lexeme);
        }

        if (Check(TokenType.Do))
        {
            return ParseCallExpression();
        }

        var bad = Peek();
        throw new ParseException($"Unexpected token '{bad.LexemeOrType()}' in expression.", bad.Line, bad.Column);
    }

    private CallExpr ParseCallExpression()
    {
        var doToken = Consume(TokenType.Do, "Expected 'do'.");
        var methodName = Consume(TokenType.Identifier, "Expected method name after 'do'.");
        Consume(TokenType.And, "Expected keyword 'and' after method name.");
        Consume(TokenType.Use, "Expected keyword 'use' after 'and'.");

        var args = new List<Expr>();
        if (!IsExpressionTerminator(Peek().Type))
        {
            args.Add(ParseExpression());
            while (Match(TokenType.Comma))
            {
                args.Add(ParseExpression());
            }
        }

        return new CallExpr(doToken, methodName.Lexeme, args);
    }

    private bool IsExpressionTerminator(TokenType type)
        => type is TokenType.Semicolon or TokenType.LeftBrace or TokenType.RightBrace or TokenType.EndOfFile;

    private void RequireSemicolon(string context)
    {
        if (!Match(TokenType.Semicolon))
        {
            var token = Peek();
            throw new ParseException($"Missing semicolon ';' after {context}.", token.Line, token.Column);
        }
    }

    private bool Match(TokenType type)
    {
        if (!Check(type))
        {
            return false;
        }

        Advance();
        return true;
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type))
        {
            return Advance();
        }

        var token = Peek();
        throw new ParseException(message, token.Line, token.Column);
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd())
        {
            return type == TokenType.EndOfFile;
        }

        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd())
        {
            _current++;
        }

        return Previous();
    }

    private bool IsAtEnd() => Peek().Type == TokenType.EndOfFile;
    private Token Peek() => _tokens[_current];
    private Token Previous() => _tokens[_current - 1];
}

internal static class TokenExtensions
{
    public static string LexemeOrType(this Token token)
        => string.IsNullOrEmpty(token.Lexeme) ? token.Type.ToString() : token.Lexeme;
}
