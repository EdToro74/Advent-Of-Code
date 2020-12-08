using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day01
    {
        public static void Run()
        {
            var numbers = Utility.Utility.GetDayFile(1).Select(l => int.Parse(l)).ToArray();

            var target = 2020;

            var result = FindTarget(numbers, target, 2);
            DisplayResult(result);

            result = FindTarget(numbers, target, 3);
            DisplayResult(result);
        }

        private static void DisplayResult((bool found, IEnumerable<int> addends) result) => Console.WriteLine($"{string.Join(" * ", result.addends)} = {result.addends.Aggregate((l, r) => l * r)}");

        private static (bool found, IEnumerable<int> addends) FindTarget(Span<int> numbers, int target, int numberOfAddends)
        {
            if (numberOfAddends > 0)
            {
                for (var i = 0; i < numbers.Length; i++)
                {
                    var candidate = numbers[i];
                    if (numberOfAddends == 1 && candidate == target)
                    {
                        return (true, new[] { candidate });
                    }

                    var remaining = target - candidate;
                    var result = FindTarget(numbers[(i + 1)..], remaining, numberOfAddends - 1);
                    if (result.found)
                    {
                        return (true, result.addends.Concat(new[] { candidate }));
                    }
                }
            }

            return (false, null);
        }
    }
}
