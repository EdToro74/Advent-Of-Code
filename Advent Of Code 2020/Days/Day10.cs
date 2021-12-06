using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day10
    {
        public static void Run()
        {
            var input = new[] { 0 }.Concat(Utility.Utility.GetDayFile(10).Select(l => int.Parse(l))).OrderBy(i => i).ToList();

            var differences = new Dictionary<int, int>()
            {
                [1] = 0,
                [2] = 0,
                [3] = 1
            };

            foreach (var (x, y) in input.EnumeratePairs())
            {
                var difference = y - x;
                differences[difference]++;
            }

            Console.WriteLine($"{differences[1]} * {differences[3]} = {differences[1] * differences[3]}");

            var optionals = new List<int>();
            var oneOfRequired = new List<IEnumerable<int>>();

            foreach (var window in input.SlidingWindow(5))
            {
                if (window.Last() - window.First() == 4)
                {
                    var required = window.Skip(1).Take(3).ToList();
                    oneOfRequired.Add(required);
                }
                else if (window.ElementAt(3) - window.ElementAt(1) <= 3)
                {
                    optionals.Add(window.ElementAt(2));
                }
            }

            _ = optionals.RemoveAll(o => oneOfRequired.Any(r => r.Contains(o)));

            Console.WriteLine(Math.Pow(2, optionals.Count) * Math.Pow(7, oneOfRequired.Count));
        }
    }
}
