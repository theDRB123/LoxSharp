using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: generateAst <output_dir>");
            Environment.Exit(64);
        }
        string output_dir = args[0];
        defineAST(output_dir, "Expr", [
            "Binary   : Expr Left,Token Operator,Expr Right",
            "Grouping : Expr Expression",
            "Literal  : Object Value",
            "Unary    : Token Operator, Expr right"
        ]);
    }

    private static void defineAST(string output_dir, string baseName, List<string> types)
    {
        string path = "../../" + baseName + ".cs";
        using (StreamWriter writer = new(path, false, Encoding.UTF8))
        {
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine();
            writer.WriteLine("public abstract class " + baseName + "\n {");
            foreach (var type in types)
            {
                string className = type.Split(':')[0].Trim();
                string fields = type.Split(':')[1].Trim();
                defineType(writer, baseName, className, fields);
            }
            writer.WriteLine("}");
            writer.Close();
        }
    }


    private static void defineType(StreamWriter writer, string baseName, string className, string fieldList)
    {
        writer.WriteLine("class " + className + " : " + baseName + "\n {");

        writer.WriteLine("");
        string[] fields = fieldList.Split(',').Select(s => s.Trim()).ToArray();
        foreach (var field in fields)
        {
            writer.WriteLine("readonly " + field + ";");
        }

        writer.WriteLine(" " + className + "(" + fieldList + ") \n {");

        foreach (var field in fields)
        {
            var name = field.Split(' ')[1];
            writer.WriteLine("      this." + name + " = " + name + ";");
        }
        writer.WriteLine("   } \n }");
    }
}