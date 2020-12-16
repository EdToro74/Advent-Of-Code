using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day07
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(7);

            var parentToChildren = new Dictionary<string, IEnumerable<(int count, string bag)>>();
            var childToParent = new Dictionary<string, List<string>>();

            foreach (var line in input)
            {
                var match = Regex.Match(line, "(?<parentBag>[a-z ]+) bags contain (?<empty>no other bags.)?");
                var parentBag = match.Groups["parentBag"].Value;
                var isEmpty = match.Groups["empty"].Success;
                if (isEmpty)
                {
                    parentToChildren.Add(parentBag, Enumerable.Empty<(int, string)>());
                }
                else
                {
                    var children = new List<(int count, string bag)>();

                    parentToChildren.Add(parentBag, children);

                    foreach (Match childMatch in Regex.Matches(line, " (?<childAmount>[0-9]+) (?<childBag>[a-z ]+) (?:bag|bags)(?:,|\\.)"))
                    {
                        var amount = int.Parse(childMatch.Groups["childAmount"].Value);
                        var childBag = childMatch.Groups["childBag"].Value;

                        children.Add((amount, childBag));
                        if (!childToParent.ContainsKey(childBag))
                        {
                            childToParent.Add(childBag, new List<string>());
                        }
                        childToParent[childBag].Add(parentBag);
                    }
                }
            }

            var target = "shiny gold";
            var stack = new Stack<string>(new[] { target });

            var potentialParents = new HashSet<string>();

            while (stack.Any())
            {
                var current = stack.Pop();

                if (childToParent.TryGetValue(current, out var parents))
                {
                    foreach (var parent in parents)
                    {
                        if (potentialParents.Add(parent))
                        {
                            stack.Push(parent);
                        }
                    }
                }
            }

            Console.WriteLine($"{potentialParents.Count} bags can contain `{target}`");

            var totalBags = -1L;

            var childrenStack = new Stack<(int count, string bag)>(new[] { (1, target) });

            while (childrenStack.Any())
            {
                var current = childrenStack.Pop();

                totalBags += current.count;

                if (parentToChildren.TryGetValue(current.bag, out var children))
                {
                    foreach(var child in children)
                    {
                        childrenStack.Push((current.count * child.count, child.bag));
                    }
                }
            }

            Console.WriteLine($"{totalBags} needed to carry 1 `{target}`");
        }
    }
}