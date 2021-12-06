using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2021.Days
{
    internal class Day06
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(6).First();

            var initialFish = input.Split(',').Select(int.Parse);

            Console.WriteLine($"Part 1: {SimulateFish(initialFish, 80)}");
            Console.WriteLine($"Part 2: {SimulateFish(initialFish, 256)}");
        }

        public static long SimulateFish(IEnumerable<int> initialFish, int days)
        {
            var fish = new Dictionary<int, long>(Enumerable.Range(0, 9).Select(i => new KeyValuePair<int, long>(i, 0)));

            foreach (var day in initialFish.GroupBy(f => f))
            {
                fish[day.Key] = day.Count();
            }

            for (var i = 0; i < days; i++)
            {
                var newFish = fish[0];

                for (var f = 0; f < 8; f++)
                {
                    fish[f] = fish[f + 1];
                }

                fish[6] += newFish;
                fish[8] = newFish;
            }

            return fish.Sum(d => d.Value);
        }
    }
}