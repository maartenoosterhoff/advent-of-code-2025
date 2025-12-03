using AdventOfCode2025.Puzzles.Utils;
using FluentAssertions;

namespace AdventOfCode2025.Puzzles.Puzzle3;

public class Runner
{
    [Test]
    [Arguments("TestInput", 357)]
    [Arguments("Input", 16854)]
    public void RunAlpha(string filename, int expected)
    {
        var actual = Execute(filename, bank => Finder(bank, 2));
        actual.Should().Be(expected);
    }

    [Test]
    [Arguments("TestInput", 3121910778619L)]
    [Arguments("Input", 167526011932478L)]
    public void RunBeta(string filename, long expected)
    {
        var actual = Execute(filename, bank => Finder(bank, 12));
        actual.Should().Be(expected);
    }

    static long Finder(string bank, int max)
    {
        const string byMax = "987654321";

        var curr = 0;
        var chars = new List<char>();
        var pos = 0;
        while (curr < max)
        {
            foreach (var m in byMax)
            {
                var newPos = bank.Substring(0, bank.Length - (max - curr) + 1).IndexOf(m, pos);
                if (newPos >= 0)
                {
                    chars.Add(m);
                    pos = newPos + 1;
                    break;
                }
            }
            curr++;
        }

        if (chars.Count == max)
        {
            return long.Parse(new string(chars.ToArray()));
        }
        return 0L;
    }

    private static long Execute(string filename, Func<string, long> finder)
    {
        var lines = EmbeddedResourceReader.Read<Runner>(filename);

        var total = 0L;

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            total += finder(line);

        }

        return total;
    }
}