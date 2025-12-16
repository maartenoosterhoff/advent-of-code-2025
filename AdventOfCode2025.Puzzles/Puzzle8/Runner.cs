using AdventOfCode2025.Puzzles.Utils;
using FluentAssertions;

namespace AdventOfCode2025.Puzzles.Puzzle8;

public class Runner
{
    [Test]
    [Arguments("TestInput", 40, 10L)]
    [Arguments("Input", 46398, 1000L)]
    public void RunAlpha(string filename, int expected, int connectCount)
    {
        var actual = Execute(
            filename,
            connectCount,
            (_, _) => false,
            static (_, circuits) => circuits
                .Where(x => x.Size != 0)
                .Select(x => x.Size)
                .OrderByDescending(x => x)
                .Take(3)
                .Aggregate((total, next) => total * next));
        actual.Should().Be(expected);
    }

    [Test]
    [Arguments("TestInput", 25272L)]
    [Arguments("Input", 8141888143L)]
    public void RunBeta(string filename, long expected)
    {
        var actual = Execute(
            filename,
            int.MaxValue,
            static (junctionBoxes, circuits) =>
                circuits.Count(x => x.Size > 0) == 1 &&
                circuits.Sum(x => x.Size) == junctionBoxes.Count,
            static (lastConnection, _) => (long)lastConnection.Left.X * (long)lastConnection.Right.X);
        actual.Should().Be(expected);
    }

    private static long Execute(string filename, int connectCount, Func<List<JunctionBox>, List<Circuit>, bool> earlyExit, Func<Connection, List<Circuit>, long> calculator)
    {
        var lines = EmbeddedResourceReader.Read<Runner>(filename);

        var junctionBoxes = lines.Select(JunctionBox.Create).ToList();
        var permutations = Permutate(junctionBoxes).OrderBy(x => x.Distance).ToList();
        var circuits = new List<Circuit>();
        Connection lastConnection = null!;
        for (var i = 0; i < connectCount; i++)
        {
            var perm = permutations[i];
            var leftCircuit = circuits.FirstOrDefault(x => x.Contains(perm.Left));
            var rightCircuit = circuits.FirstOrDefault(x => x.Contains(perm.Right));
            if (leftCircuit is null && rightCircuit is null)
            {
                circuits.Add(Circuit.Create(perm.Left, perm.Right));
            }
            else if (leftCircuit is null && rightCircuit is not null)
            {
                rightCircuit.Add(perm.Left);
            }
            else if (leftCircuit is not null && rightCircuit is null)
            {
                leftCircuit.Add(perm.Right);
            }
            else if (leftCircuit is not null && rightCircuit is not null)
            {
                if (leftCircuit.Equals(rightCircuit))
                {
                    // Both are not null but equal
                }
                else
                {
                    // Both are not null but different
                    leftCircuit.Combine(rightCircuit);
                    rightCircuit.Clear();
                }
            }

            lastConnection = perm;

            if (earlyExit(junctionBoxes, circuits))
            {
                break;
            }
        }

        return calculator(lastConnection, circuits);
    }

    private static List<Connection> Permutate(List<JunctionBox> input)
    {
        var count = input.Count;
        var items = new List<Connection>();
        for (var i = 0; i < count - 1; i++)
        {
            for (var j = i + 1; j < count; j++)
            {
                items.Add(new Connection(input[i], input[j]));
            }
        }

        return items;
    }


    private sealed record JunctionBox(int X, int Y, int Z)
    {
        public static JunctionBox Create(string line)
        {
            var numbers = line.Split(',').Select(int.Parse).ToArray();
            return new(numbers[0], numbers[1], numbers[2]);
        }
    }


    private sealed record Connection(JunctionBox Left, JunctionBox Right)
    {
        public long Distance { get; } =
            (long)Math.Sqrt(
                Math.Pow(Left.X - Right.X, 2) +
                Math.Pow(Left.Y - Right.Y, 2) +
                Math.Pow(Left.Z - Right.Z, 2));
    }

    private sealed record Circuit(List<JunctionBox> JunctionBoxes)
    {
        public static Circuit Create(JunctionBox left, JunctionBox right)
        {
            return new([left, right]);
        }
        public bool Contains(JunctionBox junctionBox) => JunctionBoxes.Contains(junctionBox);

        public void Add(JunctionBox junctionBox) => JunctionBoxes.Add(junctionBox);

        public void Combine(Circuit circuit) => JunctionBoxes.AddRange(circuit.JunctionBoxes);

        public int Size => JunctionBoxes.Count;

        public void Clear()
        {
            JunctionBoxes.Clear();
        }
    }
}