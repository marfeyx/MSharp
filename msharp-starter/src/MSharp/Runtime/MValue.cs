using System.Globalization;
using MSharp.Errors;
using MSharp.Lexing;

namespace MSharp.Runtime;

public enum MValueKind
{
    Number,
    String
}

public readonly record struct MValue(MValueKind Kind, double NumberValue, string StringValue)
{
    public static MValue FromNumber(double value) => new(MValueKind.Number, value, string.Empty);
    public static MValue FromString(string value) => new(MValueKind.String, 0, value);

    public double AsNumber(Token token)
    {
        if (Kind != MValueKind.Number)
        {
            throw new RuntimeException("Expected a number.", token.Line, token.Column);
        }

        return NumberValue;
    }

    public string AsString(Token token)
    {
        if (Kind != MValueKind.String)
        {
            throw new RuntimeException("Expected a string.", token.Line, token.Column);
        }

        return StringValue;
    }

    public bool IsTruthy() => Kind switch
    {
        MValueKind.Number => Math.Abs(NumberValue) > double.Epsilon,
        MValueKind.String => !string.IsNullOrEmpty(StringValue),
        _ => false
    };

    public string ToDisplayString() => Kind switch
    {
        MValueKind.Number => NumberValue.ToString("0.################", CultureInfo.InvariantCulture),
        MValueKind.String => StringValue,
        _ => string.Empty
    };
}
