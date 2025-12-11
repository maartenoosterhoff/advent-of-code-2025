using AdventOfCode2025.Puzzles.Utils;
using FluentAssertions;
using TUnit.Generated;

namespace AdventOfCode2025.Puzzles.Puzzle6;

public class Runner
{
    [Test]
    [Arguments("TestInput", 4277556)]
    [Arguments("Input", 4951502530386L)]
    public void RunAlpha(string filename, long expected)
    {
        var actual = Execute(
            filename,
            (data, @operator) =>
            {
                return @operator switch
                {
                    '+' => data.Select(long.Parse).Aggregate((total, next) => next + total),
                    '*' => data.Select(long.Parse).Aggregate((total, next) => next * total),
                    _ => throw new InvalidOperationException()
                };
            });
        actual.Should().Be(expected);
    }

    [Test]
    [Arguments("TestInput", 3263827)]
    [Arguments("Input", 0)]
    public void RunBeta(string filename, long expected)
    {
        var actual = Execute2(
            filename,
            (data, @operator) =>
            {
                return @operator switch
                {
                    '+' => data.Select(long.Parse).Aggregate((total, next) => next + total),
                    '*' => data.Select(long.Parse).Aggregate((total, next) => next * total),
                    _ => throw new InvalidOperationException()
                };
            });
        actual.Should().Be(expected);
    }

    private static long Execute(string filename, Func<List<string>, char, long> operation)
    {
        var lines = EmbeddedResourceReader.Read<Runner>(filename);

        var dataSplit = lines.Select(x => x.Replace("  ", " ").Split([" "], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)).ToArray();
        var rowCount = dataSplit.Length;
        var count = dataSplit.First().Length;
        if (dataSplit.Any(x => x.Length != count))
        {
            throw new InvalidOperationException();
        }

        var numbersData = dataSplit.Take(rowCount - 1).ToArray();
        var operatorData = dataSplit[rowCount - 1];
        var total = 0L;
        for (var i = 0; i < count; i++)
        {
            var op = operatorData[i][0];
            var data = numbersData.Select(z => z[i]).ToList();
            total += operation(data, op);
        }

        return total;
    }

    private static long Execute2(string filename, Func<List<string>, char, long> operation)
    {
        var lines = EmbeddedResourceReader.Read<Runner>(filename);

        var dataSplit = lines.Select(x => x.Replace("  ", " ").Split([" "], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)).ToArray();
        var rowCount = dataSplit.Length;
        var count = dataSplit.First().Length;
        if (dataSplit.Any(x => x.Length != count))
        {
            throw new InvalidOperationException();
        }

        var numbersData = dataSplit.Take(rowCount - 1).ToArray();
        var operatorData = dataSplit[rowCount - 1];
        var total = 0L;
        for (var i = 0; i < count; i++)
        {
            var op = operatorData[i][0];
            var data = numbersData.Select(z => z[i]).ToList();
            total += operation(data, op);
        }

        return total;
    }
}