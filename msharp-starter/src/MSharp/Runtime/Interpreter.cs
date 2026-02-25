using MSharp.Ast;
using MSharp.Errors;

namespace MSharp.Runtime;

public sealed class Interpreter
{
    private readonly Builtins _builtins;
    private readonly RuntimeEnvironment _environment;

    public Interpreter()
        : this(new Builtins(), new RuntimeEnvironment())
    {
    }

    public Interpreter(Builtins builtins, RuntimeEnvironment environment)
    {
        _builtins = builtins;
        _environment = environment;
    }

    public RuntimeEnvironment Environment => _environment;

    public void ExecuteProgram(IReadOnlyList<Stmt> statements)
    {
        foreach (var statement in statements)
        {
            ExecuteStatement(statement);
        }
    }

    private void ExecuteStatement(Stmt stmt)
    {
        switch (stmt)
        {
            case NumberAssignmentStmt numberAssign:
                _environment.Set(numberAssign.Name, MValue.FromNumber(numberAssign.Value.Value));
                break;

            case StringAssignmentStmt stringAssign:
                _environment.Set(stringAssign.Name, MValue.FromString(stringAssign.Value.Value));
                break;

            case SetFromCallStmt setStmt:
                var callResult = EvaluateExpression(setStmt.Call);
                _environment.Set(setStmt.Name, callResult);
                break;

            case SayStmt sayStmt:
                var value = EvaluateExpression(sayStmt.Value);
                Console.WriteLine(value.ToDisplayString());
                break;

            case CallStmt callStmt:
                _ = EvaluateExpression(callStmt.Call);
                break;

            case BlockStmt block:
                foreach (var inner in block.Statements)
                {
                    ExecuteStatement(inner);
                }
                break;

            case IfStmt ifStmt:
                if (EvaluateExpression(ifStmt.Condition).IsTruthy())
                {
                    ExecuteStatement(ifStmt.ThenBlock);
                }
                else if (ifStmt.ElseBlock is not null)
                {
                    ExecuteStatement(ifStmt.ElseBlock);
                }
                break;

            case WhileStmt whileStmt:
                while (EvaluateExpression(whileStmt.Condition).IsTruthy())
                {
                    ExecuteStatement(whileStmt.Body);
                }
                break;

            default:
                throw new RuntimeException("Unsupported statement.", stmt.StartToken.Line, stmt.StartToken.Column);
        }
    }

    private MValue EvaluateExpression(Expr expr)
    {
        return expr switch
        {
            NumberLiteralExpr number => MValue.FromNumber(number.Value),
            StringLiteralExpr str => MValue.FromString(str.Value),
            VariableExpr variable => _environment.Get(variable.Name, variable.StartToken),
            CallExpr call => EvaluateCall(call),
            _ => throw new RuntimeException("Unsupported expression.", expr.StartToken.Line, expr.StartToken.Column)
        };
    }

    private MValue EvaluateCall(CallExpr call)
    {
        var args = call.Arguments.Select(EvaluateExpression).ToList();
        return _builtins.Invoke(call.Name, args, call.StartToken);
    }
}
