using System;
using System.Collections.Generic;
using System.IO;

namespace Advent_Of_Code_2019
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine(Day01.Part1(GetDayFile(1)));
            Console.WriteLine(Day01.Part2(GetDayFile(1)));
            Console.WriteLine(Day02.Part1(GetDayFile(2)));
            Console.WriteLine(Day02.Part2(GetDayFile(2)));
            Console.WriteLine(Day03.Part1(GetDayFile(3)));
            Console.WriteLine(Day03.Part2(GetDayFile(3)));
            Console.WriteLine(Day04.Part1(GetDayFile(4)));
            Console.WriteLine(Day04.Part2(GetDayFile(4)));
            Console.WriteLine(Day05.Part1(GetDayFile(5)));
            Console.WriteLine(Day05.Part2(GetDayFile(5)));
            Console.WriteLine(Day06.Part1(GetDayFile(6)));
            Console.WriteLine(Day06.Part2(GetDayFile(6)));
            Console.WriteLine(Day07.Part1(GetDayFile(7)));
            Console.WriteLine(Day07.Part2(GetDayFile(7)));
            Console.WriteLine(Day08.Part1(GetDayFile(8), 25, 6));
            Console.WriteLine(Day08.Part2(GetDayFile(8), 25, 6));
            Console.WriteLine(Day09.Part1(GetDayFile(9)));
            Console.WriteLine(Day09.Part2(GetDayFile(9)));
            Console.WriteLine(Day10.Part1(GetDayFile(10)));
            Console.WriteLine(Day10.Part2(GetDayFile(10)));
            Console.WriteLine(Day11.Part1(GetDayFile(11)));
            Console.WriteLine(Day11.Part2(GetDayFile(11)));
            Console.WriteLine(Day12.Part1(GetDayFile(12)));
            Console.WriteLine(Day12.Part2(GetDayFile(12)));
            Console.WriteLine(Day13.Part1(GetDayFile(13)));
            Console.WriteLine(Day13.Part2(GetDayFile(13)));
            Console.WriteLine(Day14.Part1(GetDayFile(14)));
            Console.WriteLine(Day14.Part2(GetDayFile(14)));
            Console.WriteLine(Day15.Part1(GetDayFile(15)));
            Console.WriteLine(Day15.Part2(GetDayFile(15)));
            Console.WriteLine(Day16.Part1(GetDayFile(16)));
            Console.WriteLine(Day16.Part2(GetDayFile(16)));
            Console.WriteLine(Day17.Part1(GetDayFile(17)));
            Console.WriteLine(Day17.Part2(GetDayFile(17)));
            Console.WriteLine(Day18.Part1(GetDayFile(18)));
            Console.WriteLine(Day18.Part2(GetDayFile(18)));
            Console.WriteLine(Day19.Part1(GetDayFile(19)));
            Console.WriteLine(Day19.Part2(GetDayFile(19)));
            Console.WriteLine(Day20.Part1(GetDayFile(20)));
            Console.WriteLine(Day20.Part2(GetDayFile(20)));
            Console.WriteLine(Day21.Part1(GetDayFile(21)));
            Console.WriteLine(Day21.Part2(GetDayFile(21)));
            Console.WriteLine(Day22.Part1(GetDayFile(22)));
            Console.WriteLine(Day22.Part2(GetDayFile(22)));
            Console.WriteLine(Day23.Part1(GetDayFile(23)));
            Console.WriteLine(Day23.Part2(GetDayFile(23)));
            Console.WriteLine(Day24.Part1(GetDayFile(24)));
            Console.WriteLine(Day24.Part2(GetDayFile(24)));
            Console.WriteLine(Day25.Part1(GetDayFile(25)));
        }

        private static IEnumerable<string> GetDayFile(int day) => GetFile(Path.Combine(Directory.GetCurrentDirectory(), $@"..\..\..\Input\Day {day}.txt"));

        private static IEnumerable<string> GetFile(string path) => File.ReadLines(path);
    }
}
