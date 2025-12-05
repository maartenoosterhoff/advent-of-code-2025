using AdventOfCode2025.Puzzles.Utils;
using FluentAssertions;

namespace AdventOfCode2025.Puzzles.Puzzle2;

public class Runner
{
    [Test]
    [Arguments("TestInput", 1227775554L)]
    [Arguments("Input", 52316131093L)]
    public void RunAlpha(string filename, long expected)
    {
        var actual = Execute(filename, IsInvalidId);
        actual.Should().Be(expected);
        return;

        static bool IsInvalidId(long id)
        {
            var idTxt = id.ToString();
            var len = idTxt.Length;
            if (len % 2 == 0)
            {
                var half = len / 2;
                if (idTxt[..half] == idTxt[half..])
                {
                    return true;
                }
            }
            return false;
        }
    }

    [Test]
    [Arguments("TestInput", 4174379265L)]
    [Arguments("Input", 69564213293L)]
    public void RunBeta(string filename, long expected)
    {
        var actual = Execute(filename, IsInvalidId);
        actual.Should().Be(expected);
        return;

        static bool IsInvalidId(long id)
        {
            var idTxt = id.ToString();
            var len = idTxt.Length;
            var max = len / 2 + 1;
            var curr = 1;
            while (curr <= max)
            {
                if (len % curr == 0 && len != curr)
                {
                    // first part
                    var exp = idTxt[..curr];
                    var pos = curr;
                    var isInvalid = true;
                    while (pos < len)
                    {
                        if (exp != idTxt.Substring(pos, curr))
                        {
                            isInvalid = false;
                        }
                        pos += curr;
                    }
                    if (isInvalid)
                    {
                        return true;
                    }
                }

                curr++;
            }

            return false;
        }
    }

    private static long Execute(string filename, Func<long, bool> isInvalidId)
    {
        var lines = EmbeddedResourceReader.Read<Runner>(filename);

        var sumOfInvalidIds = 0L;

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            var unparsedRanges = line.Split(',');
            foreach (var unparsedRange in unparsedRanges)
            {
                var rangeData = unparsedRange.Split('-');

                for (var id = long.Parse(rangeData[0]); id <= long.Parse(rangeData[1]); id++)
                {
                    if (isInvalidId(id))
                    {
                        sumOfInvalidIds += id;
                    }

                }
            }
        }

        return sumOfInvalidIds;
    }
}