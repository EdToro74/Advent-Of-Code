using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Advent_Of_Code_2019
{
    static class Day07
    {
        public static long Part1(IEnumerable<string> input)
        {
            return RunAmps(input, new[] { 0, 1, 2, 3, 4 });
        }

        public static long Part2(IEnumerable<string> input)
        {
            return RunAmps(input, new[] { 5, 6, 7, 8, 9 });
        }

        public static long RunAmps(IEnumerable<string> input, int[] phases)
        {
            var program = IntCodeProcessor.ParseProgram(input);

            var maxSignal = 0L;

            foreach (var permutation in Permutations.GetPermutations(phases))
            {
                var size = permutation.Count();
                var amps = new IEnumerator<long>[size];
                var inputs = new Queue<long>[size];
                var moveNextResults = new bool[size];

                for (var i = 0; i < size; i++)
                {
                    inputs[i] = new Queue<long>();
                    inputs[i].Enqueue(permutation.ElementAt(i));
                    if (i == 0)
                    {
                        inputs[i].Enqueue(0);
                    }

                    var copy = IntCodeProcessor.CopyProgram(program);
                    var ampIndex = i;
                    amps[i] = IntCodeProcessor.ProcessProgramEnumerable(copy, () => inputs[ampIndex].Dequeue()).GetEnumerator();
                }

                do
                {
                    for (var i = 0; i < amps.Length; i++)
                    {
                        var hasOutput = amps[i].MoveNext();
                        moveNextResults[i] = hasOutput;
                        if (hasOutput)
                        {
                            inputs[(i + 1) % amps.Length].Enqueue(amps[i].Current);
                        }
                    }
                } while (moveNextResults.Any(r => r));

                var finalSignal = inputs[0].Dequeue();

                maxSignal = Math.Max(maxSignal, finalSignal);
            }

            return maxSignal;
        }
    }
}