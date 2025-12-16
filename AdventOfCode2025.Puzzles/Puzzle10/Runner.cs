using AdventOfCode2025.Puzzles.Utils;
using FluentAssertions;
using System.Collections;
using System.Collections.Immutable;

namespace AdventOfCode2025.Puzzles.Puzzle10;

public class Runner
{
    [Test]
    [Arguments("TestInput", 7)]
    [Arguments("Input", 432L)]
    public void RunAlpha(string filename, long expected)
    {
        var actual = Execute(filename, m => m.FindFewestButtonPresses1());
        actual.Should().Be(expected);
    }

    [Test]
    [Arguments("TestInput", 33)]
    [Arguments("Input", 0)]
    public void RunBeta(string filename, long expected)
    {
        var actual = Execute(filename, m => m.FindFewestButtonPresses2());
        actual.Should().Be(expected);
    }

    private static long Execute(string filename, Func<Machine, int> finder)
    {
        var lines = EmbeddedResourceReader.Read<Runner>(filename);

        var machines = lines.Select(Machine.Parse).ToArray();
        var fewestButtonPressedPerMachine = machines.Select(finder).ToArray();
        var totalPresses = fewestButtonPressedPerMachine.Sum();
        return totalPresses;
    }

    public sealed class Machine
    {
        private readonly BitArray _desiredState;
        private readonly BitArray _currentState;
        private readonly List<int> _desiredJoltages;

        private readonly List<BitArray> _buttons;

        public Machine(
            string desiredState,
            string[] buttons,
            string desiredJoltages)
        {
            _desiredState = new BitArray(desiredState.Trim('[').Trim(']').Select(x => x == '#').ToArray());
            _currentState = new BitArray(_desiredState.Length);
            _buttons = buttons.Select(x =>
            {
                var lightsInButton = x.Trim('(').Trim(')').Split(',').Select(int.Parse).ToArray();
                var button = new BitArray(Enumerable.Range(0, _desiredState.Length).Select(x => lightsInButton.Contains(x)).ToArray());
                return button;
            }).ToList();
            _desiredJoltages = desiredJoltages.Trim('{').Trim('}').Split(",").Select(int.Parse).ToList();
        }

        public int FindFewestButtonPresses1()
        {
            var current = new List<int>();
            for (int seqLen = 1; seqLen <= _buttons.Count; seqLen++)
            {
                var p = PopulateSequence(current, seqLen, _buttons.Count);
                if (p > 0)
                {
                    return p;
                }
            }

            return 0;

            int PopulateSequence(List<int> current, int seqLen, int btnCount)
            {
                for (int i = 0; i < btnCount; i++)
                {
                    if (!current.Contains(i))
                    {
                        current.Add(i);
                        if (current.Count == seqLen)
                        {
                            var isOk = IsDesiredStateAchivedWithButtonPresses(current, false);
                            if (isOk)
                            {
                                return current.Count;
                            }
                        }
                        else
                        {
                            var r = PopulateSequence(current, seqLen, btnCount);
                            if (r > 0)
                            {
                                return r;
                            }
                        }
                        current.Remove(i);
                    }
                }

                return 0;
            }
        }

        public int FindFewestButtonPresses2()
        {
            var current = new List<int>();
            for (int seqLen = 1; seqLen <= _buttons.Count; seqLen++)
            {
                for (int maxBtnCount = 1; maxBtnCount <= seqLen; maxBtnCount++)
                {
                    var p = PopulateSequence(current, seqLen, maxBtnCount, _buttons.Count);
                    if (p > 0)
                    {
                        return p;
                    }
                }
            }

            return 0;

            int PopulateSequence(List<int> current, int seqLen, int maxBtnCount, int btnCount)
            {
                for (int button = 0; button < btnCount; button++)
                {
                    for (int currentButton = 0; currentButton < maxBtnCount; currentButton++)
                    {
                        current.Add(button);
                        if (current.Count == seqLen)
                        {
                            var isOk = IsDesiredStateAchivedWithButtonPresses(current, true);
                            if (isOk)
                            {
                                return current.Count;
                            }
                        }
                        else
                        {
                            var r = PopulateSequence(current, seqLen, maxBtnCount, btnCount);
                            if (r > 0)
                            {
                                return r;
                            }
                        }
                        current.Remove(button);
                    }
                }

                return 0;
            }
        }

        public bool IsDesiredStateAchivedWithButtonPresses(List<int> buttons, bool inclJoltages)
        {
            var current = new BitArray(_currentState);
            var currentJoltages = new int[_desiredJoltages.Count];
            foreach (var button in buttons)
            {
                for (int i = 0; i < current.Length; i++)
                {
                    if (_buttons[button][i])
                    {
                        current[i] = !current[i];
                        currentJoltages[i]++;
                    }
                }
            }
            // bool equals = ba1.Xor(ba2).OfType<bool>().All(e => !e);
            if (current.Xor(_desiredState).OfType<bool>().All(x => !x))
            {
                // Equal
                if (!inclJoltages)
                {
                    return true;
                }

                // Compare joltages
                if (currentJoltages.SequenceEqual(_desiredJoltages))
                {
                    return true;
                }
            }

            return false;
        }

        public static Machine Parse(string data)
        {
            var nodes = data.Split(' ');
            var nodeCount = nodes.Length;
            var lights = nodes[0];
            var buttons = nodes.Skip(1).Take(nodeCount - 2).ToArray();
            var joltages = nodes[nodes.Length - 1];
            return new Machine(lights, buttons, joltages);
        }
    }
}