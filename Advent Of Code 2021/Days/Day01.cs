using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2021.Days
{
    internal class Day01
    {
        public static void Run()
        {
            var measurements = Utility.Utility.GetDayFile(1).Select(l => int.Parse(l)).ToArray();
            var increases = measurements.EnumeratePairs().Select(pair => pair.Item2 > pair.Item1 ? 1 : 0).Sum();
            Console.WriteLine($"Day 1 Part 1: {increases}");

            var windowIncreases = measurements.SlidingWindow(3).Select(window => window.Sum()).EnumeratePairs().Select(pair => pair.Item2 > pair.Item1 ? 1 : 0).Sum();
            Console.WriteLine($"Day 1 Part 2: {windowIncreases}");
        }
    }
}
