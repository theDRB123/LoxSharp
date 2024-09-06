using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using static TokenType;

public class Parser
{

    private readonly List<Token> tokens;
    private int current = 0;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public List<Stmt> parse()
    {
        List<Stmt> statements = [];
        while (!isAtEnd())
        {
            statements.Add(declaration());
        }
        return statements;
    }

    // statement -> printStatement | expressionStatement;
    // printStatement -> "print" expression ";"
    // expressionStatement -> expression ";"
    // expression -> assignment;
    // assignment -> "IDENTIFIER" + assignment | conditional
    // conditional -> equality ( "?" expression ":" conditional )? ;

    private Stmt declaration()
    {
        try
        {
            if (match(VAR))
            {
                return varDeclaration();
            }
            return statement();
        }
        catch (ParseError err)
        {
            synchronize();
            return null;
        }
    }

    private Stmt varDeclaration()
    {
        Token name = consume(INDENTIFIER, "A Variable has no name !! or does it huh ?");
        Expr initializer = null;
        if (match(EQUAL))
        {
            initializer = expression();
        }
        consume(SEMICOLON, "King you dropped a ';'");
        return new Stmt.Var(name, initializer);
    }


    private Stmt statement()
    {
        if (match(PRINT))
        {
            return printStatement();
        }
        if (match(LEFT_BRACE))
        {
            return new Stmt.Block(block());
        }
        if (match(IF))
        {
            return ifStatement();
        }
        if (match(WHILE))
        {
            return whileStatement();
        }
        if (match(FOR)){
            return forStatement();
        }
        return expressionStatement();
    }


    private List<Stmt> block()
    {
        List<Stmt> statements = [];

        while (!check(RIGHT_BRACE) && !isAtEnd())
        {
            statements.Add(declaration());
        }

        consume(RIGHT_BRACE, "HEY HEY HEY !! You forgot the '}'");
        return statements;
    }

    private Stmt expressionStatement()
    {
        Expr value = expression();
        consume(SEMICOLON, "King you dropped a ';");
        return new Stmt.Expression(value);
    }


    private Stmt printStatement()
    {
        Expr value = expression();
        consume(SEMICOLON, "king you dropped a ';'");
        return new Stmt.Print(value);
    }

    private Stmt ifStatement()
    {
        consume(LEFT_PAREN, "You gotta put the '(' in the if statement bud");
        Expr condition = expression();
        consume(RIGHT_PAREN, "You gotta end the if with a ')' bud");

        Stmt thenBranch = statement();
        Stmt elseBranch = null;

        if (match(ELSE))
        {
            elseBranch = statement();
        }

        return new Stmt.If(condition, thenBranch, elseBranch);
    }
    private Stmt whileStatement()
    {
        consume(LEFT_PAREN, "Oh shit, here we go again!! You forgot the '(' in the while idiot");
        Expr condition = expression();
        consume(RIGHT_PAREN, "You should rot in hell, you forgot the ')' here !!");

        Stmt body = statement();
        return new Stmt.While(condition, body);
    }

    private Stmt forStatement(){
        consume(LEFT_PAREN, "Uhm, I think you need to have a '(' in For loop");

        Stmt inititializer;
        if(match(SEMICOLON)){
            inititializer = null;
        } else if(match(VAR)){
            inititializer = varDeclaration();
        } else {
            inititializer = expressionStatement();
        }

        Expr condition = null;
        if(!check(SEMICOLON)){
            condition = expression();
        }
        consume(SEMICOLON, "HEY YOU!! Put a ';' after the loop condition");

        Expr increment = null;
        if(!check(RIGHT_PAREN)){
            increment = expression();
        }
        consume(RIGHT_PAREN, "Close the for loop with ')' please");

        Stmt body = statement();
        if(increment != null){
            body = new Stmt.Block([body, new Stmt.Expression(increment)]);
        }
        if(condition != null){
            body = new Stmt.While(condition, body);
        }
        if(inititializer != null){
            body = new Stmt.Block([inititializer, body]);
        }
        return body;
    }

    //expression methods
    private Expr expression()
    {
        return assignment();
    }

    private Expr assignment()
    {
        Expr expr = or();

        if (match(EQUAL))
        {
            Token equals = previous();
            Expr value = assignment();

            if (expr is Expr.Variable)
            {
                Token name = ((Expr.Variable)expr).name;
                return new Expr.Assign(name, value);
            }

            LoxErrors.ThrowParseError(equals, "I think a sane person assigns something to a variable");
        }

        return expr;
    }

    private Expr or()
    {
        Expr expr = and();

        while (match(OR))
        {
            Token Operator = previous();
            Expr right = and();
            expr = new Expr.Logical(expr, Operator, right);
        }

        return expr;
    }

    private Expr and()
    {
        Expr expr = conditional();

        while (match(AND))
        {
            Token Operator = previous();
            Expr right = conditional();
            expr = new Expr.Logical(expr, Operator, right);
        }

        return expr;
    }

    private Expr conditional()
    {
        Expr expr = equality();

        if (match(QUESTION))
        {
            Expr thenBranch = expression();
            consume(COLON, "A ternary operator needs a ':' i think");
            Expr elseBranch = conditional();
            expr = new Expr.Conditional(expr, thenBranch, elseBranch);
        }
        return expr;
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
        if (match(INDENTIFIER))
        {
            return new Expr.Variable(previous());
        }
        if (match(LEFT_PAREN))
        {
            Expr expr = expression();
            consume(RIGHT_PAREN, "Complete the Parenthesis you dummy");
            return new Expr.Grouping(expr);
        }
        //error productions 
        if (match(BANG_EQUAL, EQUAL_EQUAL))
        {
            LoxErrors.ThrowParseError(previous(), "You are missing a left arm, Oh sorry mean an opeand ");
            equality();
            return null;
        }
        if (match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
        {
            LoxErrors.ThrowParseError(previous(), "Dude, I think you probably need a left hand operand here");
            comparision();
            return null;
        }
        if (match(PLUS))
        {
            LoxErrors.ThrowParseError(previous(), "Dude, I think you probably need a left hand operand here");
            comparision();
            return null;
        }
        if (match(SLASH, STAR))
        {
            LoxErrors.ThrowParseError(previous(), "Dude, I think you probably need a right hand operand here");
            factor();
            return null;
        }

        return new Expr.Literal(advance().type switch
        {
            NUMBER => (double)previous().literal,
            STRING => previous().literal,
            FALSE => false,
            TRUE => true,
            NIL => null,
            _ => LoxErrors.ThrowParseError(peek(), "I WANT AN EXPRESSION HERE NOW !!!")
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
        LoxErrors.ThrowParseError(peek(), message);
        return null;
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
