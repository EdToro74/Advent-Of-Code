using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2021.Days
{
    internal class Day03
    {
        public static void Run()
        {
            var report = Utility.Utility.GetDayFile(3).ToList();

            Part1(report);
            Part2(report);
        }

        private static void Part1(IList<string> report)
        {
            var length = report[0].Length;

            var gamma = new BitArray(length);
            var epsilon = new BitArray(length);

            for (var i = 0; i < length; i++)
            {
                var mostCommon = GetMostCommon(report, i, '0');
                if (mostCommon == '1')
                {
                    gamma[length - 1 - i] = true;
                    epsilon[length - 1 - i] = false;
                }
                else
                {
                    gamma[length - 1 - i] = false;
                    epsilon[length - 1 - i] = true;
                }
            }

            var gammaNumber = FromBitArray(gamma);
            var epsilonNumber = FromBitArray(epsilon);

            Console.WriteLine($"{gammaNumber} * {epsilonNumber} = {gammaNumber * epsilonNumber}");
        }

        private static void Part2(IList<string> report)
        {
            var oxygenGeneratorRating = GetReading((items, index) => GetMostCommon(items, index, '1'));
            var co2ScrubberRating = GetReading((items, index) => GetLeastCommon(items, index, '0'));

            Console.WriteLine($"{oxygenGeneratorRating} * {co2ScrubberRating} = {oxygenGeneratorRating * co2ScrubberRating}");

            int GetReading(Func<IList<string>, int, char> selector)
            {
                var potentials = report.ToList();

                var index = 0;
                while (potentials.Count > 1)
                {
                    var mostCommon = selector(potentials, index);
                    _ = potentials.RemoveAll(reading => reading[index] != mostCommon);
                    index++;
                }

                return FromString(potentials[0]);
            }
        }

        private static int FromBitArray(BitArray bitArray)
        {
            var array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }

        private static int FromString(string number)
        {
            var bitArray = new BitArray(number.Length);

            for (var i = 0; i < number.Length; i++)
            {
                bitArray[number.Length - 1 - i] = number[i] == '1';
            }

            return FromBitArray(bitArray);
        }

        private static char GetMostCommon(IList<string> readings, int index, char tieBreaker) => readings.Select(reading => reading[index]).GroupBy(n => n).OrderByDescending(g => g.Count()).ThenBy(g => g.Key == tieBreaker ? 0 : 1).First().Key;

        private static char GetLeastCommon(IList<string> readings, int index, char tieBreaker) => readings.Select(reading => reading[index]).GroupBy(n => n).OrderBy(g => g.Count()).ThenBy(g => g.Key == tieBreaker ? 0 : 1).First().Key;
    }
}
