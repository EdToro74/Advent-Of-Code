using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day17
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(17);

            var cycles = 6;

            // Part 1
            var cubes = new HashSet<(int x, int y, int z)>(input.SelectMany((s, y) => s.Select((c, x) => (active: c == '#', x)).Where(cell => cell.active).Select(cell => (cell.x, y, z: 0))));

            for (var i = 0; i < cycles; i++)
            {
                var minX = cubes.Min(cell => cell.x) - 1;
                var maxX = cubes.Max(cell => cell.x) + 1;
                var minY = cubes.Min(cell => cell.y) - 1;
                var maxY = cubes.Max(cell => cell.y) + 1;
                var minZ = cubes.Min(cell => cell.z) - 1;
                var maxZ = cubes.Max(cell => cell.z) + 1;

                cubes = (from x in Enumerable.Range(minX, (maxX - minX) + 1)
                         from y in Enumerable.Range(minY, (maxY - minY) + 1)
                         from z in Enumerable.Range(minZ, (maxZ - minZ) + 1)
                         let liveNeighborCount = GetLiveNeighbors(cubes, (x, y, z))
                         where liveNeighborCount == 3 || (cubes.Contains((x, y, z)) && liveNeighborCount == 2)
                         select (x, y, z)).ToHashSet();
            }

            Console.WriteLine($"After 6 cycles {cubes.Count} cubes are active.");

            // Part 2
            var hyperCubes = new HashSet<(int x, int y, int z, int w)>(input.SelectMany((s, y) => s.Select((c, x) => (active: c == '#', x)).Where(cell => cell.active).Select(cell => (cell.x, y, z: 0, w: 0))));

            for (var i = 0; i < cycles; i++)
            {
                var minX = hyperCubes.Min(cell => cell.x) - 1;
                var maxX = hyperCubes.Max(cell => cell.x) + 1;
                var minY = hyperCubes.Min(cell => cell.y) - 1;
                var maxY = hyperCubes.Max(cell => cell.y) + 1;
                var minZ = hyperCubes.Min(cell => cell.z) - 1;
                var maxZ = hyperCubes.Max(cell => cell.z) + 1;
                var minW = hyperCubes.Min(cell => cell.w) - 1;
                var maxW = hyperCubes.Max(cell => cell.w) + 1;

                hyperCubes = (from x in Enumerable.Range(minX, (maxX - minX) + 1)
                              from y in Enumerable.Range(minY, (maxY - minY) + 1)
                              from z in Enumerable.Range(minZ, (maxZ - minZ) + 1)
                              from w in Enumerable.Range(minW, (maxW - minW) + 1)
                              let liveNeighborCount = GetLiveNeighbors(hyperCubes, (x, y, z, w))
                              where liveNeighborCount == 3 || (hyperCubes.Contains((x, y, z, w)) && liveNeighborCount == 2)
                              select (x, y, z, w)).ToHashSet();
            }

            Console.WriteLine($"After 6 cycles {hyperCubes.Count} hypercubes are active.");
        }

        public static int GetLiveNeighbors(HashSet<(int x, int y, int z)> cubes, (int x, int y, int z) coords)
        {
            var searchCubes = from x in Enumerable.Range(-1, 3)
                              from y in Enumerable.Range(-1, 3)
                              from z in Enumerable.Range(-1, 3)
                              where x != 0 || y != 0 || z != 0
                              select (coords.x + x, coords.y + y, coords.z + z);

            return cubes.Intersect(searchCubes).Count();
        }

        public static int GetLiveNeighbors(HashSet<(int x, int y, int z, int w)> hyperCubes, (int x, int y, int z, int w) coords)
        {
            var searchCubes = from x in Enumerable.Range(-1, 3)
                              from y in Enumerable.Range(-1, 3)
                              from z in Enumerable.Range(-1, 3)
                              from w in Enumerable.Range(-1, 3)
                              where x != 0 || y != 0 || z != 0 || w != 0
                              select (coords.x + x, coords.y + y, coords.z + z, coords.w + w);

            return hyperCubes.Intersect(searchCubes).Count();
        }

        private static void DrawCubes(HashSet<(int x, int y, int z)> cubes)
        {
            var minX = cubes.Min(cell => cell.x);
            var maxX = cubes.Max(cell => cell.x);
            var minY = cubes.Min(cell => cell.y);
            var maxY = cubes.Max(cell => cell.y);
            var minZ = cubes.Min(cell => cell.z);
            var maxZ = cubes.Max(cell => cell.z);

            for (var z = minZ; z <= maxZ; z++)
            {
                if (!cubes.Any(cell => cell.z == z))
                {
                    continue;
                }

                Console.WriteLine($"z={z}");
                for (var y = minY; y <= maxY; y++)
                {
                    for (var x = minX; x <= maxX; x++)
                    {
                        Console.Write(cubes.Contains((x, y, z)) ? '#' : '.');
                    }

                    Console.WriteLine();
                }

                Console.WriteLine();
            }
        }

        private static void DrawHyperCubes(HashSet<(int x, int y, int z, int w)> hyperCubes)
        {
            var minX = hyperCubes.Min(cell => cell.x);
            var maxX = hyperCubes.Max(cell => cell.x);
            var minY = hyperCubes.Min(cell => cell.y);
            var maxY = hyperCubes.Max(cell => cell.y);
            var minZ = hyperCubes.Min(cell => cell.z);
            var maxZ = hyperCubes.Max(cell => cell.z);
            var minW = hyperCubes.Min(cell => cell.w);
            var maxW = hyperCubes.Max(cell => cell.w);

            for (var w = minW; w <= maxW; w++)
            {
                for (var z = minZ; z <= maxZ; z++)
                {
                    if (!hyperCubes.Any(cell => cell.z == z || cell.w == w))
                    {
                        continue;
                    }

                    Console.WriteLine($"z={z}, w={w}");
                    for (var y = minY; y <= maxY; y++)
                    {
                        for (var x = minX; x <= maxX; x++)
                        {
                            Console.Write(hyperCubes.Contains((x, y, z, w)) ? '#' : '.');
                        }

                        Console.WriteLine();
                    }

                    Console.WriteLine();
                }
            }
        }
    }
}
