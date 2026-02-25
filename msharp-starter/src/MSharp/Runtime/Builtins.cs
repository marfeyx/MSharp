using System.Globalization;
using MSharp.Errors;
using MSharp.Lexing;

namespace MSharp.Runtime;

public delegate MValue BuiltinMethod(IReadOnlyList<MValue> args, Token callToken);

public sealed class Builtins
{
    private readonly Dictionary<string, BuiltinMethod> _methods = new(StringComparer.OrdinalIgnoreCase);

    public Builtins()
    {
        _methods["readNumber"] = ReadNumber;
        _methods["readString"] = ReadString;
        _methods["add"] = Add;
        _methods["sub"] = Sub;
        _methods["mul"] = Mul;
        _methods["div"] = Div;
        _methods["toString"] = ToStringMethod;
        _methods["concat"] = Concat;
        _methods["eq"] = Eq;
        _methods["neq"] = Neq;
    }

    public MValue Invoke(string name, IReadOnlyList<MValue> args, Token callToken)
    {
        if (_methods.TryGetValue(name, out var method))
        {
            return method(args, callToken);
        }

        throw new RuntimeException($"Unknown method '{name}'.", callToken.Line, callToken.Column);
    }

    private static MValue ReadNumber(IReadOnlyList<MValue> args, Token token)
    {
        RequireCount(args, 1, token, "readNumber");
        var prompt = args[0].AsString(token);

        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine() ?? string.Empty;
            if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            {
                return MValue.FromNumber(value);
            }

            Console.WriteLine("Invalid number. Try again.");
        }
    }

    private static MValue ReadString(IReadOnlyList<MValue> args, Token token)
    {
        RequireCount(args, 1, token, "readString");
        var prompt = args[0].AsString(token);
        Console.Write(prompt);
        var input = Console.ReadLine() ?? string.Empty;
        return MValue.FromString(input);
    }

    private static MValue Add(IReadOnlyList<MValue> args, Token token)
    {
        RequireCount(args, 2, token, "add");
        return MValue.FromNumber(args[0].AsNumber(token) + args[1].AsNumber(token));
    }

    private static MValue Sub(IReadOnlyList<MValue> args, Token token)
    {
        RequireCount(args, 2, token, "sub");
        return MValue.FromNumber(args[0].AsNumber(token) - args[1].AsNumber(token));
    }

    private static MValue Mul(IReadOnlyList<MValue> args, Token token)
    {
        RequireCount(args, 2, token, "mul");
        return MValue.FromNumber(args[0].AsNumber(token) * args[1].AsNumber(token));
    }

    private static MValue Div(IReadOnlyList<MValue> args, Token token)
    {
        RequireCount(args, 2, token, "div");
        var denominator = args[1].AsNumber(token);
        if (Math.Abs(denominator) <= double.Epsilon)
        {
            throw new RuntimeException("Division by zero.", token.Line, token.Column);
        }

        return MValue.FromNumber(args[0].AsNumber(token) / denominator);
    }

    private static MValue ToStringMethod(IReadOnlyList<MValue> args, Token token)
    {
        RequireCount(args, 1, token, "toString");
        return MValue.FromString(args[0].ToDisplayString());
    }

    private static MValue Concat(IReadOnlyList<MValue> args, Token token)
    {
        RequireCount(args, 2, token, "concat");
        return MValue.FromString(args[0].ToDisplayString() + args[1].ToDisplayString());
    }

    private static MValue Eq(IReadOnlyList<MValue> args, Token token)
    {
        RequireCount(args, 2, token, "eq");
        var equal = AreEqual(args[0], args[1]);
        return MValue.FromNumber(equal ? 1 : 0);
    }

    private static MValue Neq(IReadOnlyList<MValue> args, Token token)
    {
        RequireCount(args, 2, token, "neq");
        var equal = AreEqual(args[0], args[1]);
        return MValue.FromNumber(equal ? 0 : 1);
    }

    private static bool AreEqual(MValue left, MValue right)
    {
        if (left.Kind != right.Kind)
        {
            return false;
        }

        return left.Kind switch
        {
            MValueKind.Number => Math.Abs(left.NumberValue - right.NumberValue) <= double.Epsilon,
            MValueKind.String => string.Equals(left.StringValue, right.StringValue, StringComparison.Ordinal),
            _ => false
        };
    }

    private static void RequireCount(IReadOnlyList<MValue> args, int count, Token token, string methodName)
    {
        if (args.Count != count)
        {
            throw new RuntimeException($"Method '{methodName}' expects {count} argument(s), got {args.Count}.", token.Line, token.Column);
        }
    }
}
