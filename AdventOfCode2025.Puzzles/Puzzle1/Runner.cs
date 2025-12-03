using AdventOfCode2025.Puzzles.Utils;
using FluentAssertions;
using Xunit;

namespace AdventOfCode2025.Puzzles.Puzzle1;

public class Runner
{
    [Theory]
    [InlineData("TestInput", 3)]
    [InlineData("Input", 1147)]
    public void RunAlpha(string filename, int expected)
    {
        var actual = Execute(filename);
        actual.Should().Be(expected);
    }

    //[Theory]
    //[InlineData("TestInput", 31)]
    //[InlineData("Input", 23177084)]
    //public void RunBeta(string filename, int expected)
    //{
    //    var (points1, points2) = Execute(filename);
    //    var map = points2.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

    //    var sum = 0;

    //    foreach (var point1 in points1)
    //    {
    //        if (map.TryGetValue(point1, out var scoreIncreaser))
    //        {
    //            sum += point1 * scoreIncreaser;
    //        }
    //    }

    //    sum.Should().Be(expected);
    //}

    private static int Execute(string filename)
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

            if (line[0] == 'L')
            {
                pos -= int.Parse(line.Substring(1));
            }
            else if (line[0] == 'R')
            {
                pos += int.Parse(line.Substring(1));
            } else
            {
                throw new NotImplementedException();
            }

                pos = pos % 100;

            if (pos == 0)
            {
                count++;
            }
        }

        return count;

    }
}