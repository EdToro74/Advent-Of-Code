using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day16
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(16);

            var rules = new List<Rule>();
            int[] myTicket = null;
            var nearbyTickets = new List<int[]>();

            var inputState = 0;

            foreach (var line in input)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    inputState++;
                    continue;
                }

                switch (inputState)
                {
                    case 0:
                        rules.Add(Rule.Parse(line));
                        break;
                    case 1:
                        if (myTicket != null) throw new InvalidOperationException("Already read my ticket");
                        if (line.StartsWith("your"))
                        {
                            continue;
                        }
                        myTicket = line.Split(',').Select(s => int.Parse(s)).ToArray();
                        break;
                    case 2:
                        if (line.StartsWith("nearby"))
                        {
                            continue;
                        }
                        nearbyTickets.Add(line.Split(',').Select(s => int.Parse(s)).ToArray());
                        break;
                }
            }

            if (nearbyTickets.Any(ticket => ticket.Length != myTicket.Length)) throw new InvalidOperationException("Not all tickets have the same value count");
            if (rules.Count != myTicket.Length) throw new InvalidOperationException("Rule count does not equal ticket value count");

            var invalidNumbers = nearbyTickets.SelectMany(t => t).Where(value => !rules.Any(rule => rule.IsValueValid(value)));
            Console.WriteLine($"Sum: {invalidNumbers.Sum()}");

            // Part 2
            var validTickets = nearbyTickets.Where(ticket => ticket.All(value => rules.Any(rule => rule.IsValueValid(value))));
            var fieldValues = Enumerable.Range(0, rules.Count).Select(i => validTickets.Concat(new[] { myTicket }).Select(ticket => ticket[i]).ToArray()).ToArray();

            var takenRules = new Dictionary<Rule, int>();
            bool progress;

            do
            {
                progress = false;
                foreach (var rule in rules.Except(takenRules.Keys))
                {
                    var singlePossibility = fieldValues.Select((values, index) => (values, index)).Where(item => !takenRules.Values.Contains(item.index) && item.values.All(value => rule.IsValueValid(value))).SingleOrFallback((null, -1));

                    if (singlePossibility.index != -1)
                    {
                        takenRules.Add(rule, singlePossibility.index);
                        progress = true;
                    }
                }
            } while (progress);

            if (takenRules.Count != rules.Count) throw new InvalidOperationException("Ambiguous answer");

            var value = 1L;
            foreach(var rule in rules.Where(rule => rule.Name.StartsWith("departure")))
            {
                value *= myTicket[takenRules[rule]];
            }

            Console.WriteLine($"Product of departure fields: {value}");
        }

        class Rule
        {
            public string Name { get; init; }
            public IEnumerable<(int min, int max)> Ranges { get; init; }

            public bool IsValueValid(int value)
            {
                return Ranges.Any(r => value >= r.min && value <= r.max);
            }

            public static Rule Parse(string input)
            {
                var match = Regex.Match(input, "^(?<name>[a-z ]+): (?:(?<min>[0-9]+)-(?<max>[0-9]+)(?: or )*)+$");
                return new Rule
                {
                    Name = match.Groups["name"].Captures[0].Value,
                    Ranges = match.Groups["min"].Captures.Cast<Capture>().Zip(match.Groups["max"].Captures.Cast<Capture>(), (min, max) => (int.Parse(min.Value), int.Parse(max.Value))).ToList()
                };
            }
        }
    }
}
