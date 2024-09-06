using System.Collections.Generic;

public abstract class Stmt
{
      public interface Visitor<T>
      {
            T VisitExpressionStmt(Expression stmt);
            T VisitPrintStmt(Print stmt);
            T VisitVarStmt(Var stmt);
      }
      public class Expression : Stmt
      {

            public readonly Expr expression;
            public Expression(Expr expression)
            {
                  this.expression = expression;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitExpressionStmt(this);
            }
      }
      public class Print : Stmt
      {

            public readonly Expr expression;
            public Print(Expr expression)
            {
                  this.expression = expression;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitPrintStmt(this);
            }
      }
      public class Var : Stmt
      {

            public readonly Token name;
            public readonly Expr initializer;
            public Var(Token name, Expr initializer)
            {
                  this.name = name;
                  this.initializer = initializer;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitVarStmt(this);
            }
      }

      public abstract T Accept<T>(Visitor<T> visitor);
}
