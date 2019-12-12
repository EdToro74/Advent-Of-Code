using System;
using System.Collections.Generic;
using System.IO;

namespace Advent_Of_Code_2019
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(Day1.Part1(GetDayFile(1)));
            //Console.WriteLine(Day1.Part2(GetDayFile(1)));
            //Console.WriteLine(Day2.Part1(GetDayFile(2)));
            //Console.WriteLine(Day2.Part2(GetDayFile(2)));
            //Console.WriteLine(Day3.Part1(GetDayFile(3)));
            //Console.WriteLine(Day3.Part2(GetDayFile(3)));
            //Console.WriteLine(Day4.Part1(GetDayFile(4)));
            //Console.WriteLine(Day4.Part2(GetDayFile(4)));
            //Console.WriteLine(Day5.Part1(GetDayFile(5)));
            //Console.WriteLine(Day5.Part2(GetDayFile(5)));
            //Console.WriteLine(Day6.Part1(GetDayFile(6)));
            //Console.WriteLine(Day6.Part2(GetDayFile(6)));
            //Console.WriteLine(Day7.Part1(GetDayFile(7)));
            Console.WriteLine(Day7.Part2(GetDayFile(7)));
        }

        static IEnumerable<string> GetDayFile(int day)
        {
            return GetFile($@"../../../Input/Day {day}.txt");
        }

        static IEnumerable<string> GetFile(string path)
        {
            return File.ReadLines(path);
        }
    }
}
