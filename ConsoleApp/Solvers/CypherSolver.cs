using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp.Solvers;

public class CypherSolver
{
    public static string Solve(string question)
    {
        var parts = question.Split('#');
        if (parts.Length < 3)
            throw new Exception("Bad cipher format");

        var hint = parts[1];
        var message = parts[2];
        
        if (hint.StartsWith("reversed"))
            return new string(message.Reverse().ToArray());
        if (hint.StartsWith("prime multiplicator"))
            return SolvePrimeMultiplicator(hint, message);
        
        var caesar = Regex.Match(
            hint,
            @"Caesar's code=([+-]?\d+)",
            RegexOptions.IgnoreCase
        );

        if (caesar.Success)
        {
            var shift = int.Parse(caesar.Groups[1].Value);

            return CaesarDecode(message, shift);
        }
        
        throw new Exception($"Unknown cipher {hint}");
    }
    
    private static string CaesarDecode(string text, int shift)
    {
        shift %= 26;

        var result = new StringBuilder();

        foreach (var ch in text)
        {
            if (ch >= 'a' && ch <= 'z')
            {
                var index = ch - 'a';

                index -= shift;

                if (index < 0)
                    index += 26;

                result.Append((char)('a' + index));
            }
            else
                result.Append(ch);
        }

        return result.ToString();
    }
    
    private static string SolvePrimeMultiplicator(string hint, string message)
    {
        const string alphabet = "abcdefghijklmnopqrstuvwxyz0123456789' ";

        var match = Regex.Match(
            hint,
            @"prime multiplicator=(\d+)"
        );

        if (!match.Success)
            throw new Exception("Bad prime multiplicator format");

        var multiplier = int.Parse(match.Groups[1].Value);
        var mod = alphabet.Length + 1;
        var result = new StringBuilder();

        foreach (var encryptedChar in message)
        {
            var encryptedIndex = alphabet.IndexOf(encryptedChar);
            if (encryptedIndex == -1)
                throw new Exception($"Unknown char: {encryptedChar}");

            var originalIndex = -1;

            for (var charIndex = 0; charIndex < alphabet.Length; charIndex++)
            {
                var newIndex =
                    (multiplier * (charIndex + 1)) % mod - 1;

                if (newIndex == encryptedIndex)
                {
                    originalIndex = charIndex;
                    break;
                }
            }

            if (originalIndex == -1)
                throw new Exception($"Cannot decode char: {encryptedChar}");

            result.Append(alphabet[originalIndex]);
        }

        return result.ToString();
    }
}