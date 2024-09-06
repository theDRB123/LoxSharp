using System.Runtime.ConstrainedExecution;

public class Token
{
    readonly TokenType type;
    readonly String lexeme;
    readonly Object literal;
    readonly int line;

    public Token(TokenType type, string lexeme, object literal, int line)
    {
        this.type = type;
        this.lexeme = lexeme;
        this.literal = literal;
        this.line = line;
    }

    public string toString()
    {
        return type + " " + lexeme + " " + literal;
    }
}