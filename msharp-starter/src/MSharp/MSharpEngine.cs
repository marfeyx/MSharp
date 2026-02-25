using MSharp.Errors;
using MSharp.Lexing;
using MSharp.Parsing;
using MSharp.Runtime;

namespace MSharp;

public sealed class MSharpEngine
{
    private readonly Interpreter _interpreter;

    public MSharpEngine()
    {
        _interpreter = new Interpreter();
    }

    public void ExecuteSource(string source)
    {
        var lexer = new Lexer(source);
        var tokens = lexer.Tokenize();
        var parser = new Parser(tokens);
        var program = parser.ParseProgram();
        _interpreter.ExecuteProgram(program);
    }

    public void ExecuteFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File not found: {path}");
        }

        var source = File.ReadAllText(path);
        ExecuteSource(source);
    }

    public static void PrintError(Exception ex)
    {
        if (ex is MSharpException msharpEx)
        {
            Console.Error.WriteLine($"M# error: {msharpEx.ToDisplayString()}");
            return;
        }

        Console.Error.WriteLine($"Error: {ex.Message}");
    }
}
