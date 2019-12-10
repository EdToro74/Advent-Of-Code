using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    static class Day1
    {
        public static int Part1(IEnumerable<string> input)
        {
            return input.Select(s => int.Parse(s)).Select(CalculateFuel).Sum();
        }

        public static int Part2(IEnumerable<string> input)
        {
            return input.Select(s => int.Parse(s)).Select(CalculateFuelRecursively).Sum();
        }

        public static int CalculateFuel(int mass)
        {
            return mass / 3 - 2;
        }

        public static int CalculateFuelRecursively(int mass)
        {
            if (mass <= 0)
            {
                return 0;
            }

            var fuel = CalculateFuel(mass);
            if (fuel <= 0)
            {
                return 0;
            }

            return fuel + CalculateFuelRecursively(fuel);
        }
    }
}
