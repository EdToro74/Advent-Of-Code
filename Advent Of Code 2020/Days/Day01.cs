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

            var result = numbers.AsSpan().FindTarget(target, 2);
            DisplayResult(result);

            result = numbers.AsSpan().FindTarget(target, 3);
            DisplayResult(result);
        }

        private static void DisplayResult((bool found, IEnumerable<int> addends) result) => Console.WriteLine($"{string.Join(" * ", result.addends)} = {result.addends.Aggregate((l, r) => l * r)}");
    }
}
