using static TokenType;

public class RuntimeError : Exception
{
    public readonly Token token;

    public RuntimeError(Token token, string message) : base(message)
    {
        this.token = token;
    }
}

public class ParseError : Exception { }

public static class LoxErrors
{
    public static bool ThrowParseError(Token token, string message)
    {
        Lox.error(token, message);
        throw new ParseError();
    }

    public static bool ThrowRuntimeError(Token Operator)
    {
        if (Operator.type == PLUS)
        {
            throw new RuntimeError(Operator, "Go to javaScript if you want to do this shit");
        }
        throw new RuntimeError(Operator, "I dont think you can divide objects..");
    }

    public static bool ThrowRuntimeError(Token Operator, string message)
    {
        throw new RuntimeError(Operator, message);
    }

}