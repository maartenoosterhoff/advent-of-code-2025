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
    [Arguments("Input", 8486156119946L)]
    public void RunBeta(string filename, long expected)
    {
        var actual = Execute2(filename);
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

    private static long Execute2(string filename)
    {
        var lines = EmbeddedResourceReader.Read<Runner>(filename);
        var len = lines[0].Length;
        var data = Enumerable.Range(0, lines.Length).Select(x => new List<string>()).ToArray();
        var current = Enumerable.Range(0, lines.Length).Select(x => "").ToArray();
        for (var i = 0; i < len; i++)
        {
            if (lines.All(l => l[i] == ' '))
            {
                for (var j = 0; j < lines.Length; j++)
                {
                    data[j].Add(current[j]);
                    current[j] = "";
                }
            }
            else
            {
                for (var j = 0; j < lines.Length; j++)
                {
                    current[j] += lines[j][i];
                }
            }
        }

        if (current[0].Length > 0)
        {
            for (var j = 0; j < lines.Length; j++)
            {
                data[j].Add(current[j]);
                current[j] = "";
            }
        }

        var sum = 0L;
        var nrsCount = data.Length - 1;
        var opIndex = data.Length - 1;
        var caseCount = data[0].Count;

        for (var i = caseCount - 1; i >= 0; i--)
        {
            var op = data[opIndex][i].Trim();

            var numberLen = data[0][i].Length;
            var numbers = Enumerable.Range(0, numberLen).Select(x => "").ToList();
            for (var j = numberLen - 1; j >= 0; j--)
            {
                for (var k = 0; k < nrsCount; k++)
                {
                    numbers[j] += data[k][i][j];
                }
            }

            if (op == "*")
            {
                sum += numbers.Select(x => long.Parse(x.Trim())).Aggregate((total, next) => total * next);
            }
            else if (op == "+")
            {
                sum += numbers.Select(x => long.Parse(x.Trim())).Sum();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        return sum;
    }
}