using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    public static class Day21
    {
        public static long Part1(IEnumerable<string> input)
        {
            var instructions = new[]
            {
                "OR A T",
                "AND B T",
                "AND C T",
                "NOT T J",
                "AND D J",
                "WALK"
            };

            var output = IntCodeProcessor.ProcessProgram(input, instructions.Select(s => s + '\n').SelectMany(s => s).Select(c => (long)c).ToArray());

            if (output.Last() > char.MaxValue)
            {
                return output.Last();
            }

            foreach (var c in output)
            {
                Console.Write((char)c);
            }

            return 0;
        }

        public static long Part2(IEnumerable<string> input)
        {
            var instructions = new[]
            {
                "OR A T",
                "AND B T",
                "AND C T",
                "NOT T J",
                "AND D J",
                "NOT E T",
                "NOT T T",
                "OR H T",
                "AND T J",
                "RUN"
            };

            var output = IntCodeProcessor.ProcessProgram(input, instructions.Select(s => s + '\n').SelectMany(s => s).Select(c => (long)c).ToArray());

            if (output.Last() > char.MaxValue)
            {
                return output.Last();
            }

            foreach (var c in output)
            {
                Console.Write((char)c);
            }

            return 0;
        }
    }
}
