using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    public static class Day19
    {
        public static int Part1(IEnumerable<string> input)
        {
            var program = IntCodeProcessor.ParseProgram(input);

            var size = 50;

            var output = Scan(size).Select(i => IntCodeProcessor.ProcessProgram(IntCodeProcessor.CopyProgram(program), i).First()).ToArray();

            var index = 0;
            Console.WriteLine(Utility.DisplayImage(output.ToJaggedArray(o =>
            {
                var x = index % size;
                var y = index / size;
                index++;
                var tile = o == 1 ? '#' : '.';
                return (x, y, tile);
            }), c => c));

            return output.Count(o => o == 1);
        }

        public static int Part2(IEnumerable<string> input)
        {
            var program = IntCodeProcessor.ParseProgram(input);

            var size = 100;

            var low = 0;
            var high = 5000;
            var found = false;

            var lowestLow = int.MaxValue;

            int CheckGuess(int y)
            {
                var startX = GetStart(y);
                while (IntCodeProcessor.ProcessProgram(IntCodeProcessor.CopyProgram(program), startX, y).First() == 0)
                {
                    startX++;
                }

                if (IntCodeProcessor.ProcessProgram(IntCodeProcessor.CopyProgram(program), startX, y).First() == 0)
                {
                    throw new Exception("Start is wrong.");
                }

                var beamWidth = Enumerable.Range(0, size + 1).Count(x => IntCodeProcessor.ProcessProgram(IntCodeProcessor.CopyProgram(program), x + startX, y).First() == 1);
                if (beamWidth < size)
                {
                    lowestLow = Math.Min(lowestLow, y);
                    return -1;
                }

                var topBeamWidth = Enumerable.Range(0, size + 1).Count(x => IntCodeProcessor.ProcessProgram(IntCodeProcessor.CopyProgram(program), x + startX, y - (size - 1)).First() == 1);
                var result = topBeamWidth.CompareTo(size);
                if (result == 0)
                {
                    found = true;
                }

                if (result == -1)
                {
                    lowestLow = Math.Min(lowestLow, y);
                }

                return result;
            }

            var bottomRow = Utility.BinarySearch(low, high, guess =>
            {
                var result = CheckGuess(guess);
                if (result == 0)
                {
                    var previousResult = CheckGuess(guess - 1);
                    if (previousResult == -1)
                    {
                        return 0;
                    }

                    return 1;
                }

                return result;
            });

            var y = bottomRow - (size - 1);
            var x = GetStart(bottomRow);

            if (found)
            {
                return x * 10000 + y;
            }

            return -1;
        }

        private static IEnumerable<long[]> Scan(int size)
        {
            for (var y = 0L; y < size; y++)
            {
                for (var x = 0L; x < size; x++)
                {
                    yield return new[] { x, y };
                }
            }
        }

        private static int GetStart(int y)
        {
            var fives = y / 62;

            var start = 5 * fives;

            var leftover = y - (62 * fives);
            if (leftover <= 12)
            {
                start++;
            }
            else if (leftover <= 24)
            {
                start += 2;
            }
            else if (leftover <= 37)
            {
                start += 3;

            }
            else if (leftover <= 49)
            {
                start += 4;
            }
            else if (leftover <= 62)
            {
                start += 5;
            }

            return y + start;
        }

    }
}
