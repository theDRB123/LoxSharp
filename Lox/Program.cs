using System.Runtime.InteropServices;

public class Lox
{
    private static bool hadError;

    public static void Main(string[] args)
    {
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
        while (true)
        {
            Console.Write("--> ");
            string? line = Console.ReadLine();
            if (line == null) break;
            run(line);
            hadError = false;
        }
    }

    private static void runFile(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        run(bytes.ToString());
    }

    private static void run(string? source)
    {
        Scanner scanner = new(source);

        if (hadError) Environment.Exit(65);
        List<Token> tokens = scanner.scanTokens();
        printTokens(tokens);

        Parser parser = new(tokens);
        Expr expression = parser.parse();

        if (hadError) return;
        Console.WriteLine(new ASTgen().Generate(expression));
    }

    private static void printTokens(List<Token> tokens)
    {
        tokens.ForEach(token => Console.WriteLine(token.toString()));
    }

    public static void error(int line, string message)
    {
        report(line, "", message);
    }

    public static void error(Token token, string message){
        if (token.type == TokenType.EOF){
            report(token.line, " at end", message);
        } else {
            report(token.line, " at '" + token.lexeme + "'", message);
        }
    }

    private static void report(int line, string where, string message)
    {
        Console.Error.WriteLine("[line => " + line + "] | Error => " + message);
        hadError = true;

    }
}



