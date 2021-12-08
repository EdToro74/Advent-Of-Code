using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2021.Days
{
    internal class Day07
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(7).First();

            var crabPositions = input.Split(',').Select(int.Parse);

            Console.WriteLine($"Part 1: {AlignCrabs(crabPositions, i => i)}");
            Console.WriteLine($"Part 2: {AlignCrabs(crabPositions, i => (i + 1) * i / 2)}");
        }

        public static int AlignCrabs(IEnumerable<int> crabPositions, Func<int, int> costAlgorithm)
        {
            var start = crabPositions.Min();
            var end = crabPositions.Max();

            var bestPositionFuel = Enumerable.Range(start, end - start + 1).Select(i => crabPositions.Select(p => costAlgorithm(Math.Abs(p - i))).Sum()).Min();

            return bestPositionFuel;
        }
    }
}