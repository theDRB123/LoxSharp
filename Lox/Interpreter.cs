using static TokenType;
using static LoxErrors;
using static LoxControlFlow;
public class Interpreter : Expr.Visitor<Object>, Stmt.Visitor<object>
{
    public bool isREPL = false;
    public readonly Env globals = new();
    private Env environment;
    // private bool breakFlag = false;
    public Interpreter()
    {
        globals.Define("clock", new Clock());
        environment = globals;
    }
    public void Interpret(List<Stmt> statements)
    {
        foreach (var stmt in statements)
        {
            try
            {
                execute(stmt);
            }
            catch (RuntimeError error)
            {
                Lox.runtimeError(error);
            }
        }
    }

    public void executeBlock(List<Stmt> statements, Env env)
    {
        Env previous = this.environment;
        try
        {
            environment = env;

            foreach (var statement in statements)
            {
                execute(statement);
            }
        }
        catch (RuntimeError err)
        {
            ThrowRuntimeError(err.token, "Skipping Block | " + err.Message);
            // breakFlag = true;
        }
        finally
        {
            environment = previous;
        }
    }

    private void execute(Stmt stmt)
    {
        stmt.Accept(this);
    }

    private object stringify(object value)
    {
        if (value == null) return null;

        if (value is double)
        {
            return (double)value;
        }

        return value.ToString();
    }
    public object VisitAssignExpr(Expr.Assign expr)
    {
        object value = evaluate(expr.value);
        environment.Assign(expr.name, value);
        return value;
    }
    public object VisitLogicalExpr(Expr.Logical expr)
    {
        object left = evaluate(expr.left);

        if (expr.Operator.type == OR)
        {
            if (isTruty(left)) return left;
        }
        else
        {
            if (!isTruty(left)) return left;
        }

        return evaluate(expr.right);
    }
    public object VisitBinaryExpr(Expr.Binary expr)
    {
        object left = evaluate(expr.Left);
        object right = evaluate(expr.Right);
        Token Operator = expr.Operator;

        bool check = checkOperands(left, right);
        object output = expr.Operator.type switch
        {
            MINUS => check ? (double)left - (double)right : null,
            SLASH => check ? handleDivisionOperator(left, right, expr.Operator) : null,
            STAR => check ? (double)left * (double)right : null,
            PLUS => handleArithmaticOperator(left, right, expr.Operator),


            GREATER => check ? (double)left > (double)right : handleStringComparison(left, right, expr.Operator),
            GREATER_EQUAL => check ? (double)left >= (double)right : handleStringComparison(left, right, expr.Operator),
            LESS => check ? (double)left < (double)right : handleStringComparison(left, right, expr.Operator),
            LESS_EQUAL => check ? (double)left <= (double)right : handleStringComparison(left, right, expr.Operator),

            EQUAL_EQUAL => isEqual(left, right),
            BANG_EQUAL => !isEqual(left, right),
            _ => null,
        };
        return output ?? ThrowRuntimeError(Operator);
    }

    private object handleDivisionOperator(object left, object right, Token Operator)
    {
        if ((double)right == 0)
        {
            ThrowRuntimeError(Operator, "PANIC PANIC PANIC !! division by zero detected");
        }
        return (double)left / (double)right;
    }

    private object handleStringComparison(object left, object right, Token Operator)
    {
        if (left is not string || right is not string) ThrowRuntimeError(Operator, "Dont compare Apples with Oranges");
        string l = left.ToString();
        string r = right.ToString();
        return stringify(Operator.type switch
        {
            GREATER => l.CompareTo(r) > 0,
            GREATER_EQUAL => l.CompareTo(r) >= 0,
            LESS => l.CompareTo(r) < 0,
            LESS_EQUAL => l.CompareTo(r) <= 0,
            _ => null,
        });
    }



    private bool isEqual(object left, object right)
    {
        if (left == null && right == null) { return true; }
        if (left == null) { return false; }
        return left.Equals(right);
    }

    private object handleArithmaticOperator(object left, object right, Token Operator)
    {
        if (left is double && right is double)
        {
            return (double)left + (double)right;
        }
        if (left is string or double && right is string or double)
        {
            return left.ToString() + right.ToString();
        }
        return null;
    }


    public object VisitConditionalExpr(Expr.Conditional conditional)
    {
        object condition = evaluate(conditional.expr);
        if (condition is not bool)
        {
            ThrowRuntimeError(null, "Dude I need a boolean for a conditional");
        }
        if ((bool)condition == true)
        {
            return evaluate(conditional.thenBranch);
        }
        else
        {
            return evaluate(conditional.elseBranch);
        }
    }

    public object VisitGroupingExpr(Expr.Grouping expr) => evaluate(expr.Expression);
    public object VisitLiteralExpr(Expr.Literal expr) => stringify(expr.Value);
    public object VisitCallExpr(Expr.Call expr)
    {
        object callee = evaluate(expr.callee);

        List<object> arguements = [];
        foreach (var arguement in expr.arguements)
        {
            arguements.Add(evaluate(arguement));
        }

        if (callee is not ICallable)
        {
            ThrowRuntimeError(expr.paren, "Dude only functions and classes can be called");
        }

        ICallable function = (ICallable)callee;
        if (arguements.Count != function.Arity())
        {
            ThrowRuntimeError(expr.paren, $"The function wants precisely {function.Arity()} arguement(s), not more not less");
        }
        return function.Call(this, arguements);
    }
    public object VisitUnaryExpr(Expr.Unary expr)
    {
        object right = evaluate(expr.right);
        bool check = checkOperand(right);
        object output = expr.Operator.type switch
        {
            MINUS => check ? -(double)right : null,
            BANG => !isTruty(right),
            _ => null
        };

        return output ?? ThrowRuntimeError(expr.Operator);
    }
    public object VisitVariableExpr(Expr.Variable expr)
    {
        return environment.Get(expr.name);
    }
    private bool checkOperand(object right) => right is double;
    private bool checkOperands(object Left, object Right) => Left is double && Right is double;


    private bool isTruty(object right)
    {
        if (right == null) { return false; }
        if (right is bool boolean) { return boolean; }
        return true;
    }
    private object evaluate(Expr expression) => expression.Accept(this);



    //Statements
    public object VisitBlockStmt(Stmt.Block stmt)
    {
        executeBlock(stmt.statements, new Env(environment));
        return null;
    }
    public object VisitExpressionStmt(Stmt.Expression stmt)
    {
        object value = evaluate(stmt.expression);
        if (isREPL)
        {
            Console.WriteLine(value);
        }
        return null;
    }

    public object VisitPrintStmt(Stmt.Print stmt)
    {
        object value = evaluate(stmt.expression);
        Console.WriteLine(value ?? "nil");
        return null;
    }

    public object VisitVarStmt(Stmt.Var stmt)
    {
        object value = null;
        if (stmt.initializer != null)
        {
            value = evaluate(stmt.initializer);
        }

        environment.Define(stmt.name.lexeme, value);
        return null;
    }

    public object VisitIfStmt(Stmt.If stmt)
    {
        if (isTruty(evaluate(stmt.condition)))
        {
            execute(stmt.thenBranch);
        }
        else
        {
            if (stmt.elseBranch != null)
            {
                execute(stmt.elseBranch);
            }
        }
        return null;
    }

    public object VisitWhileStmt(Stmt.While stmt)
    {
        while (isTruty(evaluate(stmt.condition)))
        {
            try
            {
                execute(stmt.body);
            }
            catch (Break)
            {
                break;
            }
        }
        return null;
    }

    public object VisitBreakStmt(Stmt.Break stmt)
    {
        if (stmt.type.type == BREAK)
        {
            if (environment.enclosing == null)
            {
                ThrowRuntimeError(stmt.type, "you need a loop to have a break ~sun tzu!!");
                return null;
            }
            else
            {
                Break();
            }
        }
        return null;
    }

    public object VisitReturnStmt(Stmt.Return stmt)
    {
        if (environment.enclosing == null)
        {
            ThrowRuntimeError(stmt.keyword, "I have not place to return to!!");
            return null;
        }
        object value = null;
        if (stmt.value != null) value = evaluate(stmt.value);

        Return(value);
        return null;
    }

    public object VisitFunctionStmt(Stmt.Function stmt)
    {
        LoxFunction function = new(stmt, environment);
        environment.Define(stmt.name.lexeme, function);
        return null;
    }
}