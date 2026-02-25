using System.Text;

namespace MSharp;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintHelp();
            return 1;
        }

        try
        {
            var command = args[0].ToLowerInvariant();
            return command switch
            {
                "run" => RunFile(args),
                "repl" => RunRepl(),
                "help" or "--help" or "-h" => Help(),
                _ => UnknownCommand(args[0])
            };
        }
        catch (Exception ex)
        {
            MSharpEngine.PrintError(ex);
            return 1;
        }
    }

    private static int RunFile(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Missing file path. Usage: msharp run path/to/file.ms");
            return 1;
        }

        var engine = new MSharpEngine();
        engine.ExecuteFile(args[1]);
        return 0;
    }

    private static int RunRepl()
    {
        var engine = new MSharpEngine();
        Console.WriteLine("M# REPL");
        Console.WriteLine("Type :quit to exit.");

        var buffer = new StringBuilder();
        var braceDepth = 0;

        while (true)
        {
            Console.Write(buffer.Length == 0 ? "msharp> " : "...    ");
            var line = Console.ReadLine();
            if (line is null)
            {
                Console.WriteLine();
                break;
            }

            if (buffer.Length == 0 && (line.Trim().Equals(":quit", StringComparison.OrdinalIgnoreCase) || line.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase)))
            {
                break;
            }

            buffer.AppendLine(line);
            braceDepth += CountChar(line, '{');
            braceDepth -= CountChar(line, '}');

            if (braceDepth < 0)
            {
                Console.WriteLine("M# error: Too many closing braces.");
                buffer.Clear();
                braceDepth = 0;
                continue;
            }

            var trimmed = line.TrimEnd();
            var looksComplete = braceDepth == 0 && (trimmed.EndsWith(';') || trimmed.EndsWith('}'));
            if (!looksComplete)
            {
                continue;
            }

            try
            {
                engine.ExecuteSource(buffer.ToString());
            }
            catch (Exception ex)
            {
                MSharpEngine.PrintError(ex);
            }
            finally
            {
                buffer.Clear();
                braceDepth = 0;
            }
        }

        return 0;
    }

    private static int CountChar(string text, char c)
    {
        var count = 0;
        foreach (var ch in text)
        {
            if (ch == c)
            {
                count++;
            }
        }

        return count;
    }

    private static int Help()
    {
        PrintHelp();
        return 0;
    }

    private static int UnknownCommand(string command)
    {
        Console.Error.WriteLine($"Unknown command '{command}'.");
        PrintHelp();
        return 1;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("M# (MSharp) CLI");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  msharp run <file.ms>   Run an M# source file");
        Console.WriteLine("  msharp repl            Start interactive REPL");
        Console.WriteLine();
        Console.WriteLine("Using dotnet during development:");
        Console.WriteLine("  dotnet run --project src/MSharp -- run examples/hello.ms");
    }
}
