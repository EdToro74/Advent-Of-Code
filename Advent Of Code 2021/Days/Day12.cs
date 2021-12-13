using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Advent_Of_Code_2021.Days
{
    internal class Day12
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(12).Select(line => line.Split('-'));

            var nodes = new Dictionary<string, HashSet<string>>();

            foreach (var line in input)
            {
                var leftNode = line[0];
                var rightNode = line[1];

                if (!nodes.TryGetValue(leftNode, out var neighbors))
                {
                    neighbors = new HashSet<string>();
                    nodes[leftNode] = neighbors;
                }

                _ = neighbors.Add(rightNode);

                if (!nodes.TryGetValue(rightNode, out neighbors))
                {
                    neighbors = new HashSet<string>();
                    nodes[rightNode] = neighbors;
                }

                _ = neighbors.Add(leftNode);
            }

            Console.WriteLine($"Part 1: {Part1(nodes)}");
            Console.WriteLine($"Part 2: {Part2(nodes)}");
        }

        private static int Part1(Dictionary<string, HashSet<string>> nodes) => Traverse("start", nodes, (node, path) => node.ToLower() != node || !path.Contains(node)).Count();

        private static int Part2(Dictionary<string, HashSet<string>> nodes) => Traverse("start", nodes, (node, path) =>
        {
            if (node.ToLower() != node)
            {
                return true;
            }

            if (node == "start")
            {
                return false;
            }

            var isRevisit = path.Contains(node);
            if (!isRevisit)
            {
                return true;
            }

            var hasRevisit = path.GroupBy(n => n).Any(g => g.Key.ToLower() == g.Key && g.Count() > 1);
            if (hasRevisit)
            {
                return false;
            }

            return true;
        }).Count();

        private static IEnumerable<IEnumerable<string>> Traverse(string node, Dictionary<string, HashSet<string>> nodes, Func<string, ImmutableList<string>, bool> revisitRule, ImmutableList<string> path = null)
        {
            path ??= ImmutableList.Create<string>();

            path = path.Add(node);

            if (node == "end")
            {
                yield return path;
                yield break;
            }

            foreach (var neighbor in nodes[node].Where(neighbor => revisitRule(neighbor, path)))
            {
                foreach (var neighborPath in Traverse(neighbor, nodes, revisitRule, path))
                {
                    yield return neighborPath;
                }
            }
        }
    }
}