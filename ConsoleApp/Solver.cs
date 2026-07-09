using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Challenge.DataContracts;

namespace ConsoleApp;

public class Solver
{
    private static readonly Dictionary<string, Func<string, string>> Solvers = new()
    {
        {"determinant", SolveMatrixDet},
        {"polynomial-root", SolvePolynomialRoot}
    };
    
    public static string Solve(TaskResponse taskResponse)
    {
        if (Solvers.TryGetValue(taskResponse.TypeId, out var solver))
            return solver(taskResponse.Question);
        
        throw new ArgumentException($"Unknown task type: {taskResponse.TypeId}");
    }
    
    private static string SolvePolynomialRoot(string question)
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
    
    private static string SolveMatrixDet(string question)
    {
        var matrixText = question
            .Split('\n')
            .Select(x => x.Trim())
            .Last(x => !string.IsNullOrWhiteSpace(x));

        var rows = matrixText
            .Split([@"\\"], StringSplitOptions.RemoveEmptyEntries)
            .Select(r => r.Trim())
            .ToArray();

        var matrix = rows
            .Select(r => r.Split('&')
                .Select(x => double.Parse(x.Trim(), CultureInfo.InvariantCulture))
                .ToArray())
            .ToArray();

        var det = Determinant(matrix);

        if (Math.Abs(det - Math.Round(det)) < 1e-9)
            return ((long)Math.Round(det)).ToString();

        return det.ToString(CultureInfo.InvariantCulture);
    }
    
    private static double Determinant(double[][] matrix)
    {
        var n = matrix.Length;

        if (n == 1)
            return matrix[0][0];

        if (n == 2)
            return matrix[0][0] * matrix[1][1]
                   - matrix[0][1] * matrix[1][0];

        double det = 0;

        for (var c = 0; c < n; c++)
        {
            var minor = Minor(matrix, 0, c);

            det += (c % 2 == 0 ? 1 : -1)
                   * matrix[0][c]
                   * Determinant(minor);
        }

        return det;
    }

    private static double[][] Minor(double[][] matrix, int row, int col)
    {
        var n = matrix.Length;

        var result = new double[n - 1][];

        var r = 0;

        for (var i = 0; i < n; i++)
        {
            if (i == row)
                continue;

            result[r] = new double[n - 1];

            var c = 0;

            for (var j = 0; j < n; j++)
            {
                if (j == col)
                    continue;

                result[r][c++] = matrix[i][j];
            }

            r++;
        }

        return result;
    }
}
