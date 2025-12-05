using AdventOfCode2025.Puzzles.Utils;
using FluentAssertions;

namespace AdventOfCode2025.Puzzles.Puzzle1;

public class Runner
{
    [Test]
    [Arguments("TestInput", 3)]
    [Arguments("Input", 1147)]
    public void RunAlpha(string filename, int expected)
    {
        var actual = Execute(filename, Agg);
        actual.Should().Be(expected);
        return;

        static (int, int) Agg(int pos, LineInfo li)
        {
            var newPos = pos + li.Sign * li.Value;
            newPos %= 100;
            return (newPos, newPos == 0 ? 1 : 0);
        }
    }

    [Test]
    [Arguments("TestInput", 6)]
    [Arguments("Input", 6789)]
    public void RunBeta(string filename, int expected)
    {
        var actual = Execute(filename, Agg);
        actual.Should().Be(expected);
        return;

        static (int, int) Agg(int pos, LineInfo li)
        {
            var value = li.Value;
            var count = 0;
            while (value > 0)
            {
                pos += li.Sign;
                pos %= 100;
                if (pos == 0)
                {
                    count++;
                }
                value--;
            }

            return (pos, count);
        }
    }

    private static int Execute(string filename, Func<int, LineInfo, (int pos, int count)> agg)
    {
        var lines = EmbeddedResourceReader.Read<Runner>(filename);

        var pos = 50;
        var count = 0;

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            LineInfo li = line[0] switch
            {
                'L' => new(-1, int.Parse(line[1..])),
                'R' => new(1, int.Parse(line[1..])),
                _ => throw new NotImplementedException()
            };

            (pos, var countAdd) = agg(pos, li);
            count += countAdd;
        }

        return count;

    }
    private record LineInfo(int Sign, int Value);
}