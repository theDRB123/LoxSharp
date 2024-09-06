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
	    "Assign   	: Token name, Expr value",
            "Binary   	: Expr Left,Token Operator,Expr Right",
            "Grouping 	: Expr Expression",
            "Literal  	: Object Value",
            "Unary    	: Token Operator, Expr right",
	    "Logical  	: Expr left, Token Operator, Expr right", 
            "Conditional: Expr expr , Expr thenBranch, Expr elseBranch",
            "Variable 	: Token name"
        ]);

        defineAST(output_dir, "Stmt", [
	    "Block 	: List<Stmt> statements",
            "Expression : Expr expression",
            "Print      : Expr expression",
	    "Var	: Token name, Expr initializer",
	    "If 	: Expr condition, Stmt thenBranch, Stmt elseBranch",
	    "While	: Expr condition, Stmt body",
        ]);
    }

    private static void defineAST(string output_dir, string baseName, List<string> types)
    {
        string path = "../../Lox/" + baseName + ".cs";
        using (StreamWriter writer = new(path, false, Encoding.UTF8))
        {
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine();
            writer.WriteLine("public abstract class " + baseName + "\n {");

            defineVisitor(writer, baseName, types);

            foreach (var type in types)
            {
                string className = type.Split(':')[0].Trim();
                string fields = type.Split(':')[1].Trim();
                defineType(writer, baseName, className, fields);
            }

            writer.WriteLine("");
            writer.WriteLine("public abstract T Accept<T>(Visitor<T> visitor);");


            writer.WriteLine("}");
            writer.Close();
        }
    }

    private static void defineVisitor(StreamWriter writer, string baseName, List<string> types)
    {
        writer.WriteLine("public interface Visitor<T> \n{");

        foreach(string type in types){
            string typename = type.Split(':')[0].Trim();
            writer.WriteLine("      T Visit" + typename + baseName + "(" + typename + " " + baseName.ToLower()+ ");");

        }

        writer.WriteLine(" }");
    }

    private static void defineType(StreamWriter writer, string baseName, string className, string fieldList)
    {
        writer.WriteLine("public class " + className + " : " + baseName + "\n {");

        writer.WriteLine("");
        string[] fields = fieldList.Split(',').Select(s => s.Trim()).ToArray();
        foreach (var field in fields)
        {
            writer.WriteLine("public readonly " + field + ";");
        }

        writer.WriteLine("public " + className + "(" + fieldList + ") \n {");
        
        foreach (var field in fields)
        {
            var name = field.Split(' ')[1];
            writer.WriteLine("      this." + name + " = " + name + ";");
        }
        writer.WriteLine("}");

        writer.WriteLine("public override T Accept<T> (Visitor<T> visitor) \n{");
        writer.WriteLine("      return visitor.Visit" + className + baseName + "(this);");
        writer.WriteLine("}");

       
        
        writer.WriteLine("}");
    }
}
