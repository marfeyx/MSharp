using MSharp.Errors;
using MSharp.Lexing;

namespace MSharp.Runtime;

public sealed class RuntimeEnvironment
{
    private readonly Dictionary<string, MValue> _values = new(StringComparer.Ordinal);

    public void Set(string name, MValue value) => _values[name] = value;

    public MValue Get(string name, Token token)
    {
        if (_values.TryGetValue(name, out var value))
        {
            return value;
        }

        throw new RuntimeException($"Undefined variable '{name}'.", token.Line, token.Column);
    }
}
