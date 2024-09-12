public class Resolver : Expr.Visitor<object>, Stmt.Visitor<object>
{
    private enum FunctionType
    {
        NONE,
        FUNCTION
    }
    private readonly Interpreter interpreter;
    private readonly Stack<Dictionary<string, bool>> scopes = new();
    private FunctionType currentFunction = FunctionType.NONE;
    public Resolver(Interpreter interpreter)
    {
        this.interpreter = interpreter;
    }
    public object VisitBlockStmt(Stmt.Block stmt)
    {
        BeginScope();
        Resolve(stmt.statements);
        EndScope();
        return null;
    }

    public object VisitVarStmt(Stmt.Var stmt)
    {
        Declare(stmt.name);
        if (stmt.initializer != null)
        {
            Resolve(stmt.initializer);
        }
        Define(stmt.name);
        return null;
    }

    public object VisitVariableExpr(Expr.Variable expr)
    {
        if (scopes.Count == 0) LoxErrors.ThrowRuntimeError(expr.name, "You put the variable in its own initializer :(");
        ResolveLocal(expr, expr.name);
        return null;
    }

    public object VisitAssignExpr(Expr.Assign expr)
    {
        Resolve(expr.value);
        ResolveLocal(expr, expr.name);
        return null;
    }

    public object VisitBinaryExpr(Expr.Binary expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return null;
    }

    public object VisitGroupingExpr(Expr.Grouping expr)
    {
        Resolve(expr.Expression);
        return null;
    }

    public object VisitLiteralExpr(Expr.Literal expr)
    {
        return null;
    }

    public object VisitUnaryExpr(Expr.Unary expr)
    {
        Resolve(expr.right);
        return null;
    }

    public object VisitLogicalExpr(Expr.Logical expr)
    {
        Resolve(expr.left);
        Resolve(expr.left);
        return null;
    }

    public object VisitConditionalExpr(Expr.Conditional expr)
    {
        Resolve(expr.thenBranch);
        Resolve(expr.elseBranch);
        return null;
    }

    public object VisitCallExpr(Expr.Call expr)
    {
        Resolve(expr.callee);
        foreach (Expr arg in expr.arguements)
        {
            Resolve(arg);
        }
        return null;
    }

    public object VisitExpressionStmt(Stmt.Expression stmt)
    {
        Resolve(stmt.expression);
        return null;
    }

    public object VisitFunctionStmt(Stmt.Function stmt)
    {
        Declare(stmt.name);
        Define(stmt.name);

        ResolveFunction(stmt, FunctionType.FUNCTION);
        return null;
    }

    public object VisitPrintStmt(Stmt.Print stmt)
    {
        Resolve(stmt.expression);
        return null;
    }

    public object VisitIfStmt(Stmt.If stmt)
    {
        Resolve(stmt.condition);
        Resolve(stmt.thenBranch);
        if (stmt.elseBranch != null) Resolve(stmt.elseBranch);
        return null;
    }

    public object VisitWhileStmt(Stmt.While stmt)
    {
        Resolve(stmt.condition);
        Resolve(stmt.body);
        return null;
    }

    public object VisitBreakStmt(Stmt.Break stmt)
    {
        return null;
    }

    public object VisitReturnStmt(Stmt.Return stmt)
    {
        if (currentFunction == FunctionType.NONE)
        {
            LoxErrors.ThrowResolverError(stmt.keyword, "UHM... Excuse me, I have nowhere to return to !!");
        }
        if (stmt.value != null)
        {
            Resolve(stmt.value);
        }
        return null;
    }

    //helpers

    private void Define(Token name)
    {
        if (scopes.Count == 0) return;

        scopes.Peek()[name.lexeme] = true;
    }

    private void Declare(Token name)
    {
        if (scopes.Count == 0) return;

        var scope = scopes.Peek();
        if (scope.ContainsKey(name.lexeme))
        {
            LoxErrors.ThrowResolverError(name, "The scope already has this variable dummy");
        }
    }

    public void Resolve(List<Stmt> statements)
    {
        foreach (var stmt in statements)
        {
            Resolve(stmt);
        }
    }

    public void Resolve(Stmt stmt)
    {
        try
        {
            stmt.Accept(this);
        }catch(ResolverError err){

        }
    }

    public void Resolve(Expr expr)
    {
        expr.Accept(this);
    }

    private void ResolveFunction(Stmt.Function function, FunctionType type)
    {
        FunctionType enclosingFunction = type;
        currentFunction = type;
        BeginScope();
        foreach (Token param in function.parameters)
        {
            Declare(param);
            Define(param);
        }
        Resolve(function.body);
        EndScope();
        currentFunction = enclosingFunction;
    }

    private void BeginScope()
    {
        scopes.Push([]);
    }

    private void EndScope()
    {
        scopes.Pop();
    }

    private void ResolveLocal(Expr expr, Token name)
    {
        for (int i = scopes.Count - 1; i >= 0; i--)
        {
            if (scopes.ElementAt(i).ContainsKey(name.lexeme))
            {
                interpreter.Resolve(expr, scopes.Count - 1 - i);
                return;
            }
        }
    }
}
