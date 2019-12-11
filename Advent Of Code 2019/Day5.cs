using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    static class Day5
    {
        public static void Part1(IEnumerable<string> input)
        {
            var inputs = new[] { 1 };

            IntCodeProcessor.ProcessProgram(input, inputs);
        }

        public static void Part2(IEnumerable<string> input)
        {
            var inputs = new[] { 5 };

            IntCodeProcessor.ProcessProgram(input, inputs);
        }
    }
}
