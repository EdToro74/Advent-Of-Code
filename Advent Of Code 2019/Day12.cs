using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    internal static class Day12
    {
        public static long Part1(IEnumerable<string> input)
        {
            var system = MotionSimulator.SimulateSystem(input).GetEnumerator();

            _ = system.MoveNext();

            for (var i = 0; i < 1000; i++)
            {
                _ = system.MoveNext();
            }

            return system.Current.Sum(o => o.TotalEnergy);
        }

        public static long Part2(IEnumerable<string> input)
        {
            var system = MotionSimulator.SimulateSystem(input).GetEnumerator();

            _ = system.MoveNext();

            var steps = 0L;
            var xCycle = 0L;
            var yCycle = 0L;
            var zCycle = 0L;

            do
            {
                _ = system.MoveNext();
                steps++;

                if (xCycle == 0 && system.Current.All(o => o.Velocity.X == 0))
                {
                    xCycle = steps;
                }

                if (yCycle == 0 && system.Current.All(o => o.Velocity.Y == 0))
                {
                    yCycle = steps;
                }

                if (zCycle == 0 && system.Current.All(o => o.Velocity.Z == 0))
                {
                    zCycle = steps;
                }
            } while (xCycle == 0 || yCycle == 0 || zCycle == 0);

            return Utility.LCM(xCycle, yCycle, zCycle) * 2;
        }

        public static void PrintSystem(IEnumerable<MotionSimulator.MotionObject> system)
        {
            foreach (var moon in system)
            {
                Console.WriteLine($"P: {moon.Position} V: {moon.Velocity}");
            }

            Console.WriteLine();
        }
    }
}
