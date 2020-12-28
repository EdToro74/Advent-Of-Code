using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day23
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(23).First();

            var circle = input.Select(c => c - '0').ToList();

            var numMoves = 100;

            Console.Write("Part 1: ");
            var result = RunGame2(circle, numMoves);

            var current = result[1];
            for (var i = 0; i < 8; i++)
            {
                Console.Write(current);
                current = result[current];
            }

            Console.WriteLine();

            circle = new List<int>(circle.Concat(Enumerable.Range(10, 1_000_000 - 9)));

            numMoves = 10_000_000;

            Console.Write("Part 2: ");
            var result2 = RunGame2(circle, numMoves);
            Console.WriteLine($"{result2[1]} * {result2[result2[1]]} = {(long)result2[1] * result2[result2[1]]}");
        }

        private static Dictionary<int, int> RunGame2(List<int> circle, int numMoves)
        {
            var cups = new Dictionary<int, int>(circle.Concat(new[] { circle[0] }).EnumeratePairs((l, r) => new KeyValuePair<int, int>(l, r)));

            var max = cups.Max(kvp => kvp.Key);

            var current = circle[0];

            for (var i = 0; i < numMoves; i++)
            {
                //Console.WriteLine($"-- move {i + 1} --");

                //Console.Write($"cups: ");
                //var toPrint = 1;
                //for (var p = 0; p < 9; p++)
                //{
                //    Console.Write(toPrint == current ? $"({toPrint})" : toPrint);
                //    Console.Write(" ");
                //    toPrint = cups[toPrint];
                //}
                //Console.WriteLine();

                var pickup = new[]
                {
                    cups[current],
                    cups[cups[current]],
                    cups[cups[cups[current]]]
                };

                cups[current] = cups[pickup[2]];

                //Console.WriteLine($"pick up: {string.Join(", ", pickup)}");

                var destination = current == 1 ? max : current - 1;
                while (pickup.Contains(destination))
                {
                    destination--;
                    if (destination < 1)
                    {
                        destination = max;
                    }
                }

                //Console.WriteLine($"destination: {destination}");

                var temp = cups[destination];
                cups[destination] = pickup[0];
                cups[pickup[2]] = temp;

                //Console.WriteLine();

                current = cups[current];
            }

            return cups;
        }
    }
}