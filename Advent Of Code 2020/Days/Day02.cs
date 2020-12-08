using System;
using System.Linq;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day02
    {
        public static void Run()
        {
            var passwords = Utility.Utility.GetDayFile(2).Select(Validation.Parse);
            Console.WriteLine($"Part 1: {passwords.Count(password => password.IsValidPart1)} valid passwords");
            Console.WriteLine($"Part 2: {passwords.Count(password => password.IsValidPart2)} valid passwords");
        }

        private class Validation
        {
            public Validation(string password, char character, int min, int max)
            {
                Password = password;
                Character = character;
                FirstIndex = min;
                LastIndex = max;
            }

            public bool IsValidPart1 => Password.Count(c => c == Character) >= FirstIndex && Password.Count(c => c == Character) <= LastIndex;

            public bool IsValidPart2 => Password[FirstIndex - 1] == Character ^ Password[LastIndex - 1] == Character;

            public string Password { get; }
            public char Character { get; }
            public int FirstIndex { get; }
            public int LastIndex { get; }

            public static Validation Parse(string input)
            {
                var parts = input.Split(new char[] { ' ', '-', ':' }, StringSplitOptions.RemoveEmptyEntries);

                var firstIndex = int.Parse(parts[0]);
                var lastIndex = int.Parse(parts[1]);
                var character = parts[2][0];
                var password = parts[3];

                return new Validation(password, character, firstIndex, lastIndex);
            }
        }
    }
}
