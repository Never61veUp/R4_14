using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApp.Solvers;

public class SteganographySolver
{
    private static readonly Dictionary<string, int> Elements =
        new[]
        {
            "H","He","Li","Be","B","C","N","O","F","Ne",
            "Na","Mg","Al","Si","P","S","Cl","Ar","K","Ca",
            "Sc","Ti","V","Cr","Mn","Fe","Co","Ni","Cu","Zn",
            "Ga","Ge","As","Se","Br","Kr","Rb","Sr","Y","Zr",
            "Nb","Mo","Tc","Ru","Rh","Pd","Ag","Cd","In","Sn",
            "Sb","Te","I","Xe","Cs","Ba","La","Ce","Pr","Nd",
            "Pm","Sm","Eu","Gd","Tb","Dy","Ho","Er","Tm","Yb",
            "Lu","Hf","Ta","W","Re","Os","Ir","Pt","Au","Hg",
            "Tl","Pb","Bi","Po","At","Rn","Fr","Ra","Ac","Th",
            "Pa","U","Np","Pu","Am","Cm","Bk","Cf","Es","Fm",
            "Md","No","Lr","Rf","Db","Sg","Bh","Hs","Mt","Ds",
            "Rg","Cn","Nh","Fl","Mc","Lv","Ts","Og"
        }
        .Select((x, i) => new { x, n = i + 1 })
        .ToDictionary(x => x.x, x => x.n);
    
    public static string Solve(string question)
    {
        var text = question.Trim();
        
        if (Regex.IsMatch(text, @"^\[[A-Z][a-z]?\]"))
        {
            var indexes = Regex.Matches(text, @"\[([A-Z][a-z]?)\]")
                .Cast<Match>()
                .Select(x =>
                {
                    var symbol = x.Groups[1].Value;

                    if (!Elements.TryGetValue(symbol, out var value))
                        throw new Exception($"Unknown element {symbol}");

                    return value;
                })
                .ToArray();

            var hiddenText = Regex.Replace(text, @"^(\[[A-Z][a-z]?\]\s*)+", "")
                .Trim();

            return Extract(hiddenText, indexes);
        }
        
        var romanMatch = Regex.Match(
            text,
            @"^(?<nums>(?:[IVXLCDM]+\s*)+)(?<text>.*)$",
            RegexOptions.Singleline
        );

        if (romanMatch.Success)
        {
            var nums = romanMatch.Groups["nums"]
                .Value
                .Split((char[])null, StringSplitOptions.RemoveEmptyEntries)
                .Select(RomanToInt)
                .ToArray();

            var hiddenText = romanMatch.Groups["text"]
                .Value
                .Trim();

            return Extract(hiddenText, nums);
        }
        
        var lines = text.Split('\n');

        if (lines.Length >= 2 &&
            Regex.IsMatch(lines[0], @"^\d+(\s+\d+)*$"))
        {
            var indexes = lines[0]
                .Split(' ')
                .Select(int.Parse)
                .ToArray();

            return Extract(
                string.Join("\n", lines.Skip(1)),
                indexes
            );
        }
        
        throw new Exception("Invalid steganography format");
    }
    
    private static string Extract(string text, int[] indexes)
    {
        return new string(
            indexes.Select(i =>
            {
                if (i <= 0 || i > text.Length)
                    throw new Exception($"Index {i} out of range");

                return text[i - 1];
            })
            .ToArray()
        );
    }

    private static int RomanToInt(string roman)
    {
        var map = new Dictionary<char, int>
        {
            ['I']=1,
            ['V']=5,
            ['X']=10,
            ['L']=50,
            ['C']=100,
            ['D']=500,
            ['M']=1000
        };

        var result = 0;

        for (var i = 0; i < roman.Length; i++)
        {
            var value = map[roman[i]];

            if (i + 1 < roman.Length &&
                value < map[roman[i + 1]])
                result -= value;
            else
                result += value;
        }

        return result;
    }
}