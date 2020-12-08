using System;
using System.Linq;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day03
    {
        public static void Run()
        {
            var map = Utility.Utility.GetDayFile(3).ToArray();

            Part1(map);
            Part2(map);
        }

        private static void Part1(string[] map)
        {
            var treesHit = Traverse(map, 3, 1);
            Console.WriteLine($"{treesHit} trees hit");
        }

        private static void Part2(string[] map)
        {
            var slopes = new[]
            {
                (x: 1, y: 1),
                (x: 3, y: 1),
                (x: 5, y: 1),
                (x: 7, y: 1),
                (x: 1, y: 2)
            };

            var total = 1L;

            foreach (var slope in slopes)
            {
                var treesHit = Traverse(map, slope.x, slope.y);
                total *= treesHit;
            }

            Console.WriteLine($"Total: {total}");
        }

        private static int Traverse(string[] map, int slopeX, int slopeY)
        {
            var xLength = map[0].Length;

            var currentX = 0;
            var currentY = 0;

            var treesHit = 0;

            do
            {
                if (map[currentY][currentX] == '#')
                {
                    treesHit++;
                }

                currentX += slopeX;
                currentX %= xLength;
                currentY += slopeY;
            } while (currentY < map.Length);

            return treesHit;
        }
    }
}
