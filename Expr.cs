using System.Collections.Generic;

public abstract class Expr
{
    class Binary : Expr
    {
        readonly Expr Left;
        readonly Token Operator;
        readonly Expr Right;
        Binary(Expr Left, Token Operator, Expr Right)
        {
            this.Left = Left;
            this.Operator = Operator;
            this.Right = Right;
        }
    }
    class Grouping : Expr
    {
        readonly Expr Expression;
        Grouping(Expr Expression)
        {
            this.Expression = Expression;
        }
    }
    class Literal : Expr
    {
        readonly Object Value;
        Literal(Object Value)
        {
            this.Value = Value;
        }
    }
    class Unary : Expr
    {
        readonly Token Operator;
        readonly Expr right;
        Unary(Token Operator, Expr right)
        {
            this.Operator = Operator;
            this.right = right;
        }
    }
}
