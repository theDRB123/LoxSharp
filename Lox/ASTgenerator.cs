public class ASTgen : Expr.Visitor<string>
{
    public string Generate(Expr expr) => expr.Accept(this);

    //visitor methods for all
    public string VisitAssignExpr(Expr.Assign expr) => Parenthesize("assign " + expr.name.ToString(), expr.value);
    public string VisitBinaryExpr(Expr.Binary expr) => Parenthesize(expr.Operator.lexeme, expr.Left, expr.Right);
    public string VisitGroupingExpr(Expr.Grouping expr) => Parenthesize("group", expr.Expression);
    public string VisitLiteralExpr(Expr.Literal expr) => expr.Value == null ? "nil" : expr.Value.ToString();
    public string VisitUnaryExpr(Expr.Unary expr) => Parenthesize(expr.Operator.lexeme, expr.right);
    public string VisitConditionalExpr(Expr.Conditional expr) => Parenthesize("if", expr.expr, expr.thenBranch, expr.elseBranch);

    public string Parenthesize(string name, params Expr[] exprs)
    => "(" + name + string.Join("", exprs.Select((expr) => " " + expr.Accept(this))) + ")";

    public static void test(string[] args)
    {
        Expr expression = new Expr.Binary(
            new Expr.Grouping(
                new Expr.Binary(
                    new Expr.Literal(1),
                    new Token(TokenType.PLUS, "+", null, 1),
                    new Expr.Literal(2)
                )
            ),
            new Token(TokenType.STAR, "*", null, 1),
            new Expr.Grouping(
                new Expr.Binary(
                    new Expr.Literal(4),
                    new Token(TokenType.MINUS, "-", null, 1),
                    new Expr.Literal(3)
                )
            )
        );

        Console.WriteLine(new ASTgen().Generate(expression));
        Console.WriteLine(new RPNgen().Generate(expression));
    }

    public string VisitVariableExpr(Expr.Variable expr)
    {
        throw new NotImplementedException();
    }

    public string VisitLogicalExpr(Expr.Logical expr)
    {
        throw new NotImplementedException();
    }
}