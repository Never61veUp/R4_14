using System;

namespace ConsoleApp.Solvers;

public static class MathSolver
{
    private static string _s = "";
    private static int _pos;

    public static string Solve(string question)
    {
        _s = question.Replace(" ", "").Replace("=", "");
        _pos = 0;
        return ParseExpression().ToString();
    }

    private static long ParseExpression()
    {
        var value = ParseTerm();

        while (_pos < _s.Length)
        {
            if (_s[_pos] == '+')
            {
                _pos++;
                value += ParseTerm();
            }
            else if (_s[_pos] == '-')
            {
                _pos++;
                value -= ParseTerm();
            }
            else
                break;
        }

        return value;
    }

    private static long ParseTerm()
    {
        var value = ParseFactor();

        while (_pos < _s.Length)
        {
            if (_s[_pos] == '*')
            {
                _pos++;
                value *= ParseFactor();
            }
            else if (_s[_pos] == '/')
            {
                _pos++;
                value /= ParseFactor();
            }
            else if (_s[_pos] == '%')
            {
                _pos++;
                value %= ParseFactor();
            }
            else
                break;
        }

        return value;
    }

    private static long ParseFactor()
    {
        if (_s[_pos] == '(')
        {
            _pos++;
            var value = ParseExpression();
            _pos++;
            return value;
        }

        if (char.IsLetter(_s[_pos]))
        {
            var name = "";

            while (_pos < _s.Length && char.IsLetter(_s[_pos]))
            {
                name += _s[_pos];
                _pos++;
            }

            var a = ParseFunctionArgument();
            var b = ParseFunctionArgument();

            return name switch
            {
                "min" => Math.Min(a, b),
                "max" => Math.Max(a, b),
                "left" => a,
                "right" => b,
                "sum" => a + b,
                "mul" => a * b,
                _ => throw new Exception("Unknown function")
            };
        }

        var negative = false;

        if (_s[_pos] == '-')
        {
            negative = true;
            _pos++;
        }

        long number = 0;

        while (_pos < _s.Length && char.IsDigit(_s[_pos]))
        {
            number = number * 10 + (_s[_pos] - '0');
            _pos++;
        }

        return negative ? -number : number;
    }

    private static long ParseFunctionArgument()
    {
        if (_s[_pos] != '(')
            throw new Exception();

        _pos++;

        var value = ParseExpression();

        _pos++;

        return value;
    }
}