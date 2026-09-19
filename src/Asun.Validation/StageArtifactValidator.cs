using System.Text;
using System.Text.RegularExpressions;

namespace Asun.Validation;

public static class StageArtifactValidator
{
    private static readonly Regex LoopPattern =
        new(@"\bfor\s*\(", RegexOptions.Compiled);

    private static readonly Regex CheckPattern =
        new(@"\bCheck\s*\(", RegexOptions.Compiled);

    private static readonly Regex CheckDefinitionPattern =
        new(@"\b(?:void|static\s+void)\s+Check\s*\(", RegexOptions.Compiled);

    private static readonly Regex PlaceholderPattern =
        new(@"\b(?:TODO|NotImplementedException)\b", RegexOptions.Compiled);

    private static readonly Regex LedgerPattern =
        new(@"^-\s*\[x\]\s+(\d+)\.", RegexOptions.Compiled | RegexOptions.Multiline);

    public static StageArtifactReport ValidateSmoke(
        string source,
        int expectedRounds = 100,
        int expectedLoopGroups = 10)
    {
        ArgumentNullException.ThrowIfNull(source);

        var errors = new List<string>();
        var sanitized = StripCommentsAndStrings(source);

        if (expectedRounds <= 0 || expectedRounds % 10 != 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedRounds),
                "Expected rounds must be a positive multiple of 10.");
        }

        var loops = LoopPattern.Matches(sanitized).Count;
        var totalChecks = CheckPattern.Matches(sanitized).Count;
        var checkDefinitions = CheckDefinitionPattern.Matches(sanitized).Count;
        var checks = Math.Max(0, totalChecks - checkDefinitions);
        var roundPattern =
            new Regex(
                $@"\bround\s*==\s*{Regex.Escape(expectedRounds.ToString())}\b",
                RegexOptions.Compiled);

        var rounds = roundPattern.Matches(sanitized).Count;
        var balanced = HasBalancedDelimiters(sanitized);
        var placeholders = PlaceholderPattern.IsMatch(sanitized);

        if (loops != expectedLoopGroups)
            errors.Add($"Expected {expectedLoopGroups} loop groups but found {loops}.");

        if (checks != expectedRounds / 10)
            errors.Add(
                $"Expected {expectedRounds / 10} numbered Check call sites but found {checks}.");

        if (rounds != 1)
            errors.Add($"Expected one round == {expectedRounds} assertion but found {rounds}.");

        if (!balanced)
            errors.Add("Source delimiters are not balanced.");

        if (placeholders)
            errors.Add("Placeholder markers are present.");

        return new StageArtifactReport(
            errors.Count == 0,
            loops,
            checks,
            rounds,
            balanced,
            placeholders,
            errors);
    }

    public static StageArtifactReport ValidateLedger(
        string source,
        int firstStage,
        int lastStage)
    {
        ArgumentNullException.ThrowIfNull(source);

        var errors = new List<string>();
        var numbers = LedgerPattern.Matches(source)
            .Select(match => int.Parse(match.Groups[1].Value))
            .ToArray();

        var expectedCount = lastStage - firstStage + 1;

        if (numbers.Length != expectedCount)
        {
            errors.Add(
                $"Expected {expectedCount} ledger entries but found {numbers.Length}.");
        }

        for (var index = 0; index < numbers.Length; index++)
        {
            var expected = firstStage + index;

            if (numbers[index] != expected)
            {
                errors.Add(
                    $"Ledger entry {index + 1} expected stage {expected} but found {numbers[index]}.");
            }
        }

        if (numbers.Distinct().Count() != numbers.Length)
            errors.Add("Ledger contains duplicate stage numbers.");

        return new StageArtifactReport(
            errors.Count == 0,
            0,
            0,
            0,
            true,
            false,
            errors);
    }

    private static bool HasBalancedDelimiters(string source)
    {
        var stack = new Stack<char>();
        var pairs = new Dictionary<char, char>
        {
            ['}'] = '{',
            [']'] = '[',
            [')'] = '('
        };

        foreach (var character in source)
        {
            if (character is '{' or '[' or '(')
            {
                stack.Push(character);
                continue;
            }

            if (character is not '}' and not ']' and not ')')
                continue;

            if (stack.Count == 0 || stack.Pop() != pairs[character])
                return false;
        }

        return stack.Count == 0;
    }

    private static string StripCommentsAndStrings(string source)
    {
        var builder = new StringBuilder(source.Length);
        var inLineComment = false;
        var inBlockComment = false;
        var inString = false;
        var inChar = false;
        var verbatim = false;
        var escaped = false;

        for (var index = 0; index < source.Length; index++)
        {
            var current = source[index];
            var next = index + 1 < source.Length ? source[index + 1] : '\0';

            if (inLineComment)
            {
                if (current == '\n')
                {
                    inLineComment = false;
                    builder.Append('\n');
                }
                else
                {
                    builder.Append(' ');
                }

                continue;
            }

            if (inBlockComment)
            {
                if (current == '*' && next == '/')
                {
                    inBlockComment = false;
                    builder.Append("  ");
                    index++;
                }
                else
                {
                    builder.Append(current == '\n' ? '\n' : ' ');
                }

                continue;
            }

            if (inString)
            {
                if (verbatim && current == '"' && next == '"')
                {
                    builder.Append("  ");
                    index++;
                    continue;
                }

                if (!verbatim && !escaped && current == '"')
                    inString = false;

                if (!verbatim)
                    escaped = !escaped && current == '\\';
                else
                    escaped = false;

                builder.Append(current == '\n' ? '\n' : ' ');
                continue;
            }

            if (inChar)
            {
                if (!escaped && current == '\'')
                    inChar = false;

                escaped = !escaped && current == '\\';
                builder.Append(current == '\n' ? '\n' : ' ');
                continue;
            }

            if (current == '/' && next == '/')
            {
                inLineComment = true;
                builder.Append("  ");
                index++;
                continue;
            }

            if (current == '/' && next == '*')
            {
                inBlockComment = true;
                builder.Append("  ");
                index++;
                continue;
            }

            if (current == '"' || (current == '@' && next == '"'))
            {
                inString = true;
                verbatim = current == '@';
                if (verbatim)
                    index++;
                builder.Append(' ');
                continue;
            }

            if (current == '\'')
            {
                inChar = true;
                builder.Append(' ');
                continue;
            }

            escaped = false;
            builder.Append(current);
        }

        return builder.ToString();
    }
}
