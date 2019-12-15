using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent_Of_Code_2019
{
    static class Utility
    {
        public static long LCM(params long[] numbers)
        {
            return numbers.Aggregate(LCM);
        }

        public static long LCM(IEnumerable<long> numbers)
        {
            return numbers.Aggregate(LCM);
        }

        private static long LCM(long a, long b)
        {
            return Math.Abs(a * b) / GCD(a, b);
        }

        private static long GCD(long a, long b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }
    }
}
