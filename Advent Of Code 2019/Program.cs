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
