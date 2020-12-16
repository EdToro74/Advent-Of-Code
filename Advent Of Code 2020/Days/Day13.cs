using System;
using System.Linq;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day13
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(13);

            var startTime = int.Parse(input.First());

            var buses = input.Last().Split(',').Where(id => id != "x").Select(id => int.Parse(id));

            var nextBus = buses.Select(id => (id, nextTime: id * ((startTime / id) + 1))).OrderBy(b => b.nextTime).First();

            Console.WriteLine($"Bus {nextBus.id} is next at time {nextBus.nextTime}. Result: {nextBus.id * (nextBus.nextTime - startTime)}");

            var inputs = input.Last().Split(',').Select((s, i) =>
            {
                _ = int.TryParse(s, out var x);
                return (Mod: x, Remainder: x - i);
            }).Where(item => item.Mod != 0).ToList();

            var solution = 0L;
            var totalProduct = inputs.Select(i => (long)i.Mod).Aggregate(1L, (n, a) => a * n);
            foreach (var item in inputs)
            {
                var n = totalProduct / item.Mod;
                var inverse = MultiplicativeInverse(n, item.Mod);
                solution += item.Remainder * n * inverse;
            }

            solution %= totalProduct;
            Console.WriteLine($"Alignment at: {solution}");
        }

        public static long MultiplicativeInverse(long a, long m)
        {
            if (m == 1) return 0;
            long m0 = m;
            (long x, long y) = (1, 0);

            while (a > 1)
            {
                long q = a / m;
                (a, m) = (m, a % m);
                (x, y) = (y, x - q * y);
            }
            return x < 0 ? x + m0 : x;
        }
    }
}
