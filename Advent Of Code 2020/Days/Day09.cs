using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day09
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(9).Select(l => long.Parse(l)).ToList();

            var bufferSize = 25;

            var buffer = new Queue<long>(input.Take(bufferSize));

            var invalidNumber = 0L;
            foreach (var item in input.Skip(bufferSize).Select((item, index) => (target: item, index)))
            {
                var result = Utility.Utility.FindTarget(buffer.ToArray(), item.target, 2);
                if (!result.found)
                {
                    Console.WriteLine($"Could not find two numbers that add up to {item.target} at index {item.index}");
                    invalidNumber = item.target;
                    break;
                }

                buffer.Dequeue();
                buffer.Enqueue(item.target);
            }

            if (invalidNumber == 0) throw new InvalidOperationException("Couldn't find invalid number");

            for (var windowSize = 2; windowSize < input.Count; windowSize++)
            {
                var result = input.SlidingWindow(windowSize).FirstOrDefault(window => window.Sum() == invalidNumber);
                if (result != null)
                {
                    var min = result.Min();
                    var max = result.Max();

                    Console.WriteLine($"Encryption weakness: {min} + {max} = {min + max}");
                    break;
                }
            }
        }
    }
}
