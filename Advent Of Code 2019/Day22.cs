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
            var iterations = 1;
            var value = 2019;

            var shuffle = input.Select(command =>
            {
                if (command.StartsWith("deal into"))
                {
                    return new LinearFunction(-1, -1);
                }
                else if (command.StartsWith("cut"))
                {
                    var cut = int.Parse(command.Split(' ').Last());
                    return new LinearFunction(1, cut % deckSize * -1);
                }
                else if (command.StartsWith("deal with"))
                {
                    var increment = int.Parse(command.Split(' ').Last());
                    return new LinearFunction(increment, 0);
                }
                else
                {
                    throw new Exception($"Unknown command: {command}");
                }
            }).Aggregate(LinearFunction.Aggregate);

            return shuffle.ExecuteTimes(value, iterations, deckSize);
        }

        public static BigInteger Part2(IEnumerable<string> input)
        {
            var deckSize = 119315717514047L;
            var position = 2020;
            var iterations = 101741582076661L;

            var shuffle = input.Reverse().Select(command =>
            {
                if (command.StartsWith("deal into"))
                {
                    return new LinearFunction(-1, -1 - deckSize);
                }
                else if (command.StartsWith("cut"))
                {
                    var cut = int.Parse(command.Split(' ').Last());
                    return new LinearFunction(1, cut % deckSize);
                }
                else if (command.StartsWith("deal with"))
                {
                    var increment = int.Parse(command.Split(' ').Last());
                    var z = ModInverse(increment, deckSize);

                    return new LinearFunction(z % deckSize, 0);
                }
                else
                {
                    throw new Exception($"Unknown command: {command}");
                }
            }).Aggregate(LinearFunction.Aggregate);

            return shuffle.ExecuteTimes(position, iterations, deckSize);
        }

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
            if (v < 0)
            {
                v = (v + n) % n;
            }

            return v;
        }

        private class LinearFunction
        {
            public BigInteger K { get; }
            public BigInteger M { get; }

            public LinearFunction(BigInteger k, BigInteger m)
            {
                K = k;
                M = m;
            }

            public static LinearFunction Id => new LinearFunction(1, 0);

            public BigInteger Apply(BigInteger x) => x * K + M;

            public static LinearFunction Aggregate(LinearFunction f, LinearFunction g) => new LinearFunction(g.K * f.K, g.K * f.M + g.M);

            public BigInteger ExecuteTimes(BigInteger x, BigInteger numberOfTimes, BigInteger deckSize)
            {
                LinearFunction ExecuteTimesInternal(BigInteger k, BigInteger m, BigInteger iterations)
                {
                    if (iterations == 0)
                    {
                        return LinearFunction.Id;
                    }
                    else if (iterations % 2 == 0)
                    {
                        return ExecuteTimesInternal(k * k % deckSize, (k * m + m) % deckSize, iterations / 2);
                    }
                    else
                    {
                        var tail = ExecuteTimesInternal(k, m, iterations - 1);
                        return new LinearFunction(k * tail.K % deckSize, (k * tail.M + m) % deckSize);
                    }
                }

                var func = ExecuteTimesInternal(K, M, numberOfTimes);
                return (((func.Apply(x) + deckSize) % deckSize) + deckSize) % deckSize;
            }
        }
    }
}
