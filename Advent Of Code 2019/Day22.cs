using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Advent_Of_Code_2019
{
    public static class Day22
    {
        public static BigInteger Part1(IEnumerable<string> input)
        {
            var deckSize = 10007;

            var result = ProcessShuffle(2019, deckSize, input).Last();
            var reverse = ProcessReverseShuffle(result, deckSize, input).Last();

            return result % deckSize;
        }

        public static BigInteger Part2(IEnumerable<string> input)
        {
            var deckSize = 119315717514047L;
            var position = 2020;
            var reverse = ProcessReverseShuffle(position, deckSize, input).Last() % deckSize;

            //101741582076661

            return reverse;
        }

        private static IEnumerable<BigInteger> ProcessShuffle(BigInteger index, BigInteger deckSize, IEnumerable<string> commands)
        {
            var current = index;

            foreach (var command in commands)
            {
                if (command.StartsWith("deal into"))
                {
                    current = NewStack(current, deckSize);
                }
                else if (command.StartsWith("deal with"))
                {
                    var increment = int.Parse(command.Split(' ').Last());
                    current = Increment(current, increment, deckSize);
                }
                else if (command.StartsWith("cut"))
                {
                    var cut = int.Parse(command.Split(' ').Last());
                    current = Cut(current, cut, deckSize);
                }
                else
                {
                    throw new Exception("Unknown command");
                }

                yield return current;
            }
        }

        private static IEnumerable<BigInteger> ProcessReverseShuffle(BigInteger index, BigInteger deckSize, IEnumerable<string> commands)
        {
            var current = index;

            foreach (var command in commands.Reverse())
            {
                if (command.StartsWith("deal into"))
                {
                    current = NewStackReverse(current, deckSize);
                }
                else if (command.StartsWith("deal with"))
                {
                    var increment = int.Parse(command.Split(' ').Last());
                    current = IncrementReverse(current, increment, deckSize);
                }
                else if (command.StartsWith("cut"))
                {
                    var cut = int.Parse(command.Split(' ').Last());
                    current = CutReverse(current, cut, deckSize);
                }
                else
                {
                    throw new Exception("Unknown command");
                }

                yield return current;
            }
        }

        private static BigInteger Cut(BigInteger index, int cutSize, BigInteger deckSize) => index - cutSize;

        private static BigInteger CutReverse(BigInteger index, int cutSize, BigInteger deckSize) => index + cutSize;

        private static BigInteger Increment(BigInteger index, int increment, BigInteger deckSize) => index * increment;

        private static BigInteger IncrementReverse(BigInteger index, int increment, BigInteger deckSize)
        {
            var modInverse = ModInverse2(increment, deckSize);
            return (modInverse % deckSize) * index;
        }

        private static BigInteger NewStack(BigInteger index, BigInteger deckSize) => -index - 1;

        private static BigInteger NewStackReverse(BigInteger index, BigInteger deckSize) => -index - (1 - deckSize);

        private static BigInteger ModInverse(BigInteger a, BigInteger n)
        {
            BigInteger i = n, v = 0, d = 1;
            while (a > 0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0) v = (v + n) % n;
            return v;
        }

        private static BigInteger ModInverse2(BigInteger a, BigInteger m)
        {
            a %= m;
            for (int x = 1; x < m; x++)
            {
                if (a * x % m == 1)
                {
                    return x;
                }
            }

            return 1;
        }
    }
}
