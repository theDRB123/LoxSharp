
public class Clock : ICallable
{
    public int Arity()
    {
        return 0;
    }

    public object Call(Interpreter interpreter, List<object> arguements)
    {
        return (double)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 1000.0);
    }

    public override string ToString()
    {
        return "<native fn>";
    }
}

public class LoxFunction : ICallable
{
    private readonly Stmt.Function declaration;
    private readonly Env closure;
    public LoxFunction(Stmt.Function declaration, Env closure)
    {
        this.declaration = declaration;
        this.closure = closure;
    }
    public int Arity()
    {
        return declaration.parameters.Count;
    }
    public object Call(Interpreter interpreter, List<object> arguements)
    {
        Env environment = new(closure);

        for (int i = 0; i < declaration.parameters.Count; i++)
        {
            environment.Define(declaration.parameters[i].lexeme, arguements[i]);
        }
        try
        {
            interpreter.executeBlock(declaration.body, environment);
        }
        catch(Return rtr)
        {
            return rtr.value;
        }
        return null;
    }
    public override string ToString()
    {
        return "<fn " + declaration.name.lexeme + ">";
    }
}