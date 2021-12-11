using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2021.Days
{
    internal class Day11
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(11).Select(line => line.Select(c => int.Parse(c.ToString())).ToArray());

            Console.WriteLine($"Part 1: {Part1(input.ToArray())}");
            Console.WriteLine($"Part 2: {Part2(input.ToArray())}");
        }

        private static int Part1(int[][] input)
        {
            var step = 0;
            var flashCount = 0;

            Simulate(input, stepFlashCount =>
            {
                flashCount += stepFlashCount;
                return ++step < 100;
            });

            return flashCount;
        }

        private static long Part2(int[][] input)
        {
            var step = 0;

            Simulate(input, _ =>
            {
                ++step;
                return !input.All(row => row.All(cell => cell == 0));
            });

            return step;
        }

        private static void Simulate(int[][] input, Func<int, bool> shouldContinue)
        {
            int stepFlashCount;
            do
            {
                Traverse(input, (x, y) => input[y][x]++);

                var flashed = new HashSet<(int x, int y)>();

                var hadChanges = true;

                while (hadChanges)
                {
                    hadChanges = false;

                    Traverse(input, (x, y) =>
                    {
                        if (input[y][x] > 9)
                        {
                            if (Flash(flashed, x, y))
                            {
                                hadChanges = true;
                            }
                        }
                    });
                }

                stepFlashCount = flashed.Count;

                Traverse(input, (x, y) =>
                {
                    if (input[y][x] > 9)
                    {
                        input[y][x] = 0;
                    }
                });
            } while (shouldContinue(stepFlashCount));

            bool Flash(HashSet<(int x, int y)> flashed, int x, int y)
            {
                if (!flashed.Add((x, y)))
                {
                    return false;
                }

                if (y > 0)
                {
                    IncrementRow(x, y - 1);
                }

                IncrementRow(x, y);

                if (y < input.Length - 1)
                {
                    IncrementRow(x, y + 1);
                }

                return true;
            }

            void IncrementRow(int x, int y)
            {
                if (x > 0)
                {
                    input[y][x - 1]++;
                }

                input[y][x]++;

                if (x < input[y].Length - 1)
                {
                    input[y][x + 1]++;
                }
            }

            void Traverse(int[][] input, Action<int, int> action)
            {
                for (var y = 0; y < input.Length; y++)
                {
                    for (var x = 0; x < input[y].Length; x++)
                    {
                        action(x, y);
                    }
                }
            }
        }
    }
}