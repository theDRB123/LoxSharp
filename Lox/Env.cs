using static LoxErrors;
public class Env
{
    private readonly Dictionary<string, object> values = [];

    public void Define(string name, object value)
    {
        values[name] = value;
    }

    public object Get(Token name)
    {
        if (values.TryGetValue(name.lexeme, out object value))
        {
            return value;
        }
        return ThrowRuntimeError(name, "Wtf is " + name.lexeme + " huh? TELL ME !!");
    }

    public object Assign(Token name, object value)
    {
        if (values.TryGetValue(name.lexeme, out _)){
            values[name.lexeme] = value;
            return value;
        }
        return ThrowRuntimeError(name, "You tried to assign to a non-existing variable lmao");
    }
}