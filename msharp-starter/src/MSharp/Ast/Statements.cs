using MSharp.Lexing;

namespace MSharp.Ast;

public abstract record Stmt(Token StartToken);

public sealed record NumberAssignmentStmt(Token StartToken, string Name, NumberLiteralExpr Value) : Stmt(StartToken);
public sealed record StringAssignmentStmt(Token StartToken, string Name, StringLiteralExpr Value) : Stmt(StartToken);
public sealed record SetFromCallStmt(Token StartToken, string Name, CallExpr Call) : Stmt(StartToken);
public sealed record SayStmt(Token StartToken, Expr Value) : Stmt(StartToken);
public sealed record CallStmt(Token StartToken, CallExpr Call) : Stmt(StartToken);
public sealed record BlockStmt(Token StartToken, IReadOnlyList<Stmt> Statements) : Stmt(StartToken);
public sealed record IfStmt(Token StartToken, Expr Condition, BlockStmt ThenBlock, BlockStmt? ElseBlock) : Stmt(StartToken);
public sealed record WhileStmt(Token StartToken, Expr Condition, BlockStmt Body) : Stmt(StartToken);
