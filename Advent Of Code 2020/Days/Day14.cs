using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day14
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(14);

            var memory = new Dictionary<long, long>();

            bool?[] mask = null;

            foreach (var line in input)
            {
                var parts = line.Split(" = ");
                if (parts.Length != 2)
                {
                    throw new InvalidOperationException("Unknown command");
                }

                if (parts[0] == "mask")
                {
                    mask = GetMask(parts[1]);
                }
                else if (parts[0].StartsWith("mem["))
                {
                    var memoryIndex = long.Parse(parts[0][4..^1]);
                    var value = long.Parse(parts[1]);

                    var maskedValue = ApplyMask(mask, value);
                    memory[memoryIndex] = maskedValue;
                }
            }

            Console.WriteLine($"Version 1: {memory.Values.Sum()}");

            memory.Clear();

            foreach (var line in input)
            {
                var parts = line.Split(" = ");
                if (parts.Length != 2)
                {
                    throw new InvalidOperationException("Unknown command");
                }

                if (parts[0] == "mask")
                {
                    mask = GetMask(parts[1]);
                }
                else if (parts[0].StartsWith("mem["))
                {
                    var startingMemoryIndex = long.Parse(parts[0][4..^1]);
                    var value = long.Parse(parts[1]);

                    foreach (var memoryIndex in ApplyMaskVersion2(mask, startingMemoryIndex))
                    {
                        memory[memoryIndex] = value;
                    }
                }
            }

            Console.WriteLine($"Version 2: {memory.Values.Sum()}");
        }

        private static long ApplyMask(bool?[] mask, long value)
        {
            var maskedValue = value;

            for (var i = 0; i < mask.Length; i++)
            {
                if (mask[i] == true)
                {
                    maskedValue |= 1L << i;
                }
                else if (mask[i] == false)
                {
                    maskedValue &= ~(1L << i);
                }
            }

            return maskedValue;
        }

        private static IEnumerable<long> ApplyMaskVersion2(bool?[] mask, long value)
        {
            var maskedValue = value;

            for (var i = 0; i < mask.Length; i++)
            {
                if (mask[i] == true)
                {
                    maskedValue |= 1L << i;
                }
            }

            var indeterminateBits = mask.Select((b, i) => (b, i)).Where(item => item.b == null).Select(item => item.i).ToArray();

            foreach (var permutation in PowerSet.FastPowerSet(indeterminateBits))
            {
                var valuePermutation = maskedValue;

                foreach (var bit in indeterminateBits)
                {
                    if (permutation.Contains(bit))
                    {
                        valuePermutation |= 1L << bit;
                    }
                    else
                    {
                        valuePermutation &= ~(1L << bit);
                    }
                }

                yield return valuePermutation;
            }
        }

        private static bool?[] GetMask(string input)
        {
            if (input.Length != 36)
            {
                throw new InvalidOperationException("Invalid input length");
            }

            return input.Reverse().Select<char, bool?>(c => c switch { 'X' => null, '1' => true, '0' => false, _ => throw new InvalidOperationException($"Uknown mask character: {c}") }).ToArray();
        }
    }
}
