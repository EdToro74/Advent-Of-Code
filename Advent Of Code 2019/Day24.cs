using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    public static class Day24
    {
        public static int Part1(IEnumerable<string> input)
        {
            var index = 0;
            var width = input.First().Length;
            var height = input.Count();

            var grid = input.SelectMany(s => s.Select(c => c == '#' ? true : false)).ToJaggedArray(b => (index % width, index++ / height, b));

            var seen = new HashSet<int>();

            while (true)
            {
                var rating = CalculateRating(grid);
                if (!seen.Add(rating))
                {
                    return rating;
                }

                var next = new bool[height, width];

                for (var y = 0; y < grid.GetLength(0); y++)
                {
                    for (var x = 0; x < grid.GetLength(1); x++)
                    {
                        var live = GetLiveNeighbors(grid, x, y);
                        if (grid[y, x])
                        {
                            if (live != 1)
                            {
                                next[y, x] = false;
                            }
                            else
                            {
                                next[y, x] = true;
                            }
                        }
                        else
                        {
                            if (live == 1 || live == 2)
                            {
                                next[y, x] = true;
                            }
                            else
                            {
                                next[y, x] = false;
                            }
                        }
                    }
                }

                grid = next;
            }
        }

        public static int Part2(IEnumerable<string> input)
        {
            var width = 5;
            var height = 5;

            var inputGrid = input.SelectMany(s => s.Select(c => c == '#' ? true : false)).ToArray();

            var grids = new Dictionary<int, bool[]>() { { 0, inputGrid } };

            for (var minute = 0; minute < 200; minute++)
            {
                var nextGrids = new Dictionary<int, bool[]>();

                //Console.WriteLine($"Time {minute}:");
                var min = grids.Keys.Min();
                if (grids[min].Any(b => b))
                {
                    grids[min - 1] = new bool[height * width];
                }
                var max = grids.Keys.Max();
                if (grids[max].Any(b => b))
                {
                    grids[max + 1] = new bool[height * width];
                }

                foreach (var kvp in grids.OrderBy(kvp => kvp.Key))
                {
                    var level = kvp.Key;
                    var grid = kvp.Value;

                    var next = new bool[height * width];

                    for (var y = 0; y < height; y++)
                    {
                        for (var x = 0; x < width; x++)
                        {
                            var index = y * width + x;
                            if (index == 12)
                            {
                                continue;
                            }

                            var live = GetLiveNeighbors(index, level, grids);
                            if (grid[index])
                            {
                                if (live != 1)
                                {
                                    next[index] = false;
                                }
                                else
                                {
                                    next[index] = true;
                                }
                            }
                            else
                            {
                                if (live == 1 || live == 2)
                                {
                                    next[index] = true;
                                }
                                else
                                {
                                    next[index] = false;
                                }
                            }
                        }
                    }

                    nextGrids[level] = next;

                    //Console.WriteLine($"Depth {level}:");
                    //var i = 0;
                    //Console.WriteLine(Utility.DisplayImage(grid.ToJaggedArray(b => (i % 5, i++ / 5, i == 13 ? '?' : b ? '#' : '.')), c => c));
                    //Console.WriteLine();
                }

                grids = nextGrids;
            }

            return grids.Values.Sum(g => g.Count(c => c));
        }

        private static Dictionary<int, IEnumerable<(int levelModifier, int index)>> _neighbors = new Dictionary<int, IEnumerable<(int levelModifier, int index)>>()
        {
            { 1, new[] {(-1, 12), (0, 6), (0, 2), (-1, 8)} },
            { 2, new[] {(0, 1), (0, 7), (0, 3), (-1, 8)} },
            { 3, new[] {(0, 2), (0, 8), (0, 4), (-1, 8)} },
            { 4, new[] {(0, 3), (0, 9), (0, 5), (-1, 8)} },
            { 5, new[] {(0, 4), (0, 10), (-1, 14), (-1, 8)} },
            { 6, new[] {(-1, 12), (0, 11), (0, 7), (0, 1)} },
            { 7, new[] {(0, 6), (0, 12), (0, 8), (0, 2)} },
            { 8, new[] {(0, 7), (1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (0, 9), (0, 3) } },
            { 9, new[] {(0, 8), (0, 14), (0, 10), (0, 4)} },
            { 10, new[] {(0, 9), (0, 15), (-1, 14), (0, 5)} },
            { 11, new[] {(-1, 12), (0, 16), (0, 12), (0, 6)} },
            { 12, new[] {(0, 11), (0, 17), (1, 1), (1, 6), (1, 11), (1, 16), (1, 21), (0, 7)} },
            { 14, new[] {(1, 5), (1, 10), (1, 15), (1, 20), (1, 25), (0, 19), (0, 15), (0, 9)} },
            { 15, new[] {(0, 14), (0, 20), (-1, 14), (0, 10) } },
            { 16, new[] {(-1, 12), (0, 21), (0, 17), (0, 11) } },
            { 17, new[] {(0, 16), (0, 22), (0, 18), (0, 12)} },
            { 18, new[] {(0, 17), (0, 23), (0, 19), (1, 21), (1, 22), (1, 23), (1, 24), (1, 25)} },
            { 19, new[] {(0, 18), (0, 24), (0, 20), (0, 14)} },
            { 20, new[] {(0, 19), (0, 25), (-1, 14), (0, 15)} },
            { 21, new[] {(-1, 12), (-1, 18), (0, 22), (0, 16)} },
            { 22, new[] {(0, 21), (-1, 18), (0, 23), (0, 17)} },
            { 23, new[] {(0, 22), (-1, 18), (0, 24), (0, 18)} },
            { 24, new[] {(0, 23), (-1, 18), (0, 25), (0, 19)} },
            { 25, new[] {(0, 24), (-1, 18), (-1, 14), (0, 20)} }
        };

        private static int GetLiveNeighbors(int index, int level, Dictionary<int, bool[]> grids)
        {
            return _neighbors[index + 1].Select(neighbor => GetGridValue(level + neighbor.levelModifier, neighbor.index - 1, grids)).Count(c => c);
        }

        private static bool GetGridValue(int level, int index, Dictionary<int, bool[]> grids)
        {
            if (!grids.TryGetValue(level, out var grid))
            {
                return false;
            }

            return grid[index];
        }

        private static int CalculateRating(bool[,] grid)
        {
            var shift = 0;
            var value = 0;

            for (var y = 0; y < grid.GetLength(0); y++)
            {
                for (var x = 0; x < grid.GetLength(1); x++)
                {
                    if (grid[y, x])
                    {
                        value += 1 << shift;
                    }
                    shift++;
                }
            }

            return value;
        }

        private static int GetLiveNeighbors(bool[,] grid, int x, int y)
        {
            var live = 0;

            if (y > 0)
            {
                if (grid[y - 1, x])
                {
                    live++;
                }
            }
            if (y < grid.GetLength(0) - 1)
            {
                if (grid[y + 1, x])
                {
                    live++;
                }
            }
            if (x > 0)
            {
                if (grid[y, x - 1])
                {
                    live++;
                }
            }
            if (x < grid.GetLength(1) - 1)
            {
                if (grid[y, x + 1])
                {
                    live++;
                }
            }

            return live;
        }
    }
}
