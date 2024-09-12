using System.Runtime.InteropServices;

public class Lox
{
    private static bool hadError;
    private static bool hadRuntimeError;
    private static readonly Interpreter interpreter = new();
    public static void Main(string[] args)
    {
        Console.Clear();
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: clox [script]");
            Environment.Exit(0);
        }
        else if (args.Length == 1)
        {
            runFile(args[0]);
        }
        else
        {
            runPrompt();
        }
        // ASTgen.test(null);
    }

    private static void runPrompt()
    {
        interpreter.isREPL = true;
        while (true)
        {
            Console.Write("--> ");
            string line = Console.ReadLine();
            if (line == "")
            {
                Console.WriteLine("Empty line");
                continue;
            }
            run(line);
            hadError = false;
        }
    }

    private static void runFile(string path)
    {
        string content = File.ReadAllText(path);
        // Console.WriteLine(content);
        run(content);
        if (hadError) Environment.Exit(65);
        if (hadRuntimeError) Environment.Exit(70);
    }

    private static void run(string source)
    {
        Scanner scanner = new(source);

        if (hadError) Environment.Exit(65);
        List<Token> tokens = scanner.scanTokens();
        // printTokens(tokens);

        Parser parser = new(tokens);
        List<Stmt> statements = parser.parse();

        if (hadError) return;
        Resolver resolver = new(interpreter);
        resolver.Resolve(statements);
        if (hadError) return;
        interpreter.Interpret(statements);
        // Console.WriteLine(new ASTgen().Generate(expression));
    }

    private static void printTokens(List<Token> tokens)
    {
        tokens.ForEach(token => Console.WriteLine(token.toString()));
    }

    public static void error(int line, string message)
    {
        report(line, "", message);
    }

    public static void error(Token token, string message)
    {
        if (token.type == TokenType.EOF)
        {
            report(token.line, " at end", message);
        }
        else
        {
            report(token.line, " at '" + token.lexeme + "'", message);
        }
    }

    private static void report(int line, string where, string message)
    {
        Console.Error.WriteLine("[line => " + line + "] | Error => " + message);
        hadError = true;

    }

    public static void runtimeError(RuntimeError error)
    {
        Console.WriteLine($"[Line {error.token.line} ]" + "| Runtime error | " + error.Message);
        hadRuntimeError = true;
    }

}



