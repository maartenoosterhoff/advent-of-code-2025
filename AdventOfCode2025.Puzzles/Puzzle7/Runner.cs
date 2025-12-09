using AdventOfCode2025.Puzzles.Utils;
using FluentAssertions;

namespace AdventOfCode2025.Puzzles.Puzzle7;

public class Runner
{
    [Test]
    [Arguments("TestInput", 21)]
    [Arguments("Input", 1533)]
    public void RunAlpha(string filename, int expected)
    {
        var actual = Execute(
            filename,
            (string line, HashSet<int> points, int count) =>
            {
                var newPoints = new HashSet<int>();
                foreach (var point in points)
                {
                    if (line[point] == '^')
                    {
                        newPoints.Add(point - 1);
                        newPoints.Add(point + 1);
                        count++;
                    }
                    else
                    {
                        newPoints.Add(point);
                    }
                }

                return (newPoints, count);
            });
        actual.Should().Be(expected);
    }

    [Test]
    [Arguments("TestInput", 40)]
    [Arguments("Input", 0)]
    public void RunBeta(string filename, int expected)
    {
        //var actual = Execute(filename);
        //actual.Should().Be(expected);
    }

    private static int Execute(string filename, Func<string, HashSet<int>, int, (HashSet<int> newPoints, int newCount)> mapper)
    {
        var lines = EmbeddedResourceReader.Read<Runner>(filename);

        var points = new HashSet<int>();

        var startPos = -1;
        var count = 0;
        foreach (var line in lines)
        {
            if (startPos == -1)
            {
                startPos = lines[0].IndexOf('S');
                if (startPos < 0)
                {
                    throw new InvalidOperationException();
                }

                points.Add(startPos);

                continue;
            }

            (points, count) = mapper(line, points, count);
        }
        return count;
    }
}