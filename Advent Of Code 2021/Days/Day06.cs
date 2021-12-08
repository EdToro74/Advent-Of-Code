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
            const int cycleLength = 7;
            const int delayPeriod = 2;

            var fish = new Dictionary<int, long>(Enumerable.Range(0, cycleLength).Select(i => new KeyValuePair<int, long>(i, 0)));

            foreach (var day in initialFish.GroupBy(f => f))
            {
                fish[day.Key] = day.Count();
            }

            var queue = new Queue<long>(Enumerable.Range(0, delayPeriod).Select(_ => 0L));

            for (var i = 0; i < days; i++)
            {
                var spawnersIndex = i % cycleLength;

                queue.Enqueue(fish[spawnersIndex]);

                var juveniles = queue.Dequeue();

                fish[(spawnersIndex + cycleLength) % cycleLength] += juveniles;
            }

            return fish.Sum(d => d.Value) + queue.Sum();
        }
    }
}