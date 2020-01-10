using System.Collections.Generic;
using System.Text;

namespace Advent_Of_Code_2019
{
    static class Day05
    {
        public static string Part1(IEnumerable<string> input)
        {
            var inputs = new[] { 1L };

            var sb = new StringBuilder();
            foreach (var output in IntCodeProcessor.ProcessProgram(input, inputs))
            {
                sb.Append(output);
            }

            return sb.ToString();
        }

        public static string Part2(IEnumerable<string> input)
        {
            var inputs = new[] { 5L };

            var sb = new StringBuilder();
            foreach (var output in IntCodeProcessor.ProcessProgram(input, inputs))
            {
                sb.Append(output);
            }

            return sb.ToString();
        }
    }
}
