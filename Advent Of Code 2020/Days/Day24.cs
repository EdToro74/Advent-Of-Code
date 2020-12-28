using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day24
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(24);

            var blackTiles = new HashSet<(int x, int y)>();

            foreach (var line in input)
            {
                var coords = Traverse(line);
                if (blackTiles.Contains(coords))
                {
                    blackTiles.Remove(coords);
                }
                else
                {
                    blackTiles.Add(coords);
                }
            }

            Console.WriteLine($"Part 1: {blackTiles.Count} black tiles");

            for (var i = 0; i < 100; i++)
            {
                var nextBlackTiles = new HashSet<(int x, int y)>();

                var inScope = blackTiles.Concat(blackTiles.SelectMany(coord => GetNeighbors(coord))).Distinct();

                foreach (var coord in inScope)
                {
                    var x = coord.x;
                    var y = coord.y;

                    var blackNeighbors = GetNeighbors((x, y)).Count(coord => blackTiles.Contains(coord));
                    if (blackTiles.Contains((x, y)) && blackNeighbors > 0 && blackNeighbors < 3)
                    {
                        nextBlackTiles.Add((x, y));
                    }
                    else if (!blackTiles.Contains((x, y)) && blackNeighbors == 2)
                    {
                        nextBlackTiles.Add((x, y));
                    }
                }

                blackTiles = nextBlackTiles;
            }

            Console.WriteLine($"Part 2: {blackTiles.Count} black tiles");
        }

        private static readonly Regex _parser = new Regex("^(?<direction>e|se|sw|w|nw|ne)+$");

        private static (int x, int y) Traverse(string directions)
        {
            var x = 0;
            var y = 0;

            foreach (var direction in _parser.Match(directions).Groups["direction"].Captures.Select(c => c.Value))
            {
                switch (direction)
                {
                    case "e":
                        x++;
                        break;
                    case "se":
                        y++;
                        break;
                    case "sw":
                        x--;
                        y++;
                        break;
                    case "w":
                        x--;
                        break;
                    case "nw":
                        y--;
                        break;
                    case "ne":
                        x++;
                        y--;
                        break;
                }
            }

            return (x, y);
        }

        private static IEnumerable<(int x, int y)> GetNeighbors((int x, int y) coords) => new[] {
            (coords.x + 1, coords.y),     // e
            (coords.x,     coords.y + 1), // se
            (coords.x - 1, coords.y + 1), // sw
            (coords.x - 1, coords.y),     // w
            (coords.x,     coords.y - 1), // nw
            (coords.x + 1, coords.y - 1)  // ne
        };
    }
}