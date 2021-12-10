using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2021.Days
{
    internal class Day08
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(8);

            Console.WriteLine($"Part 1: {Part1(input)}");
            Console.WriteLine($"Part 2: {Part2(input)}");
        }

        private static int Part1(IEnumerable<string> input)
        {
            var distinct = new HashSet<int> { 2, 3, 4, 7 };
            return input.Select(l => l.Split(" | ")[1].Split(" ").Where(d => distinct.Contains(d.Length)).Count()).Sum();
        }

        private static int Part2(IEnumerable<string> input)
        {
            var segmentsToDigit = new Dictionary<string, int>
            {
                ["abcefg"] = 0,
                ["cf"] = 1,
                ["acdeg"] = 2,
                ["acdfg"] = 3,
                ["bcdf"] = 4,
                ["abdfg"] = 5,
                ["abdefg"] = 6,
                ["acf"] = 7,
                ["abcdefg"] = 8,
                ["abcdfg"] = 9
            };

            var outputs = new List<int>();

            foreach (var line in input)
            {
                var wireMap = new Dictionary<char, char>();

                var parts = line.Split(" | ");
                var patterns = parts[0].Split(" ");

                var onePattern = patterns.Single(p => p.Length == 2);
                var sevenPattern = patterns.Single(p => p.Length == 3);

                wireMap['a'] = sevenPattern.Single(c => !onePattern.Contains(c));

                var segmentCounts = patterns.SelectMany(p => p).GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());

                wireMap['b'] = segmentCounts.Where(kvp => kvp.Value == 6).Select(kvp => kvp.Key).Single();
                wireMap['c'] = segmentCounts.Where(kvp => kvp.Value == 8 && kvp.Key != wireMap['a']).Select(kvp => kvp.Key).Single();
                wireMap['e'] = segmentCounts.Where(kvp => kvp.Value == 4).Select(kvp => kvp.Key).Single();
                wireMap['f'] = segmentCounts.Where(kvp => kvp.Value == 9).Select(kvp => kvp.Key).Single();

                wireMap['d'] = patterns.Where(p => p.Length == 4).Single().Except(new[] { wireMap['b'], wireMap['c'], wireMap['f'] }).Single();
                wireMap['g'] = "abcdefg".Except(wireMap.Values).Single();

                var reverseWireMap = wireMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

                var value = 0;
                foreach (var outputDigit in parts[1].Split(" ").Select(p => string.Join("", p.Select(c => reverseWireMap[c]).OrderBy(c => c))))
                {
                    var digit = segmentsToDigit[outputDigit];
                    value = (value * 10) + digit;
                }

                outputs.Add(value);
            }

            return outputs.Sum();
        }
    }
}