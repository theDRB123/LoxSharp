using System.Text.RegularExpressions;

public class RPNgen : Expr.Visitor<string>
{
    public string VisitAssignExpr(Expr.Assign expr) => "( assign " + expr.name + " " + helper(expr.value) + ")";
    public string VisitBinaryExpr(Expr.Binary expr) => helper(expr.Left, expr.Right) + " " + expr.Operator.lexeme;
    public string VisitGroupingExpr(Expr.Grouping expr) => helper(expr.Expression);
    public string VisitLiteralExpr(Expr.Literal expr) => expr.Value.ToString();
    public string VisitUnaryExpr(Expr.Unary expr) => helper(expr.right) + expr.Operator.lexeme;
    public string VisitConditionalExpr(Expr.Conditional expr) => "if" + helper(expr.expr) + " then " + helper(expr.thenBranch) + " else " + helper(expr.elseBranch);
    public string Generate(Expr expr) => expr.Accept<string>(this);

    public string helper(params Expr[] exprs)
    => string.Join(" ",exprs.Select((expr) => expr.Accept<string>(this)));

    public string VisitVariableExpr(Expr.Variable expr)
    {
        throw new NotImplementedException();
    }

    public string VisitLogicalExpr(Expr.Logical expr)
    {
        throw new NotImplementedException();
    }

    public string VisitCallExpr(Expr.Call expr)
    {
        throw new NotImplementedException();
    }
}