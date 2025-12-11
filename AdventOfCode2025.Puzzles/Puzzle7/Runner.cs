using AdventOfCode2025.Puzzles.Utils;
using FluentAssertions;
using System.Collections.Concurrent;

namespace AdventOfCode2025.Puzzles.Puzzle7;

public class Runner
{
    [Test]
    [Arguments("TestInput", 21)]
    [Arguments("Input", 1533)]
    public void RunAlpha(string filename, long expected)
    {
        var actual = Execute(
            filename,
            (string line, ConcurrentDictionary<int, long> points, long count) =>
            {
                var newPoints = new ConcurrentDictionary<int, long>();
                foreach (var kvp in points)
                {
                    var point = kvp.Key;
                    if (line[point] == '^')
                    {
                        newPoints.TryAdd(point - 1, 0);
                        newPoints.TryAdd(point + 1, 0);
                        count++;
                    }
                    else
                    {
                        newPoints.TryAdd(point, 0);
                    }
                }

                return (newPoints, count);
            });
        actual.Should().Be(expected);
    }

    [Test]
    [Arguments("TestInput", 40)]
    [Arguments("Input", 10733529153890L)]
    public void RunBeta(string filename, long expected)
    {
        var actual = Execute(
            filename,
            (string line, ConcurrentDictionary<int, long> points, long count) =>
            {
                var newPoints = new ConcurrentDictionary<int, long>();
                foreach (var kvp in points)
                {
                    var point = kvp.Key;
                    if (line[point] == '^')
                    {
                        newPoints.AddOrUpdate(point - 1, kvp.Value, (_, v) => v + kvp.Value);
                        newPoints.AddOrUpdate(point + 1, kvp.Value, (_, v) => v + kvp.Value);
                    }
                    else
                    {
                        newPoints.AddOrUpdate(point, kvp.Value, (_, v) => v + kvp.Value);
                    }
                }
                count = newPoints.Sum(kvp => kvp.Value);
                return (newPoints, count);
            });
        actual.Should().Be(expected);
    }

    private static long Execute(string filename, Func<string, ConcurrentDictionary<int, long>, long, (ConcurrentDictionary<int, long> newPoints, long newCount)> mapper)
    {
        var lines = EmbeddedResourceReader.Read<Runner>(filename);

        var points = new ConcurrentDictionary<int, long>();

        var startPos = -1;
        long count = 0;
        foreach (var line in lines)
        {
            if (startPos == -1)
            {
                startPos = lines[0].IndexOf('S');
                if (startPos < 0)
                {
                    throw new InvalidOperationException();
                }

                points.TryAdd(startPos, 1);

                continue;
            }

            (points, count) = mapper(line, points, count);
        }

        return count;
    }
}