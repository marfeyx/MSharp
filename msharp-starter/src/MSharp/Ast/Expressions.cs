using MSharp.Lexing;

namespace MSharp.Ast;

public abstract record Expr(Token StartToken);

public sealed record NumberLiteralExpr(Token StartToken, double Value) : Expr(StartToken);
public sealed record StringLiteralExpr(Token StartToken, string Value) : Expr(StartToken);
public sealed record VariableExpr(Token StartToken, string Name) : Expr(StartToken);
public sealed record CallExpr(Token StartToken, string Name, IReadOnlyList<Expr> Arguments) : Expr(StartToken);
