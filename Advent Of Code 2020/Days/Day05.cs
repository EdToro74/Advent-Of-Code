using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day05
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(5);

            var seats = input.Select(seat => BinarySpacePartition(seat[0..7], 'F', 'B') * 8 + BinarySpacePartition(seat[7..], 'L', 'R')).OrderBy(s => s).ToList();
            Console.WriteLine($"Max Seat: {seats.Last()}");

            foreach (var (x, y) in seats.EnumeratePairs())
            {
                if (x != y - 1)
                {
                    Console.WriteLine($"My Seat: {x + 1}");
                }
            }
        }

        private static int BinarySpacePartition(string input, char lowerHalfToken, char upperHalfToken)
        {
            var min = 0;
            var max = 1 << input.Length;

            for (var i = 0; i < input.Length; i++)
            {
                var token = input[i];

                if (i == input.Length - 1)
                {
                    if (token == lowerHalfToken)
                    {
                        return min;
                    }
                    else if (token == upperHalfToken)
                    {
                        return max - 1;
                    }
                }

                var next = min + ((max - min) / 2);
                if (token == lowerHalfToken)
                {
                    max = next;
                }
                else if (token == upperHalfToken)
                {
                    min = next;
                }
            }

            throw new InvalidOperationException();
        }
    }
}
