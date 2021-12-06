using System.Collections.Generic;
using System.Text;

namespace Advent_Of_Code_2019
{
    internal static class Day09
    {
        public static string Part1(IEnumerable<string> input)
        {
            var sb = new StringBuilder();
            foreach (var output in IntCodeProcessor.ProcessProgram(input, 1))
            {
                _ = sb.Append(output);
                _ = sb.AppendLine();
            }

            return sb.ToString();
        }

        public static string Part2(IEnumerable<string> input)
        {
            var sb = new StringBuilder();
            foreach (var output in IntCodeProcessor.ProcessProgram(input, 2))
            {
                _ = sb.Append(output);
                _ = sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
