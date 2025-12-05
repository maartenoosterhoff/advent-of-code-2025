using AdventOfCode2025.Puzzles.Utils;
using FluentAssertions;

namespace AdventOfCode2025.Puzzles.Puzzle5;

public class Runner
{
    [Test]
    [Arguments("TestInput", 3)]
    [Arguments("Input", 558)]
    public void RunAlpha(string filename, int expected)
    {
        var (ranges, ingredientIds) = Execute(filename);
        var actual = ingredientIds.Count(ranges.ContainsIngredientId);
        actual.Should().Be(expected);
    }

    [Test]
    [Arguments("TestInput", 14)]
    [Arguments("Input", 344813017450467L)]
    public void RunBeta(string filename, long expected)
    {
        var (ranges, _) = Execute(filename);
        var actual = ranges.FreshIngredientIdCount();
        actual.Should().Be(expected);
    }

    private static (IngredientRanges Ranges, List<long> IngredientIds) Execute(string filename)
    {
        var lines = EmbeddedResourceReader.Read<Runner>(filename);

        var readingRanges = true;
        var ranges = new IngredientRanges();
        var ingredientIds = new List<long>();
        foreach (var line in lines)
        {
            if (readingRanges)
            {
                if (string.IsNullOrEmpty(line))
                {
                    readingRanges = false;
                    continue;
                }

                var values = line.Split('-');
                ranges.AddRange(long.Parse(values[0]), long.Parse(values[1]));

                continue;
            }

            if (string.IsNullOrEmpty(line))
            {
                continue;
            }


            ingredientIds.Add(long.Parse(line));
        }

        return (ranges, ingredientIds);
    }

    private sealed class IngredientRanges
    {
        private readonly List<Range> _ranges = new();

        public void AddRange(long start, long end)
        {
            _ranges.Add(new(start, end));
        }

        public bool ContainsIngredientId(long id)
        {
            return _ranges.Any(x => x.Start <= id && id <= x.End);
        }

        public long FreshIngredientIdCount()
        {
            var foundOverlap = true;
            var data = _ranges.ToList();
            while (foundOverlap)
            {
                foundOverlap = false;
                foreach (var item in data)
                {
                    var others = data.Where(x => x != item);
                    var otherItem = others.FirstOrDefault(x => x.HasOverlapWith(item));
                    if (otherItem is null)
                    {
                        continue;
                    }
                    var combinedItem = Range.Combine(item, otherItem);
                    data.Remove(item);
                    data.Remove(otherItem);
                    data.Add(combinedItem);

                    foundOverlap = true;
                    break;
                }
            }

            return data.Sum(x => x.End - x.Start + 1);
        }
    }

    private sealed class Range(long start, long end)
    {
        public long Start { get; } = start;
        public long End { get; } = end;

        public bool HasOverlapWith(Range range) =>
            range.End >= this.Start &&
            this.End >= range.Start;

        public static Range Combine(Range range1, Range range2)
        {
            if (!range1.HasOverlapWith(range2))
            {
                throw new InvalidOperationException();
            }

            return new Range(
                Math.Min(range1.Start, range2.Start),
                Math.Max(range1.End, range2.End));
        }
    }
}