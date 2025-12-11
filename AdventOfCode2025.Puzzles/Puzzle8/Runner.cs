using AdventOfCode2025.Puzzles.Utils;
using FluentAssertions;
using System.Collections.Concurrent;

namespace AdventOfCode2025.Puzzles.Puzzle8;

public class Runner
{
    [Test]
    [Arguments("TestInput", 40)]
    [Arguments("Input", 0)]
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

        var junctionBoxes = lines.Select(JunctionBox.Create).ToArray();
        var permutations = Permutate(junctionBoxes).OrderBy(x => x.Distance).ToArray();
        var circuits = new List<Circuit>();
        for (int i = 0; i < 10; i++)
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
            else
            {
                // Both are not null
                // TODO: Combine circuits
            }

        }

        return 0;
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

        public int Size => JunctionBoxes.Count;
    }
}