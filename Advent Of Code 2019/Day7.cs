﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combinatorics.Collections;

namespace Advent_Of_Code_2019
{
    static class Day7
    {
        public static int Part1(IEnumerable<string> input)
        {
            return RunAmps(input, new[] { 0, 1, 2, 3, 4 });
        }

        public static int Part2(IEnumerable<string> input)
        {
            return RunAmps(input, new[] { 5, 6, 7, 8, 9 });
        }

        public static int RunAmps(IEnumerable<string> input, int[] phases)
        {
            var program = IntCodeProcessor.ParseProgram(input);

            var maxSignal = 0;

            foreach (var permutation in new Permutations<int>(phases))
            {
                var amps = new IEnumerator<int>[permutation.Count];
                var inputs = new Queue<int>[permutation.Count];
                var moveNextResults = new bool[permutation.Count];

                for (var i = 0; i < permutation.Count; i++)
                {
                    inputs[i] = new Queue<int>();
                    inputs[i].Enqueue(permutation[i]);
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