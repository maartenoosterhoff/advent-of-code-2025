using AdventOfCode2025.Puzzles.Utils;
using FluentAssertions;

namespace AdventOfCode2025.Puzzles.Puzzle8;

public class Runner
{
    [Test]
    [Arguments("TestInput", 40, 10)]
    [Arguments("Input", 46398, 1000)]
    public void RunAlpha(string filename, int expected, int connectCount)
    {
        var actual = Execute(filename, connectCount);
        actual.Should().Be(expected);
    }

    //[Test]
    //[Arguments("TestInput", 40)]
    //[Arguments("Input", 10733529153890L)]
    //public void RunBeta(string filename, long expected)
    //{
    //}

    private static int Execute(string filename, int connectCount)
    {
        var lines = EmbeddedResourceReader.Read<Runner>(filename);

        var junctionBoxes = lines.Select(JunctionBox.Create).ToArray();
        var permutations = Permutate(junctionBoxes).OrderBy(x => x.Distance).ToArray();
        var circuits = new List<Circuit>();
        for (int i = 0; i < connectCount; i++)
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
        }

        return circuits
            .Where(x => x.Size != 0)
            .Select(x => x.Size)
            .OrderByDescending(x => x)
            .Take(3)
            .Aggregate((total, next) => total * next);
    }

    private static List<Connection> Permutate(JunctionBox[] input)
    {
        var count = input.Length;
        var items = new List<Connection>();
        for (int i = 0; i < count - 1; i++)
        {
            for (int j = i + 1; j < count; j++)
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