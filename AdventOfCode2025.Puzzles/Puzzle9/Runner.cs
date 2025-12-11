using AdventOfCode2025.Puzzles.Utils;
using FluentAssertions;
using System.Text;

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

    [Test]
    [Arguments("TestInput", 24)]
    [Arguments("Input", 0)]
    public void RunBeta(string filename, long expected)
    {
        var lines = EmbeddedResourceReader.Read<Runner>(filename);

        var tiles = lines.Select(Tile.Create).ToArray();
        var xMin = tiles.Min(t => t.X);
        var yMin = tiles.Min(t => t.Y);
        var xmax = tiles.Max(t => t.X);
        var ymax = tiles.Max(t => t.Y);

        var map = new Map(xmax, ymax, xMin, yMin);

        for (var i = 0; i < tiles.Length; i++)
        {
            map.CreateLine(tiles[i], tiles[i == 0 ? tiles.Length - 1 : i - 1]);
        }

        map.PaintInside();

        var permutations = Permutate(tiles);
        var largestRectangle = permutations
            .OrderByDescending(x => x.Surface)
            .First(x => map.UsesOnlyRedAndGreenTiles(x));

        var actual = largestRectangle.Surface;
        actual.Should().Be(expected);

    }

    private class Map
    {
        private readonly long _xMax;
        private readonly long _yMax;
        private readonly long _xMin;
        private readonly long _yMin;

        private readonly string[] _map;

        public Map(long xMax, long yMax, long xMin, long yMin)
        {
            _xMax = xMax + 2;
            _yMax = yMax + 2;
            _xMin = xMin - 1;
            _yMin = yMin - 1;

            _map = Enumerable.Range(0, (int)(_xMax - _xMin)).Select(_ => new string('.', (int)(_yMax - _yMin))).ToArray();
        }

        public void MarkRed(long x, long y)
        {
            SetCell(x, y, '#');
        }

        private void SetCell(long x, long y, char c, bool force = false)
        {
            if (!force && GetCell(x, y) != '.')
            {
                return;
            }

            x -= _xMin;
            y -= _yMin;

            SetCellIntern(x, y, c);
            //_map[x, y] = c;
        }

        private void SetCellIntern(long x, long y, char c)
        {
            var line = _map[x];
            if (y == 0)
            {
                line = $"{c}{line[1..]}";
            }
            else if (y == line.Length - 1)
            {
                line = $"{line[..^1]}{c}";
            }
            else
            {
                line = $"{line[..(int)y]}{c}{line[(int)(y+1)..]}";
            }

            _map[x] = line;
        }

        private char GetCell(long x, long y)
        {
            x -= _xMin;
            y -= _yMin;

            return _map[x][(int)y];

        }

        public void MarkGreen(long x, long y)
        {
            SetCell(x, y, 'X');
        }

        public void CreateLine(Tile a, Tile b)
        {
            if (a.X == b.X)
            {
                long y1 = a.Y, y2 = b.Y;
                if (y1 > y2)
                {
                    (y1, y2) = (y2, y1);
                }
                MarkRed(a.X, a.Y);
                MarkRed(b.X, b.Y);
                for (var y = y1; y <= y2; y++)
                {
                    MarkGreen(a.X, y);
                }
            }
            else if (a.Y == b.Y)
            {
                long x1 = a.X, x2 = b.X;
                if (x1 > x2)
                {
                    (x1, x2) = (x2, x1);
                }
                MarkRed(a.X, a.Y);
                MarkRed(b.X, b.Y);
                for (var x = x1; x <= x2; x++)
                {
                    MarkGreen(x, a.Y);
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public override string ToString()
        {
            // Note: x and y are reversed

            StringBuilder sb = new();
            var yLen = _map[0].Length;
            for (var y = 0; y < yLen; y++)
            {
                for (var x = 0; x < _map.Length; x++)
                {
                    sb.Append(_map[x][y]);
                }

                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        public void PaintInside()
        {
            var pointsToDo = new Queue<(long x, long y)>();
            pointsToDo.Enqueue((_xMin, _yMin));

            while (pointsToDo.Count > 0)
            {
                var point = pointsToDo.Dequeue();
                SetCell(point.x, point.y, '_', true);

                var newPoints = new List<(long x, long y)>
                {
                    (point.x - 1, point.y),
                    (point.x + 1, point.y),
                    (point.x, point.y - 1),
                    (point.x, point.y + 1)
                };

                foreach (var newPoint in newPoints)
                {
                    if (IsPointValid(newPoint.x, newPoint.y) &&
                        GetCell(newPoint.x, newPoint.y) == '.')
                    {
                        pointsToDo.Enqueue(newPoint);
                    }
                }
            }

            var yLen = _map[0].Length;
            for (var y = 0; y < yLen; y++)
            {
                for (var x = 0; x < _map.Length; x++)
                {
                    //        for (var x = 0; x <= _map.GetUpperBound(0); x++)
                    //{
                    //    for (var y = 0; y <= _map.GetUpperBound(1); y++)
                    //    {
                    if (_map[x][y] == '.')
                    {
                        SetCellIntern(x, y, 'X');
                        //_map[x][y] = 'X';
                    }
                    if (_map[x][y] == '_')
                    {
                        SetCellIntern(x, y, '.');
                        //_map[][y] = '.';
                    }
                }
            }
        }

        private bool IsPointValid(long x, long y)
        {
            return x >= _xMin &&
                   x < _xMax &&
                   y >= _yMin &&
                   y < _yMax;
        }

        public bool UsesOnlyRedAndGreenTiles(Rectangle rectangle)
        {
            var x1 = rectangle.Left.X;
            var y1 = rectangle.Left.Y;
            var x2 = rectangle.Right.X;
            var y2 = rectangle.Right.Y;
            if (x1 > x2)
            {
                (x1, x2) = (x2, x1);
            }

            if (y1 > y2)
            {
                (y1, y2) = (y2, y1);
            }

            for (var x = x1; x <= x2; x++)
            {
                for (var y = y1; y <= y2; y++)
                {
                    var c = GetCell(x, y);
                    if (c is not 'X' and not '#')
                    {
                        return false;
                    }
                }

            }

            return true;
        }
    }

    private static long Execute(string filename)
    {
        var lines = EmbeddedResourceReader.Read<Runner>(filename);

        var tiles = lines.Select(Tile.Create).ToArray();
        var permutations = Permutate(tiles);
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