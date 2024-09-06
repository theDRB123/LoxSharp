using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using static TokenType;
public class Parser
{
    public class ParseError : Exception { }
    private readonly List<Token> tokens;
    private int current = 0;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public Expr parse(){
        try {
            return expression();
        } catch (ParseError error){
            return null;
        }
    }

    private Expr expression()
    {
        return equality();
    }

    private Expr equality()
    {
        // equality -> comparision (( "!=" | "==") comparision)*
        Expr expr = comparision();

        while (match(BANG_EQUAL, EQUAL_EQUAL))
        {
            Token Operator = previous();
            Expr Right = comparision();
            expr = new Expr.Binary(expr, Operator, Right);
        }
        return expr;
    }

    private Expr comparision()
    {
        // comparision  ->  term (( ">" | ">= | "<" | "<=" ) term) * ;
        Expr expr = term();

        while (match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
        {
            Token Operator = previous();
            Expr Right = term();
            expr = new Expr.Binary(expr, Operator, Right);
        }
        return expr;
    }

    private Expr term()
    {
        // term -> factor (("+" | "-") factor )*
        Expr expr = factor();
        while (match(PLUS, MINUS))
        {
            Token Operator = previous();
            Expr Right = factor();
            expr = new Expr.Binary(expr, Operator, Right);
        }
        return expr;
    }

    private Expr factor()
    {
        // factor -> unary (("/" | "*") unary)*
        Expr expr = unary();
        while (match(SLASH, STAR))
        {
            Token Operator = previous();
            Expr Right = unary();
            expr = new Expr.Binary(expr, Operator, Right);
        }
        return expr;
    }

    private Expr unary()
    {
        if (match(BANG, MINUS))
        {
            Token Operator = previous();
            Expr Right = unary();
            return new Expr.Unary(Operator, Right);
        }
        return primary();
    }

    private Expr primary()
    {
        if (match(LEFT_PAREN))
        {
            Expr expr = expression();
            consume(RIGHT_PAREN, "Expected ')' after expressions");
            return new Expr.Grouping(expr);
        }
        return new Expr.Literal(advance().type switch
        {
            NUMBER => previous().literal,
            STRING => previous().literal,
            FALSE => false,
            TRUE => true,
            NIL => null,
            _ => throw error(peek(), "Expect expression")
        });
    }



    private Token peek() => tokens[current];
    private Token previous() => tokens[current - 1];
    private bool check(TokenType type) => isAtEnd() ? false : peek().type == type;
    private bool isAtEnd() => peek().type == EOF;
    private Token advance()
    {
        if (!isAtEnd())
        {
            current++;
        }
        return previous();
    }
    private bool match(params TokenType[] tokens)
    {
        foreach (var token in tokens)
        {
            if (check(token))
            {
                advance();
                return true;
            }
        }
        return false;
    }


    private Token consume(TokenType type, string message)
    {
        if (check(type))
        {
            return advance();
        }
        throw error(peek(), message);
    }

    private ParseError error(Token token, string message)
    {
        Lox.error(token, message);
        return new ParseError();
    }

    private void synchronize()
    {
        advance();

        while (!isAtEnd())
        {
            if (previous().type == SEMICOLON) return;

            switch (peek().type)
            {
                case CLASS:
                    break;
                case FUN:
                    break;
                case VAR:
                    break;
                case FOR:
                    break;
                case IF:
                    break;
                case WHILE:
                    break;
                case PRINT:
                    break;
                case RETURN:
                    return;
            }

            advance();
        }
    }
}
