using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApp.Solvers;

public class PolynomialSolver
{
    public static string Solve(string question)
    {
        var polynomial = question
            .Split('\n')
            .Select(x => x.Trim())
            .Last(x => x.Contains("x"))
            .Replace(" ", "");

        var coefficients = ParsePolynomial(polynomial);

        var root = FindRoot(coefficients);

        if (root == null)
            return "no roots";

        return root.Value.ToString(
            "0.##########",
            CultureInfo.InvariantCulture
        );
    }
    
    private static double[] ParsePolynomial(string polynomial)
    {
        polynomial = polynomial.Replace("-", "+-");

        var terms = polynomial
            .Split('+', StringSplitOptions.RemoveEmptyEntries);

        var dict = new Dictionary<int, double>();

        foreach (var term in terms)
        {
            var match = Regex.Match(
                term,
                @"\(?([+-]?\d*\.?\d+)\)?(?:\*x(?:\^(\d+))?)?"
            );

            if (!match.Success)
                continue;

            var coef = double.Parse(
                match.Groups[1].Value
                    .Replace("(", "")
                    .Replace(")", ""),
                CultureInfo.InvariantCulture
            );

            var power = 0;

            if (term.Contains('x'))
            {
                power = 1;

                if (match.Groups[2].Success)
                    power = int.Parse(match.Groups[2].Value);
            }

            dict[power] = coef;
        }

        var maxPower = dict.Keys.Max();

        var result = new double[maxPower + 1];

        foreach (var item in dict)
            result[item.Key] = item.Value;

        return result;
    }
    
    private static double Calculate(double[] c, double x)
    {
        double result = 0;

        for (var i = c.Length - 1; i >= 0; i--)
        {
            result = result * x + c[i];
        }

        return result;
    }
    
    private static double? FindRoot(double[] coefficients)
    {
        var step = 0.5;

        double left = -100;
        double right = 100;

        var previousX = left;
        var previousY = Calculate(coefficients, previousX);

        for (var x = left + step; x <= right; x += step)
        {
            var y = Calculate(coefficients, x);

            if (Math.Abs(y) < 0.001)
                return x;
            
            if (previousY * y < 0)
            {
                var a = previousX;
                var b = x;

                for (var i = 0; i < 100; i++)
                {
                    var mid = (a + b) / 2;
                    var value = Calculate(coefficients, mid);

                    if (Math.Abs(value) < 0.000001)
                        return mid;

                    if (Calculate(coefficients, a) * value < 0)
                        b = mid;
                    else
                        a = mid;
                }

                return (a + b) / 2;
            }

            previousX = x;
            previousY = y;
        }

        return null;
    }
}