using System.Collections;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static TokenType;


public class Scanner
{
    private readonly string source;
    private List<Token> tokens = [];
    private readonly Dictionary<string, TokenType> keywords = new()
    {
        { "and", AND},
        { "class" , CLASS},
        { "else", ELSE },
        { "false", FALSE },
        { "for", FOR },
        { "fn", FUN },
        { "if", IF },
        { "nil", NIL },
        { "or", OR },
        { "print", PRINT },
        { "return", RETURN },
        { "super", SUPER },
        { "this", THIS },
        { "true", TRUE },
        { "var", VAR },
        { "while", WHILE },
        { "break", BREAK}
    };

    private int start = 0;
    private int current = 0;
    private int line = 1;

    public Scanner(string source)
    {
        this.source = source;
    }
    public List<Token> scanTokens()
    {
        while (!isAtTheEnd())
        {
            start = current;
            scanToken();
        }

        tokens.Add(new Token(EOF, "", null, line));
        return tokens;
    }

    private void scanToken()
    {
        char c = advance();
        TokenType type = c switch
        {
            '(' => LEFT_PAREN,
            ')' => RIGHT_PAREN,
            '{' => LEFT_BRACE,
            '}' => RIGHT_BRACE,
            ',' => COMMA,
            '.' => DOT,
            '-' => MINUS,
            '+' => PLUS,
            ';' => SEMICOLON,
            '*' => STAR,
            '?' => QUESTION,
            ':' => COLON,
            '!' => match('=') ? BANG_EQUAL : BANG,
            '=' => match('=') ? EQUAL_EQUAL : EQUAL,
            '<' => match('=') ? LESS_EQUAL : LESS,
            '>' => match('=') ? GREATER_EQUAL : GREATER,
            '/' => Comment() ? NULL : SLASH,
            ' ' => NULL,
            '\r' => NULL,
            '\t' => NULL,
            '\n' => newLine(),
            // handling string literals
            '"' => stringLiteral(),
            // _ => isDigit(c) ? number() : INVALID,
            _ => handleDefault(c)
        };
        if (type == INVALID)
        {
            Lox.error(line, "I can only eat ASCII you dum dum");
        }
        if (type != NULL)
        {
            addToken(type);
        }

    }



    private TokenType handleDefault(char c)
    {
        if (isDigit(c))
        {
            number();
            return NULL;
        }
        else if (isAlpha(c))
        {
            indentifier();
            return NULL;
        }
        Lox.error(line, "Wtf is this character dude");
        return INVALID;
    }

    private void indentifier()
    {
        while (isAlphaNumeric(peek())) advance();
        string text = source[start..current];
        if (keywords.TryGetValue(text, out TokenType type))
        {
            addToken(type);
        }
        else
        {
            addToken(INDENTIFIER);
        }
    }

    private bool isAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
    }
    private bool isAlphaNumeric(char c)
    {
        return isAlpha(c) || isDigit(c);
    }

    private TokenType number()
    {
        while (isDigit(peek())) advance();

        //fractional,
        if (peek() == '.' && isDigit(peekNext()))
        {
            advance();
            while (isDigit(peek())) advance();
        }

        addToken(NUMBER, Double.Parse(source[start..current]));
        return NULL;
    }

    private bool isDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private TokenType stringLiteral()
    {
        while (peek() != '"' && !isAtTheEnd())
        {
            if (peek() == '\n') line++;
            advance();
        }

        if (isAtTheEnd())
        {
            Lox.error(line, "Could you not atleast complete the string T_T");
            return NULL;
        }

        advance();

        string value = source[(start + 1)..(current - 1)];
        addToken(STRING, value);
        return NULL;
    }

    private TokenType newLine()
    {
        line++;
        return NULL;
    }
    private bool Comment()
    {
        if (CommentBlock())
        {
            return true;
        }
        if (CommentLine())
        {
            return true;
        }
        return false;

    }

    private bool CommentBlock()
    {
        if (match('*'))
        {
            while (true)
            {
                if (isAtTheEnd())
                {
                    Lox.error(line, "Comment not terminated");
                    return true;
                }
                char next = advance();
                switch (next)
                {   
                    case '/':
                        advance();
                        if(CommentBlock()){
                            continue;
                        };
                        break;
                    case '*':
                        
                    default:
                        break;
                }
                if (peek() == '/')
                {
                    advance();
                    if(CommentBlock())
                    {
                        continue;
                    };
                }

                if (peek() == '*')
                {
                    advance();
                    if (peek() == '/')
                    {
                        advance();
                        break;
                    }
                }
            }
            return true;
        }
        return false;
    }
    private bool CommentLine()
    {
        if (match('/'))
        {
            while (peek() != '\n' && !isAtTheEnd())
            {
                advance();
            }
            return true;
        }
        return false;
    }

    private char peek()
    {
        return isAtTheEnd() ? '\0' : source[current];
    }

    private char peekNext()
    {
        return isAtTheEnd() ? '\0' : source[current + 2];
    }

    private bool match(char expected)
    {
        if (isAtTheEnd()) return false;
        if (source[current] != expected) return false;
        current++;
        return true;
    }
    private void addToken(TokenType type)
    {
        addToken(type, null);
    }
    private void addToken(TokenType type, Object literal)
    {
        string text = source[start..current];
        tokens.Add(new(type, text, literal, line));
    }

    private char advance()
    {
        return source[current++];
    }

    private bool isAtTheEnd()
    {
        return current >= source.Length;
    }


}