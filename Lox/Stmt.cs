﻿using System.Collections.Generic;

public abstract class Stmt
{
      public interface Visitor<T>
      {
            T VisitBlockStmt(Block stmt);
            T VisitExpressionStmt(Expression stmt);
            T VisitFunctionStmt(Function stmt);
            T VisitPrintStmt(Print stmt);
            T VisitVarStmt(Var stmt);
            T VisitIfStmt(If stmt);
            T VisitWhileStmt(While stmt);
            T VisitBreakStmt(Break stmt);
            T VisitReturnStmt(Return stmt);
      }
      
      public class Block : Stmt
      {

            public readonly List<Stmt> statements;
            public Block(List<Stmt> statements)
            {
                  this.statements = statements;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitBlockStmt(this);
            }
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
      public class Function : Stmt
      {

            public readonly Token name;
            public readonly List<Token> parameters;
            public readonly List<Stmt> body;
            public Function(Token name, List<Token> parameters, List<Stmt> body)
            {
                  this.name = name;
                  this.parameters = parameters;
                  this.body = body;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitFunctionStmt(this);
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
      public class If : Stmt
      {

            public readonly Expr condition;
            public readonly Stmt thenBranch;
            public readonly Stmt elseBranch;
            public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
            {
                  this.condition = condition;
                  this.thenBranch = thenBranch;
                  this.elseBranch = elseBranch;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitIfStmt(this);
            }
      }
      public class While : Stmt
      {

            public readonly Expr condition;
            public readonly Stmt body;
            public While(Expr condition, Stmt body)
            {
                  this.condition = condition;
                  this.body = body;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitWhileStmt(this);
            }
      }
      public class Break : Stmt
      {

            public readonly Token type;
            public Break(Token type)
            {
                  this.type = type;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitBreakStmt(this);
            }
      }
      public class Return : Stmt
      {

            public readonly Token keyword;
            public readonly Expr value;
            public Return(Token keyword, Expr value)
            {
                  this.keyword = keyword;
                  this.value = value;
            }
            public override T Accept<T>(Visitor<T> visitor)
            {
                  return visitor.VisitReturnStmt(this);
            }
      }

      public abstract T Accept<T>(Visitor<T> visitor);
}
