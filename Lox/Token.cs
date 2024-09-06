using System.Runtime.ConstrainedExecution;

public class Token(TokenType type, string lexeme, object literal, int line)
{
    public readonly TokenType type = type;
    public readonly string lexeme = lexeme;
    public readonly object literal = literal;
    public readonly int line = line;

    public string toString()
    {
        return type + " " + lexeme + " " + literal;
    }
}