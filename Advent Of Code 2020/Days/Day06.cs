using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day06
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(6);

            var allSeed = Enumerable.Range(0, 26).Select(i => (char)(i + 97)).ToList();

            var groupAnswersAny = new HashSet<char>();
            var groupAnswersAll = new HashSet<char>(allSeed);

            var totalAny = 0;
            var totalAll = 0;

            foreach (var line in input)
            {
                if (line == string.Empty)
                {
                    totalAny += groupAnswersAny.Count;
                    groupAnswersAny.Clear();

                    totalAll += groupAnswersAll.Count;
                    groupAnswersAll = new HashSet<char>(allSeed);
                    continue;
                }

                groupAnswersAny.UnionWith(line);
                groupAnswersAll.IntersectWith(line);
            }

            totalAny += groupAnswersAny.Count;
            totalAll += groupAnswersAll.Count;

            Console.WriteLine($"Total Any: {totalAny}");
            Console.WriteLine($"Total All: {totalAll}");
        }
    }
}
