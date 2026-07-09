using System;
using System.Globalization;
using System.Linq;

namespace ConsoleApp.Solvers;

public class MatrixSolver
{
    public static string Solve(string question)
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