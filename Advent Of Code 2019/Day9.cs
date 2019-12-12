﻿using System.Collections.Generic;
using System.Text;

namespace Advent_Of_Code_2019
{
    static class Day9
    {
        public static string Part1(IEnumerable<string> input)
        {
            var sb = new StringBuilder();
            foreach (var output in IntCodeProcessor.ProcessProgram(input, 1))
            {
                sb.Append(output);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static string Part2(IEnumerable<string> input)
        {
            var sb = new StringBuilder();
            foreach (var output in IntCodeProcessor.ProcessProgram(input, 2))
            {
                sb.Append(output);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
