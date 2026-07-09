using System;
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
            .Last(x => x.Contains("x"));

        polynomial = polynomial.Replace(" ", "");
        
        var linearMatch = Regex.Match(
            polynomial,
            @"\(?([+-]?\d*\.?\d+)\)?\*x([+-]\(?[+-]?\d*\.?\d+\)?)"
        );

        if (linearMatch.Success && !polynomial.Contains("^2"))
        {
            var aLinear = double.Parse(
                linearMatch.Groups[1].Value,
                CultureInfo.InvariantCulture
            );

            var bLinear = double.Parse(
                linearMatch.Groups[2].Value
                    .Replace("(", "")
                    .Replace(")", ""),
                CultureInfo.InvariantCulture
            );

            var root = -bLinear / aLinear;

            return root.ToString(
                "0.##########",
                CultureInfo.InvariantCulture
            );
        }

        var match = Regex.Match(
            polynomial,
            @"\(?([+-]?\d*\.?\d+)\)?\*x\^2\+\(?([+-]?\d*\.?\d+)\)?\*x\+\(?([+-]?\d*\.?\d+)\)?"
        );

        if (!match.Success)
            throw new ArgumentException($"Unknown polynomial format: {polynomial}");

        var a = double.Parse(
            match.Groups[1].Value,
            CultureInfo.InvariantCulture
        );

        var b = double.Parse(
            match.Groups[2].Value,
            CultureInfo.InvariantCulture
        );

        var c = double.Parse(
            match.Groups[3].Value,
            CultureInfo.InvariantCulture
        );

        var d = b * b - 4 * a * c;

        if (d < 0)
            return "no roots";
        
        var root1 = (-b + Math.Sqrt(d)) / (2 * a);

        return root1.ToString(
            "0.##########",
            CultureInfo.InvariantCulture
        );
    }
}