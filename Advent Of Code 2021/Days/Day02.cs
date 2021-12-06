using System;

namespace Advent_Of_Code_2021.Days
{
    internal class Day02
    {
        public static void Run()
        {
            var commands = Utility.Utility.GetDayFile(2);
            var (horizontal, depth) = SubmarineMovement.MoveBasic(commands);
            Console.WriteLine($"Day 1 Part 1: {horizontal * depth}");

            (horizontal, depth) = SubmarineMovement.Move(commands);
            Console.WriteLine($"Day 1 Part 2: {horizontal * depth}");
        }
    }
}
