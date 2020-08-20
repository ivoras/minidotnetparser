using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;

public class DialogActionParserException : Exception
{
    public DialogActionParserException(string msg) : base(msg) { }
}


public class DialogActionValue
{
    public enum Type
    {
        Identifier,
        String,
        Bool,
        Number,
        Operator
    };
    public Type type;

    private string s;
    private float f;
    private bool b;

    public string String
    {
        get
        {
            if (type != Type.String)
            {
                throw new DialogActionParserException($"Not a string. {type}");
            }
            return s;
        }
    }

    public string Identifier
    {
        get
        {
            if (type != Type.Identifier)
            {
                throw new DialogActionParserException($"Not an identifier. {type}");
            }
            return s;
        }
    }

    public bool Bool
    {
        get
        {
            if (type != Type.Bool)
            {
                throw new DialogActionParserException($"Not a Bool. {type}");
            }
            return b;
        }
    }

    public float Number
    {
        get
        {
            if (type != Type.Number)
            {
                throw new DialogActionParserException($"Not a number. {type}");
            }
            return f;
        }
    }

    public string Operator
    {
        get
        {
            if (type != Type.Operator)
            {
                throw new DialogActionParserException($"Not an operator. {type}");
            }
            return s;
        }
    }

    public DialogActionValue(Type t, string s)
    {
        if (t != Type.String && t != Type.Identifier && t != Type.Operator)
        {
            throw new DialogActionParserException($"This constructor form is only applicable to strings, identifiers and operators");
        }
        type = t;
        this.s = s;
    }

    public DialogActionValue(Type t, float f)
    {
        if (t != Type.Number)
        {
            throw new DialogActionParserException($"This constructor form is only applicable to numbers");
        }
        type = t;
        this.f = f;
    }

    public DialogActionValue(Type t, bool b)
    {
        if (t != Type.Bool)
        {
            throw new DialogActionParserException($"This constructor form is only applicable to bools");
        }
        type = t;
        this.b = b;
    }

    public override string ToString()
    {
        switch(type)
        {
            case Type.String:
                return $"<s {s}>";
            case Type.Identifier:
                return $"<i {s}>";
            case Type.Operator:
                return $"<o {s}>";
            case Type.Number:
                return $"<n {f}>";
            case Type.Bool:
                return $"<b {b}>";
            default:
                throw new DialogActionParserException($"Unknown type {type}");
        }
    }

}

public class DialogAction
{
    public string action;
    public List<DialogActionValue> args;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder($"{action} ");
        foreach (var a in args)
        {
            sb.Append(a.ToString());
            sb.Append(" ");
        }
        
        return sb.ToString();
    }
}

public class DialogActionParser
{
    private const string AlphaString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string AlphanumericString = AlphaString + "0123456789";
    private const string IdentifierString = AlphanumericString + "_";
    private const string NumberString = "0123456789.";
    private const string QuoteString = "\"'";
    private const string OperatorString = "=+-*/";

    public static List<DialogAction> Parse(string inputStr)
    {
        var actions = new List<DialogAction>();

        foreach (var l in inputStr.Split(new char[] { '\n' }))
        {
            var line = l;
            line = line.Substring(gobbleWhitespace(line));
            Tuple<int, string> cmd = gobbleIdentifier(line);
            if (cmd.Item1 == 0)
            {
                throw new DialogActionParserException("Cannot find cmd");
            }
            line = line.Substring(cmd.Item1);

            var action = new DialogAction {
                action = cmd.Item2,
                args = new List<DialogActionValue>()
            };

            while (line.Length > 0)
            {
                line = line.Substring(gobbleWhitespace(line));
                if (line.Length == 0)
                {
                    break;
                }

                Tuple<int, bool> argB = gobbleBool(line);
                if (argB.Item1 != 0)
                {
                    action.args.Add(new DialogActionValue(DialogActionValue.Type.Bool, argB.Item2));
                    line = line.Substring(argB.Item1);
                    continue;
                }

                Tuple<int, float> argN = gobbleNumber(line);
                if (argN.Item1 != 0)
                {
                    action.args.Add(new DialogActionValue(DialogActionValue.Type.Number, argN.Item2));
                    line = line.Substring(argN.Item1);
                    continue;
                }

                Tuple<int, string> argS = gobbleIdentifier(line);
                if (argS.Item1 != 0)
                {
                    action.args.Add(new DialogActionValue(DialogActionValue.Type.Identifier, argS.Item2));
                    line = line.Substring(argS.Item1);
                    continue;
                }

                argS = gobbleString(line);
                if (argS.Item1 != 0)
                {
                    action.args.Add(new DialogActionValue(DialogActionValue.Type.String, argS.Item2));
                    line = line.Substring(argS.Item1);
                    continue;
                }

                argS = gobbleOperator(line);
                if (argS.Item1 != 0)
                {
                    action.args.Add(new DialogActionValue(DialogActionValue.Type.Operator, argS.Item2));
                    line = line.Substring(argS.Item1);
                    continue;
                }

                throw new DialogActionParserException($"Unknown token at [{line}]");
            }

            actions.Add(action);
        }
        return actions;
    }

    private static int gobbleWhitespace(string s)
    {
        var i = 0;
        while (i < s.Length && (s[i] == ' ' || s[i] == '\t'))
        {
            i++;
        }
        return i;
    }

    private static Tuple<int, string> gobbleIdentifier(string line)
    {
        if (line.Length == 0)
        {
            return new Tuple<int, string>(0, "");
        }
        var i = 0;
        while (i < line.Length && IdentifierString.IndexOf(line[i]) != -1)
        {
            i++;
        }
        return new Tuple<int, string>(i, line.Substring(0, i));
    }

    private static Tuple<int, string> gobbleString(string line)
    {
        if (line.Length == 0)
        {
            return new Tuple<int, string>(0, "");
        }
        var q = line[0];
        if (QuoteString.IndexOf(q) == -1)
        {
            return new Tuple<int, string>(0, "");
        }
        line = line.Substring(1);
        var i = 0;
        var inEscape = false;
        StringBuilder sb = new StringBuilder();
        while (i < line.Length)
        {
            var ch = line[i];
            if (ch == '\\')
            {
                inEscape = true;
                i++;
                continue;
            }
            if (inEscape)
            {
                if (ch == q)
                {
                    sb.Append(ch);
                    inEscape = false;
                } else if (ch == 'n')
                {
                    sb.Append('\n');
                    inEscape = false;
                } else if (ch == 't')
                {
                    sb.Append('\t');
                    inEscape = false;
                } else
                {
                    throw new ExecutionEngineException($"Unknown escape: \\{ch} at [{line}]");
                }
                i++;
                continue;
            }
            if (ch == q)
            {
                i++;
                i++;
                break;
            }
            sb.Append(ch);
            i++;
        }
        return new Tuple<int, string>(i, sb.ToString());
    }

    private static Tuple<int, float>gobbleNumber(string line)
    {
        if (line.Length == 0)
        {
            return new Tuple<int, float>(0, 0);
        }
        int i = 0;
        while (i < line.Length && NumberString.IndexOf(line[i]) != -1)
        {
            i++;
        }
        if (i == 0)
        {
            return new Tuple<int, float>(0, 0);
        }
        var nstr = line.Substring(0, i);
        try
        {
            var f = float.Parse(nstr, CultureInfo.InvariantCulture.NumberFormat);
            return new Tuple<int, float>(i, f);
        } catch (Exception e)
        {
            throw new DialogActionParserException($"Error parsing number at {line}");
        }
    }

    private static Tuple<int, bool> gobbleBool(string line)
    {
        if (line.Length == 0)
        {
            return new Tuple<int, bool>(0, false);
        }
        var tok = gobbleIdentifier(line);
        if (tok.Item1 != 0)
        {
            if (tok.Item2 == "true")
            {
                return new Tuple<int, bool>(tok.Item1, true);
            } else if (tok.Item2 == "false")
            {
                return new Tuple<int, bool>(tok.Item1, false);
            } else
            {
                return new Tuple<int, bool>(0, false);
            }
        }
        return new Tuple<int, bool>(0, false);
    }

    private static Tuple<int, string> gobbleOperator(string line)
    {
        var i = 0;
        while (i < line.Length && OperatorString.IndexOf(line[i]) != -1)
        {
            i++;
        }
        return new Tuple<int, string>(i, line.Substring(0, i));

    }

}
