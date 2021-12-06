using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent_Of_Code_2021.Days
{
    internal class Day05
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(5);

            Part1(input);
            Part2(input);
        }

        public static void Part1(IEnumerable<string> input)
        {
            var vents = GetVents(input, noDiagonal: true);
            Console.WriteLine(vents.Count(coord => coord.count >= 2));
        }

        public static void Part2(IEnumerable<string> input)
        {
            var vents = GetVents(input, noDiagonal: false);
            Console.WriteLine(vents.Count(coord => coord.count >= 2));
        }

        private static IEnumerable<((int x, int y) coord, int count)> GetVents(IEnumerable<string> input, bool noDiagonal)
        {
            var lines = input.Select(line =>
            {
                var parts = line.Split(" -> ");
                var start = parts[0].Split(',').Select(int.Parse).ToList();
                var end = parts[1].Split(',').Select(int.Parse).ToList();

                return new { Start = (x: start[0], y: start[1]), End = (x: end[0], y: end[1]) };
            });

            var vents = lines.SelectMany(coords =>
            {
                var lineCoordinates = new List<(int x, int y)>();

                if (noDiagonal && coords.Start.x != coords.End.x && coords.Start.y != coords.End.y)
                {
                    return lineCoordinates;
                }

                var current = coords.Start;

                do
                {
                    lineCoordinates.Add(current);
                    current = (current.x + coords.End.x.CompareTo(current.x), current.y + coords.End.y.CompareTo(current.y));
                } while (current != coords.End);

                lineCoordinates.Add(current);

                return lineCoordinates;
            }).GroupBy(coords => coords).Select(g => (coords: g.Key, count: g.Count()));

            //PrintVents(vents);

            return vents;
        }

        private static void PrintVents(IEnumerable<((int x, int y) coord, int count)> vents)
        {
            var maxY = vents.Max(vent => vent.coord.y);
            var maxX = vents.Max(vent => vent.coord.x);

            var sb = new StringBuilder();

            var orderedVents = vents.OrderBy(vent => vent.coord.y).ThenBy(vent => vent.coord.x).ToList();

            var ventIndex = 0;

            for (var y = 0; y <= maxY; y++)
            {
                for (var x = 0; x <= maxX; x++)
                {
                    if (ventIndex < orderedVents.Count && orderedVents[ventIndex].coord == (x, y))
                    {
                        _ = sb.Append(orderedVents[ventIndex].count);
                        ventIndex++;
                    }
                    else
                    {
                        _ = sb.Append('.');
                    }
                }
                _ = sb.AppendLine();
            }

            Console.WriteLine(sb);
        }
    }
}