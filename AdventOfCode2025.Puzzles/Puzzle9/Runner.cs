using AdventOfCode2025.Puzzles.Utils;
using FluentAssertions;

namespace AdventOfCode2025.Puzzles.Puzzle9;

public class Runner
{
    [Test]
    [Arguments("TestInput", 50)]
    [Arguments("Input", 4744899849L)]
    public void RunAlpha(string filename, long expected)
    {
        var actual = Execute(filename);
        actual.Should().Be(expected);
    }

    //[Test]
    //[Arguments("TestInput", 40)]
    //[Arguments("Input", 10733529153890L)]
    //public void RunBeta(string filename, long expected)
    //{
    //}

    private static long Execute(string filename)
    {
        var lines = EmbeddedResourceReader.Read<Runner>(filename);

        var junctionBoxes = lines.Select(Tile.Create).ToArray();
        var permutations = Permutate(junctionBoxes);
        var largestRectangle = permutations
            .OrderByDescending(x => x.Surface)
            .First();
        return largestRectangle.Surface;
    }

    private static List<Rectangle> Permutate(Tile[] input)
    {
        var count = input.Length;
        var items = new List<Rectangle>();
        for (var i = 0; i < count - 1; i++)
        {
            for (var j = i + 1; j < count; j++)
            {
                items.Add(new Rectangle(input[i], input[j]));
            }
        }

        return items;
    }


    private sealed record Tile(long X, long Y)
    {
        public static Tile Create(string line)
        {
            var numbers = line.Split(',').Select(long.Parse).ToArray();
            return new(numbers[0], numbers[1]);
        }
    }


    private sealed record Rectangle(Tile Left, Tile Right)
    {
        public long Surface { get; } =
                (1 + Math.Abs(Left.X - Right.X)) *
                (1 + Math.Abs(Left.Y - Right.Y));
    }
}