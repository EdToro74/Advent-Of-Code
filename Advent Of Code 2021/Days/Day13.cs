using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2021.Days
{
    internal class Day13
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(13);

            var dots = new HashSet<(int x, int y)>();
            var folds = new List<(bool isY, int axis)>();

            foreach (var line in input)
            {
                if (line == string.Empty)
                {
                    continue;
                }
                else if (line.StartsWith("fold "))
                {
                    var isY = line.Contains("y=");
                    var axis = int.Parse(line.Split("=")[1]);
                    folds.Add((isY, axis));
                }
                else
                {
                    var coords = line.Split(',');
                    _ = dots.Add((int.Parse(coords[0]), int.Parse(coords[1])));
                }
            }

            Console.WriteLine($"Part 1: {Part1(new HashSet<(int x, int y)>(dots), folds)}");
            Console.WriteLine($"Part 2:");
            Part2(new HashSet<(int x, int y)>(dots), folds);
        }

        private static int Part1(HashSet<(int x, int y)> dots, IEnumerable<(bool isY, int axis)> folds)
        {
            Fold(dots, folds.Take(1));

            return dots.Count;
        }

        private static void Part2(HashSet<(int x, int y)> dots, List<(bool isY, int axis)> folds)
        {
            Fold(dots, folds);
            Print(dots);
        }

        private static void Fold(HashSet<(int x, int y)> dots, IEnumerable<(bool isY, int axis)> folds)
        {
            foreach (var fold in folds)
            {
                if (fold.isY)
                {
                    var maxY = dots.Max(dot => dot.y);
                    var i = 2;

                    for (var y = fold.axis + 1; y <= maxY; y++)
                    {
                        foreach (var dot in dots.Where(dot => dot.y == y).ToArray())
                        {
                            _ = dots.Remove(dot);
                            _ = dots.Add((dot.x, y - i));
                        }

                        i += 2;
                    }
                }
                else
                {
                    var maxX = dots.Max(dot => dot.x);
                    var i = 2;

                    for (var x = fold.axis + 1; x <= maxX; x++)
                    {
                        foreach (var dot in dots.Where(dot => dot.x == x).ToArray())
                        {
                            _ = dots.Remove(dot);
                            _ = dots.Add((dot.x - i, dot.y));
                        }

                        i += 2;
                    }
                }
            }
        }

        private static void Print(HashSet<(int x, int y)> dots)
        {
            var maxY = dots.Max(dot => dot.y);
            var maxX = dots.Max(dot => dot.x);

            for (var y = 0; y <= maxY; y++)
            {
                for (var x = 0; x <= maxX; x++)
                {
                    Console.Write(dots.Contains((x, y)) ? '#' : ' ');
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}