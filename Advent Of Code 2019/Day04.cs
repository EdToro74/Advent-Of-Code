using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    static class Day04
    {
        public static int Part1(IEnumerable<string> input)
        {
            var parts = input.First().Split('-');
            var start = int.Parse(parts[0]);
            var end = int.Parse(parts[1]);

            var found = 0;

            for (var candidate = start; candidate <= end; candidate++)
            {
                if (HasAdjacentMatchingDigits(candidate) && DigitsNeverDecrease(candidate))
                {
                    found++;
                }
            }

            return found;
        }

        public static int Part2(IEnumerable<string> input)
        {
            var parts = input.First().Split('-');
            var start = int.Parse(parts[0]);
            var end = int.Parse(parts[1]);

            var found = 0;

            for (var candidate = start; candidate <= end; candidate++)
            {
                if (HasPairedMatchingDigits(candidate) && DigitsNeverDecrease(candidate))
                {
                    found++;
                }
            }

            return found;
        }

        private static bool DigitsNeverDecrease(int candidate)
        {
            var digits = candidate.ToString();

            var last = char.MinValue;

            foreach (var digit in digits)
            {
                if (digit < last)
                {
                    return false;
                }

                last = digit;
            }

            return true;
        }

        private static bool HasAdjacentMatchingDigits(int candidate)
        {
            var last = char.MinValue;

            foreach (var current in candidate.ToString())
            {
                if (current == last)
                {
                    return true;
                }

                last = current;
            }

            return false;
        }

        /// <summary>
        /// Paired matching digits must not be part of a larger group of matching digits.
        /// </summary>
        private static bool HasPairedMatchingDigits(int candidate)
        {
            var digits = candidate.ToString();

            for (var i = 1; i < digits.Length; i++)
            {
                // Does it match previous
                if (digits[i] != digits[i - 1])
                {
                    continue;
                }

                // We matched the previous, make sure we don't match the next
                if (i != digits.Length - 1 && digits[i] == digits[i + 1])
                {
                    continue;
                }

                // We matched the previous, make sure we don't match the one before that
                if (i != 1 && digits[i] == digits[i - 2])
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}
