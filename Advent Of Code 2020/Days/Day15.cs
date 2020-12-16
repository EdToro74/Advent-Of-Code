using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day15
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(15).First().Split(',').Select(s => int.Parse(s));

            var usedNumbers = new Dictionary<int, int>(input.Select((n, i) => new KeyValuePair<int, int>(n, i + 1)));

            var lastNumber = input.Last();

            for (var i = usedNumbers.Count + 1; i <= 2020; i++)
            {
                var toSpeak = 0;
                if (usedNumbers.TryGetValue(lastNumber, out var lastIndex))
                {
                    if (lastIndex != i - 1)
                    {
                        toSpeak = (i - 1) - lastIndex;
                    }
                }

                usedNumbers[lastNumber] = i - 1;
                lastNumber = toSpeak;
            }

            Console.WriteLine($"2020th number: {lastNumber}");

            for (var i = 2021; i <= 30000000; i++)
            {
                var toSpeak = 0;
                if (usedNumbers.TryGetValue(lastNumber, out var lastIndex))
                {
                    if (lastIndex != i - 1)
                    {
                        toSpeak = (i - 1) - lastIndex;
                    }
                }

                usedNumbers[lastNumber] = i - 1;
                lastNumber = toSpeak;
            }

            Console.WriteLine($"30000000 number: {lastNumber}");
        }
    }
}
