public interface ICallable
{
    int Arity();
    object Call(Interpreter interpreter, List<Object> arguements);
}