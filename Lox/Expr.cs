using System.Collections.Generic;

public abstract class Expr
{
      public interface Visitor<T>
      {
            T VisitBinaryExpr(Binary expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitUnaryExpr(Unary expr);
            T VisitConditionalExpr(Conditional conditional);
      }
      public class Binary : Expr
      {

            public readonly Expr Left;
            public readonly Token Operator;
            public readonly Expr Right;
            public Binary(Expr Left, Token Operator, Expr Right)
            {
                  this.Left = Left;
                  this.Operator = Operator;
                  this.Right = Right;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitBinaryExpr(this);
            }
      }
      public class Grouping : Expr
      {

            public readonly Expr Expression;
            public Grouping(Expr Expression)
            {
                  this.Expression = Expression;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitGroupingExpr(this);
            }
      }
      public class Literal : Expr
      {

            public readonly Object Value;
            public Literal(Object Value)
            {
                  this.Value = Value;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitLiteralExpr(this);
            }
      }
      public class Unary : Expr
      {

            public readonly Token Operator;
            public readonly Expr right;
            public Unary(Token Operator, Expr right)
            {
                  this.Operator = Operator;
                  this.right = right;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitUnaryExpr(this);
            }
      }

      public class Conditional : Expr
      {
            public readonly Expr expr;
            public readonly Expr thenBranch;
            public readonly Expr elseBranch;

            public Conditional(Expr expr, Expr thenBranch, Expr elseBranch)
            {
                  this.expr = expr;
                  this.thenBranch = thenBranch;
                  this.elseBranch = elseBranch;
            }

            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitConditionalExpr(this);
            }

      }

      public abstract T Accept<T>(Visitor<T> visitor);
}
