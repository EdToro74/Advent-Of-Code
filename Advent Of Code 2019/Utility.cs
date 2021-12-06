using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent_Of_Code_2019
{
    internal static class Utility
    {
        public static long LCM(params long[] numbers) => numbers.Aggregate(LCM);

        public static long LCM(IEnumerable<long> numbers) => numbers.Aggregate(LCM);

        public static TOut[,] ToJaggedArray<T, TOut>(this IEnumerable<T> items, Func<T, (int x, int y, TOut value)> mapper)
        {
            var objects = items.Select(mapper).ToArray();

            var xMax = objects.Max(p => p.x);
            var xMin = objects.Min(p => p.x);
            var xRange = xMax - xMin + 1;
            var xOffset = -xMin;

            var yMax = objects.Max(p => p.y);
            var yMin = objects.Min(p => p.y);
            var yRange = yMax - yMin + 1;
            var yOffset = -yMin;

            var result = new TOut[yRange, xRange];
            foreach (var (x, y, value) in objects)
            {
                result[y + yOffset, x + xOffset] = value;
            }

            return result;
        }

        public static string DisplayImage<T>(T[,] image, Func<T, char> valueMapper)
        {
            var height = image.GetLength(0);
            var width = image.GetLength(1);

            var display = new StringBuilder();

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var value = valueMapper(image[y, x]);
                    _ = display.Append(value);
                }

                _ = display.AppendLine();
            }

            return display.ToString();
        }

        public static int BinarySearch(int low, int high, Func<int, int> comparer)
        {
            while (low < high)
            {
                var mid = (low + high) / 2;
                var guessResult = comparer(mid);
                if (guessResult > 0)
                {
                    high = mid;
                }
                else if (guessResult < 0)
                {
                    if (mid == low)
                    {
                        break;
                    }

                    low = mid;
                }
                else
                {
                    low = mid;
                    break;
                }
            }

            return low;
        }

        public static long BinarySearch(long low, long high, Func<long, int> comparer)
        {
            while (low < high)
            {
                var mid = (low + high) / 2;
                var guessResult = comparer(mid);
                if (guessResult > 0)
                {
                    high = mid;
                }
                else if (guessResult < 0)
                {
                    if (mid == low)
                    {
                        break;
                    }

                    low = mid;
                }
                else
                {
                    low = mid;
                    break;
                }
            }

            return low;
        }

        private static long LCM(long a, long b) => Math.Abs(a * b) / GCD(a, b);

        private static long GCD(long a, long b) => b == 0 ? a : GCD(b, a % b);

        public static int ManhattanDistance((int x, int y) a, (int x, int y) b) => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }
}
