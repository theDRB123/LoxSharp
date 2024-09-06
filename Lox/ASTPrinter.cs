public class ASTPrinter : Expr.Visitor<string>
{
    string Print(Expr expr)
    {
        return expr.Accept(this);
    }


    //visitor methods for all
    public string VisitBinaryExpr(Expr.Binary expr)
    {
        return Parenthesize(expr.Operator.lexeme, expr.Left, expr.Right);
    }

    public string VisitGroupingExpr(Expr.Grouping expr)
    {
        return Parenthesize("group", expr.Expression);
    }

    public string VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.Value == null ? "nil" : expr.Value.ToString();
    }

    public string VisitUnaryExpr(Expr.Unary expr)
    {
        return Parenthesize(expr.Operator.lexeme, expr.right);
    }


    public string Parenthesize(string name, params Expr[] exprs)
    {
        string s = "";
        s += "(" + name;
        foreach (Expr expr in exprs)
        {
            s += " " + expr.Accept(this);
        }
        s += ")";
        return s;
    }


    public static void run(string[] args)
    {
        Expr expression = new Expr.Binary(
            new Expr.Unary(new Token(TokenType.MINUS, "-", null, 1), new Expr.Literal(123)),
            new Token(TokenType.STAR, "*", null, 1),
            new Expr.Grouping(new Expr.Literal(56.334))
        );

        Console.WriteLine(new ASTPrinter().Print(expression));
    }
}