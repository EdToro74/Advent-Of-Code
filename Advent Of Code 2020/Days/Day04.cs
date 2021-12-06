using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day04
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(4);

            var requiredFieldPassports = 0;
            var validPassports = 0;

            var requiredFields = new[]
            {
                "byr",
                "iyr",
                "eyr",
                "hgt",
                "hcl",
                "ecl",
                "pid"
            };

            var passport = new Dictionary<string, string>();

            foreach (var line in input)
            {
                if (line == string.Empty)
                {
                    if (HasRequiredFields(passport, requiredFields))
                    {
                        requiredFieldPassports++;
                        if (IsValid(passport))
                        {
                            validPassports++;
                        }
                    }

                    passport.Clear();
                    continue;
                }

                var data = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var datum in data)
                {
                    var parts = datum.Split(':');
                    passport.Add(parts[0], parts[1]);
                }
            }

            Console.WriteLine($"{requiredFieldPassports} passports with required fields.");
            Console.WriteLine($"{validPassports} passports with required fields and valid data.");
        }

        private static bool HasRequiredFields(IDictionary<string, string> passport, string[] requiredFields) => passport.Keys.Intersect(requiredFields).Count() == requiredFields.Length;

        private static readonly IEnumerable<Func<IDictionary<string, string>, bool>> _rules = new List<Func<IDictionary<string, string>, bool>>()
        {
            d => int.TryParse(d["byr"], out var year) && year >= 1920 && year <= 2002,
            d => int.TryParse(d["iyr"], out var year) && year >= 2010 && year <= 2020,
            d => int.TryParse(d["eyr"], out var year) && year >= 2020 && year <= 2030,
            d =>
            {
                var height = d["hgt"];
                if (height.EndsWith("cm"))
                {
                    _ = int.TryParse(height[0..^2], out var cm);
                    return cm >= 150 && cm <= 193;
                }
                else if (height.EndsWith("in"))
                {
                    _ = int.TryParse(height[0..^2], out var inches);
                    return inches >= 59 && inches <= 76;
                }

                return false;
            },
            d => Regex.IsMatch(d["hcl"], "^#[0-9a-f]{6}$"),
            d => new[] { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" }.Contains(d["ecl"]),
            d => Regex.IsMatch(d["pid"], "^[0-9]{9}$")
        };

        private static bool IsValid(IDictionary<string, string> passport) => _rules.All(r => r(passport));
    }
}
