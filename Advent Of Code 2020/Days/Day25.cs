using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day25
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(25);

            var subjectNumber = 7;
            var mod = 20201227;

            var cardPublicKey = int.Parse(input.First());
            var doorPublicKey = int.Parse(input.Skip(1).First());

            var cardLoopSize = FindLoopSize(subjectNumber, mod, cardPublicKey);
            var doorLoopSize = FindLoopSize(subjectNumber, mod, doorPublicKey);

            var encryptionKey = Encrypt(doorPublicKey, mod, cardLoopSize);
            Console.WriteLine($"Part 1: {encryptionKey}");
        }

        private static int FindLoopSize(int subjectNumber, int mod, int result)
        {
            var loopSize = 0;
            var currentValue = 1;

            while (currentValue != result)
            {
                loopSize++;
                currentValue *= subjectNumber;
                currentValue %= mod;
            }

            return loopSize;
        }

        private static int Encrypt(int subjectNumber, int mod, int loopSize)
        {
            var currentValue = 1L;
            for (var i = 0; i < loopSize; i++)
            {
                currentValue *= subjectNumber;
                currentValue %= mod;
            }

            return (int)currentValue;
        }
    }
}