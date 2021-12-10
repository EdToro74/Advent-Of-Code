using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2021.Days
{
    internal class Day09
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(9);

            var heightMap = input.Select(line => line.Select(c => c - '0').ToArray()).ToArray();

            Console.WriteLine($"Part 1: {Part1(heightMap)}");
            Console.WriteLine($"Part 2: {Part2(heightMap)}");
        }

        private static int Part1(int[][] heightMap)
        {
            var lowPoints = heightMap.SelectMany((row, y) => row.Where((height, x) => GetNeighbors(heightMap, (x, y)).All(neighbor => neighbor.height > height)));

            return lowPoints.Select(height => height + 1).Sum();
        }

        private static int Part2(int[][] heightMap)
        {
            var lowPoints = heightMap.SelectMany((row, y) => row.Select((height, x) => (coords: (x, y), height)).Where(point => GetNeighbors(heightMap, point.coords).All(neighbor => neighbor.height > point.height)));

            var basinSizes = new List<int>();

            foreach (var lowPoint in lowPoints)
            {
                var basinPoints = new HashSet<(int x, int y)>();

                void BuildBasin((int x, int y) point)
                {
                    foreach (var neighbor in GetNeighbors(heightMap, point).Where(neighbor => neighbor.height != 9))
                    {
                        if (basinPoints.Add(neighbor.coords))
                        {
                            BuildBasin(neighbor.coords);
                        }
                    }
                }

                BuildBasin(lowPoint.coords);

                basinSizes.Add(basinPoints.Count);
            }

            return basinSizes.OrderByDescending(b => b).Take(3).Aggregate((l, r) => l * r);
        }


        private static IEnumerable<((int x, int y) coords, int height)> GetNeighbors(int[][] heightMap, (int x, int y) coords)
        {
            if (coords.x > 0)
            {
                yield return ((coords.x - 1, coords.y), heightMap[coords.y][coords.x - 1]);
            }
            if (coords.x < heightMap[0].Length - 1)
            {
                yield return ((coords.x + 1, coords.y), heightMap[coords.y][coords.x + 1]);
            }
            if (coords.y > 0)
            {
                yield return ((coords.x, coords.y - 1), heightMap[coords.y - 1][coords.x]);
            }
            if (coords.y < heightMap.Length - 1)
            {
                yield return ((coords.x, coords.y + 1), heightMap[coords.y + 1][coords.x]);
            }
        }
    }
}