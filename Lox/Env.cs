using System.Net;
using static LoxErrors;
public class Env
{
    private readonly Dictionary<string, object> values = [];
    public readonly Env enclosing;

    public Env(){
        enclosing = null;
    }

    public Env(Env env){
        enclosing = env;
    }

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

        return enclosing != null ? enclosing.Get(name) : ThrowRuntimeError(name, "Wtf is " + name.lexeme + " huh? TELL ME !!");
    }

    public object GetAt(int distance, string name){
        return Ancestor(distance).values[name]; 
    }
    public object Assign(Token name, object value)
    {
        if (values.TryGetValue(name.lexeme, out _)){
            values[name.lexeme] = value;
            return value;
        }

        return enclosing != null ? enclosing.Assign(name,value) : ThrowRuntimeError(name, "You tried to assign to a non-existing variable lmao");
    }

    public void AssignAt(int distance, Token name, object value){
        Ancestor(distance).values[name.lexeme] = value;     
    }

    public Env Ancestor(int distance){
        Env env = this;
        for(int i = 0; i < distance; i++){
            env = env.enclosing;
        }
        return env;
    }
}