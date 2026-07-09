using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Challenge.DataContracts;

namespace ConsoleApp;

public class Solver
{
    private static readonly Dictionary<string, Func<string, string>> Solvers = new()
    {
        { "steganography", SolveSteganography }
    };

    public static string Solve(TaskResponse taskResponse)
    {
        if (Solvers.TryGetValue(taskResponse.TypeId, out var solver))
            return solver(taskResponse.Question);

        throw new ArgumentException($"Unknown task type: {taskResponse.TypeId}");
    }

    private static string SolveSteganography(string question)
    {
        if (string.IsNullOrWhiteSpace(question))
            return "";

        question = question.Trim();

        // === Новая стеганография с формулой (Cypher-вариант) ===
        if (question.Contains("multiplicator") || (question.Contains('#') && question.Contains("formula")))
        {
            return SolveCypher(question);
        }

        // === Классическая стеганография (индексы арабские/римские + текст книги) ===
        return SolveClassicSteganography(question);
    }

    private static string SolveCypher(string question)
    {
        var parts = question.Split('#');
        if (parts.Length < 3) return "error_invalid_format";

        string paramsStr = parts[1];      
        string encryptedText = parts[2];  

        int multIndex = paramsStr.IndexOf("multiplicator=");
        if (multIndex == -1) return "error_no_mult";

        var multStr = new string(paramsStr.Substring(multIndex + "multiplicator=".Length)
            .TakeWhile(char.IsDigit)
            .ToArray());

        if (!long.TryParse(multStr, out long multiplicator))
            return "error_invalid_mult";

        int bookStartIndex = question.IndexOf(encryptedText) + encryptedText.Length;
        if (bookStartIndex >= question.Length) return "error_no_book";

        string bookText = question.Substring(bookStartIndex).TrimStart('\r', '\n');
        long abcLength = bookText.Length; 

        var decryptedChars = new char[encryptedText.Length];

        for (int charIndex = 0; charIndex < encryptedText.Length; charIndex++)
        {
            long targetIndex = (multiplicator * (charIndex + 1L)) % (abcLength + 1) - 1;

            if (targetIndex >= 0 && targetIndex < bookText.Length)
            {
                decryptedChars[charIndex] = bookText[(int)targetIndex];
            }
            else
            {
                decryptedChars[charIndex] = ' ';
            }
        }

        var result = new string(decryptedChars);
        Console.WriteLine($"[Cypher/Stego] Decoded: {result}");
        return result;
    }

    private static string SolveClassicSteganography(string question)
    {
        var lines = question.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        int[] indexes = null;
        int textStartIndex = -1;

        var romanRegex = new Regex(@"^[IVXLCDM]+$", RegexOptions.IgnoreCase);

        for (int i = 0; i < lines.Length; i++)
        {
            var trimmed = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;

            var parts = trimmed.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 2 && parts.All(p => int.TryParse(p, out _) || romanRegex.IsMatch(p)))
            {
                indexes = parts.Select(p =>
                {
                    if (int.TryParse(p, out int arabic)) return arabic;
                    return RomanToArabic(p.ToUpper());
                }).ToArray();

                textStartIndex = i + 1;
                break;
            }
        }

        if (indexes == null || textStartIndex == -1 || textStartIndex >= lines.Length)
            return "error_no_data";

        // Соединяем строки книги через родной перенос строки (\n), чтобы не сдвигать индексы
        string fullText = string.Join("\n", lines.Skip(textStartIndex));

        Console.WriteLine($"[DEBUG] Text length: {fullText.Length}");

        var result = new System.Text.StringBuilder();
        foreach (int idx in indexes)
        {
            if (idx >= 0 && idx < fullText.Length)
                result.Append(fullText[idx]);
            else
                result.Append('?');
        }

        string answer = result.ToString();
        Console.WriteLine($"[Classic Stego] Extracted: '{answer}'");
        return answer;
    }

    private static int RomanToArabic(string roman)
    {
        var romanMap = new Dictionary<char, int> { { 'I', 1 }, { 'V', 5 }, { 'X', 10 }, { 'L', 50 }, { 'C', 100 }, { 'D', 500 }, { 'M', 1000 } };
        int total = 0, idx = 0;
        while (idx < roman.Length)
        {
            int s1 = romanMap[roman[idx]];
            if (idx + 1 < roman.Length)
            {
                int s2 = romanMap[roman[idx + 1]];
                if (s1 >= s2) { total += s1; idx++; }
                else { total += (s2 - s1); idx += 2; }
            }
            else { total += s1; idx++; }
        }
        return total;
    }
}