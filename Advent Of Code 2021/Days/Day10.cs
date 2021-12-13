using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2021.Days
{
    internal class Day10
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(10).ToList();

            Console.WriteLine($"Part 1: {Part1(input)}");
            Console.WriteLine($"Part 2: {Part2(input)}");
        }

        private static int Part1(List<string> input)
        {
            var scores = new Dictionary<char, int>()
            {
                [')'] = 3,
                [']'] = 57,
                ['}'] = 1197,
                ['>'] = 25137
            };

            return input.Select(Parse).Where(result => result.firstIllegalCharacter.HasValue).Sum(result => scores[result.firstIllegalCharacter.Value]);
        }

        private static long Part2(List<string> input)
        {
            var scores = new Dictionary<char, int>()
            {
                [')'] = 1,
                [']'] = 2,
                ['}'] = 3,
                ['>'] = 4
            };

            var lineScores = new List<long>();

            foreach (var result in input.Select(Parse).Where(result => result.firstIllegalCharacter == null))
            {
                var lineScore = 0L;
                foreach(var missing in result.expectedCharacters)
                {
                    lineScore *= 5;
                    lineScore += scores[missing];
                }

                lineScores.Add(lineScore);
            }

            lineScores.Sort();
            return lineScores[lineScores.Count / 2];
        }

        private static (char? firstIllegalCharacter, Stack<char> expectedCharacters) Parse(string line)
        {
            var closeChars = new HashSet<char>() { ')', ']', '}', '>' };

            var expectedCharacters = new Stack<char>();

            foreach (var c in line)
            {
                if (closeChars.Contains(c))
                {
                    if (expectedCharacters.Count == 0 || c != expectedCharacters.Peek())
                    {
                        return (c, null);
                    }

                    _ = expectedCharacters.Pop();
                }
                else
                {
                    expectedCharacters.Push((char)(c + (c == '(' ? 1 : 2)));
                }
            }

            return (null, expectedCharacters);
        }
    }
}