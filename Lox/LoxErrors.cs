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
        Lox.error(token, "ParseError | " + message);
        throw new ParseError();
    }

    public static bool ThrowRuntimeError(Token Operator)
    {
        if (Operator.type == PLUS)
        {
            throw new RuntimeError(Operator, "RuntimeError | " + "Go to javaScript if you want to do this shit");
        }
        throw new RuntimeError(Operator, "RuntimeError | " + "dont think you can divide objects..");
    }

    public static bool ThrowRuntimeError(Token Operator, string message)
    {
        throw new RuntimeError(Operator, "RuntimeError | " + message);
    }

}

public class Break : Exception { }

public class Return : Exception
{
    public readonly object value;

    public Return(object value) : base()
    {
        this.value = value;
    }
}

public class LoxControlFlow
{
    public static bool Break()
    {
        throw new Break();
    }

    public static void Return(object value)
    {
        throw new Return(value);
    }

}