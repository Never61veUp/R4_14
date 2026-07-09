using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApp.Solvers;

public class ShapeSolver
{
    public static string Solve(string question)
    {
        var answersMatch = Regex.Match(
            question,
            @"Possible answers=([^\r\n|]+)"
        );

        if (!answersMatch.Success)
            throw new Exception("Answers not found");
        
        var answers = answersMatch.Groups[1]
            .Value
            .Split(',')
            .Select(x => x.Trim())
            .ToList();
        
        var points = Regex.Matches(
                question,
                @"\((-?\d+),\s*(-?\d+)\)"
            )
            .Select(m => new Point(
                int.Parse(m.Groups[1].Value),
                int.Parse(m.Groups[2].Value)))
            .ToList();
        
        if (points.Count < 3)
            throw new Exception("Not enough points");
        
        return answers
            .OrderBy(answer => ShapeError(points, answer))
            .First();
    }


    private static double ShapeError(
        List<Point> points,
        string shape)
    {
        var minX = points.Min(p => p.X);
        var maxX = points.Max(p => p.X);

        var minY = points.Min(p => p.Y);
        var maxY = points.Max(p => p.Y);
        
        var width = maxX - minX;
        var height = maxY - minY;
        
        if (width == 0 || height == 0)
            return double.MaxValue;
        
        var centerX = (minX + maxX) / 2.0;
        var centerY = (minY + maxY) / 2.0;
        
        switch (shape)
        {
            case "circle":
            {
                var distances = points
                    .Select(p =>
                        Distance(
                            p.X,
                            p.Y,
                            centerX,
                            centerY))
                    .ToList();
                
                var avgRadius = distances.Average();
                
                var radiusError = distances
                    .Select(d => Math.Abs(d - avgRadius))
                    .Average()
                    / avgRadius;
                
                var aspectError =
                    Math.Abs(width - height)
                    / Math.Max(width, height);
                
                return radiusError + aspectError;
            }

            case "square":
            {
                var aspectError =
                    Math.Abs(width - height)
                    / Math.Max(width, height);
                
                var cornerPoints = points.Count(p =>
                {
                    var dx = Math.Abs(p.X - centerX);
                    var dy = Math.Abs(p.Y - centerY);

                    return dx > width * 0.35 &&
                           dy > height * 0.35;
                });
                
                var cornerRatio =
                    cornerPoints / (double)points.Count;
                
                return aspectError + (1 - cornerRatio);
            }

            case "equilateraltriangle":
            {
                var ratio =
                    (double)height / width;


                return Math.Abs(ratio - 0.866);
            }


            default:
                return double.MaxValue;
        }
    }
    
    private static double Distance(
        double x1,
        double y1,
        double x2,
        double y2)
    {
        return Math.Sqrt(
            Math.Pow(x1 - x2, 2) +
            Math.Pow(y1 - y2, 2));
    }
    
    private record Point(int X, int Y);
}