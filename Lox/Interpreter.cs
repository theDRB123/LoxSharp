using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using static TokenType;

public class Interpreter : Expr.Visitor<Object>
{
    public void Interpret(Expr expression){
        try{
            object value = evaluate(expression);
            Console.WriteLine(stringify(value));
        } catch(RuntimeError error){
            Lox.runtimeError(error);
        }
    }

    private string stringify(object value)
    {
        if (value == null) return "nil";

        if(value is double) {
            string text = value.ToString();
            if(text.EndsWith(".0")){
                text = text.Substring(0, text.Length - 2);
            }
            return text;
        }

        return value.ToString();
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
            SLASH => check ? (double)left / (double)right : null,
            STAR => check ? (double)left * (double)right : null,
            PLUS => handleArithmaticOperator(left, right),

            GREATER => check ? (double)left > (double)right : null,
            GREATER_EQUAL => check ? (double)left >= (double)right : null,
            LESS => check ? (double)left < (double)right : null,
            LESS_EQUAL => check ? (double)left <= (double)right : null,

            EQUAL_EQUAL => isEqual(left, right),
            BANG_EQUAL => !isEqual(left, right),
            _ => null,
        };
        return output ?? throwError(Operator);
    }

    private bool isEqual(object left, object right)
    {
        if (left == null && right == null) { return true; }
        if (left == null) { return false; }
        return left.Equals(right);
    }

    private object handleArithmaticOperator(object left, object right)
    {
        if (left is double dlft && right is double drgt)
        {
            return dlft + drgt;
        }
        if (left is string slft && right is string srgt)
        {
            return slft + srgt;
        }
        return null;
    }

    public object VisitConditionalExpr(Expr.Conditional conditional)
    {
        throw new NotImplementedException();
    }

    public object VisitGroupingExpr(Expr.Grouping expr) => evaluate(expr.Expression);
    public object VisitLiteralExpr(Expr.Literal expr) => expr.Value;
    public object VisitUnaryExpr(Expr.Unary expr)
    {

        Object right = evaluate(expr.right);
        bool check = checkOperands(expr.Operator, right);

        object output = expr.Operator.type switch
        {
            MINUS => check ? -(double)right : null,
            BANG => !isTruty(right),
            _ => null
        };

        return output ?? throwError(expr.Operator);
    }

    private bool checkOperands(object right) => right is double;
    private bool checkOperands(object Left, object Right) => Left is double && Right is double;
    private bool throwError(Token Operator)
    {
        if (Operator.type == PLUS)
        {
            throw new RuntimeError(Operator, "Operands must be a numbers or strings");
        }
        throw new RuntimeError(Operator, "Operand(s) must be numbers");
    }
    private bool isTruty(object right)
    {
        if (right == null) { return false; }
        if (right is bool boolean) { return boolean; }
        return true;
    }
    private object evaluate(Expr expression) => expression.Accept(this);
}