using AdventOfCode2025.Puzzles.Utils;
using FluentAssertions;
using TUnit.Assertions.Conditions;

namespace AdventOfCode2025.Puzzles.Puzzle4;

public class Runner
{
    [Test]
    [Arguments("TestInput", 13)]
    [Arguments("Input", 1424)]
    public void RunAlpha(string filename, int expected)
    {
        var grid = Execute(filename);

        var actual = 0;
        for (var x = 0; x < grid.XMax; x++)
        {
            for (var y = 0; y < grid.XMax; y++)
            {
                if (grid.IsOccupied(x, y) == 1 && grid.CountAdjacentIsOccupied(x, y) < 4)
                {
                    actual++;
                }
            }
        }

        actual.Should().Be(expected);
    }

    [Test]
    [Arguments("TestInput", 43)]
    [Arguments("Input", 8727)]
    public void RunBeta(string filename, int expected)
    {
        var grid = Execute(filename);

        var actual = 0;
        var removed = int.MaxValue;

        while (removed > 0)
        {
            removed = 0;
            for (var x = 0; x < grid.XMax; x++)
            {
                for (var y = 0; y < grid.XMax; y++)
                {
                    if (grid.IsOccupied(x, y) == 1 && grid.CountAdjacentIsOccupied(x, y) < 4)
                    {
                        removed++;
                        grid.SetAsRemoved(x, y);
                    }
                }
            }

            actual += removed;
        }

        actual.Should().Be(expected);
    }

    private static Grid Execute(string filename)
    {
        var lines = EmbeddedResourceReader.Read<Runner>(filename);

        var grid = new Grid([.. lines.Where(line => !string.IsNullOrEmpty(line))]);

        return grid;
    }

    private sealed class Grid
    {
        private readonly string[] _data;

        public int XMax { get; }
        public int YMax { get; }

        public Grid(string[] data)
        {
            if (data.Any(string.IsNullOrEmpty))
            {
                throw new InvalidOperationException();
            }

            YMax = data.Length;
            XMax = data[0].Length;

            if (data.Any(x => x.Length != XMax))
            {
                throw new InvalidOperationException();
            }

            _data = data;
        }


        public int IsOccupied(int x, int y)
        {
            if (x < 0 || y < 0 ||
                y >= _data.Length ||
                x >= _data[y].Length)
            {
                return 0;
            }

            return _data[y][x] == '@' ? 1 : 0;
        }

        public int CountAdjacentIsOccupied(int x, int y)
        {
            return
                IsOccupied(x - 1, y - 1) +
                IsOccupied(x - 1, y) +
                IsOccupied(x - 1, y + 1) +
                IsOccupied(x, y - 1) +
                IsOccupied(x, y + 1) +
                IsOccupied(x + 1, y - 1) +
                IsOccupied(x + 1, y) +
                IsOccupied(x + 1, y + 1);
        }

        public void SetAsRemoved(int x, int y)
        {
            var line = _data[y].ToArray();
            line[x] = '.';
            _data[y] = new string(line);
        }
    }
}