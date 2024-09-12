using System.Collections.Generic;

public abstract class Expr
{
      public interface Visitor<T>
      {
            T VisitAssignExpr(Assign expr);
            T VisitBinaryExpr(Binary expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitUnaryExpr(Unary expr);
            T VisitLogicalExpr(Logical expr);
            T VisitConditionalExpr(Conditional expr);
            T VisitVariableExpr(Variable expr);
            T VisitCallExpr(Call expr);
      }
      public class Assign : Expr
      {

            public readonly Token name;
            public readonly Expr value;
            public Assign(Token name, Expr value)
            {
                  this.name = name;
                  this.value = value;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitAssignExpr(this);
            }
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
      public class Logical : Expr
      {

            public readonly Expr left;
            public readonly Token Operator;
            public readonly Expr right;
            public Logical(Expr left, Token Operator, Expr right)
            {
                  this.left = left;
                  this.Operator = Operator;
                  this.right = right;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitLogicalExpr(this);
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
      public class Variable : Expr
      {

            public readonly Token name;
            public Variable(Token name)
            {
                  this.name = name;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitVariableExpr(this);
            }
      }
      public class Call : Expr
      {

            public readonly Expr callee;
            public readonly Token paren;
            public readonly List<Expr> arguements;
            public Call(Expr callee, Token paren, List<Expr> arguements)
            {
                  this.callee = callee;
                  this.paren = paren;
                  this.arguements = arguements;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitCallExpr(this);
            }
      }

      public abstract T Accept<T>(Visitor<T> visitor);
}
