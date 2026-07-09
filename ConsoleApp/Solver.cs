using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Challenge.DataContracts;
using ConsoleApp.Solvers;

namespace ConsoleApp;

public class Solver
{
    private static readonly Dictionary<string, Func<string, string>> Solvers = new()
    {
        {"determinant", MatrixSolver.Solve},
        {"polynomial-root", PolynomialSolver.Solve},
    };
    
    public static string Solve(TaskResponse taskResponse)
    {
        if (Solvers.TryGetValue(taskResponse.TypeId, out var solver))
            return solver(taskResponse.Question);
        
        throw new ArgumentException($"Unknown task type: {taskResponse.TypeId}");
    }
}
