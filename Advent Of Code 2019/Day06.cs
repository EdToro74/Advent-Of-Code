using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    static class Day06
    {
        public static int Part1(IEnumerable<string> input)
        {
            var tree = BuildTree(input);

            return tree.TotalOrbits;
        }

        public static int Part2(IEnumerable<string> input)
        {
            var tree = BuildTree(input);

            var current = tree;

            var foundChild = true;
            var lastHasSan = current.HasDescendant("SAN");
            var lastHasYou = current.HasDescendant("YOU");

            while (foundChild)
            {
                foundChild = false;
                foreach (var child in current.Children)
                {
                    var hasSan = child.HasDescendant("SAN");
                    var hasYou = child.HasDescendant("YOU");

                    if (hasSan.hasDescendant && hasYou.hasDescendant)
                    {
                        current = child;
                        lastHasSan = hasSan;
                        lastHasYou = hasYou;
                        foundChild = true;
                        break;
                    }
                }
            }

            return lastHasSan.orbitsAway + lastHasYou.orbitsAway;
        }

        private static OrbitTreeNode BuildTree(IEnumerable<string> input)
        {
            var nodes = new Dictionary<string, OrbitTreeNode>();

            OrbitTreeNode FindNode(string name, string parentName = null)
            {
                if (name == null)
                {
                    return null;
                }

                var parentNode = FindNode(parentName);

                if (!nodes.TryGetValue(name, out var node))
                {
                    node = new OrbitTreeNode(name);
                    node.Parent = parentNode;
                    nodes[name] = node;
                }
                else
                {
                    if (node.Parent != null && parentNode != null)
                    {
                        throw new Exception($"Node {node.Name} already has a parent {node.Parent.Name}");
                    }
                    else if (parentNode != null)
                    {
                        node.Parent = parentNode;
                    }
                }

                return node;
            }

            foreach (var line in input)
            {
                var parts = line.Split(')');

                var parentName = parts[0];
                var childName = parts[1];

                var parent = FindNode(parentName);
                var child = FindNode(childName, parentName);

                parent.Children.Add(child);
            }

            return nodes.Values.Single(n => n.Parent == null);
        }

        private class OrbitTreeNode
        {
            public string Name { get; }
            public List<OrbitTreeNode> Children { get; } = new List<OrbitTreeNode>();
            public OrbitTreeNode Parent { get; set; }

            public OrbitTreeNode(string name)
            {
                Name = name;
            }

            public int Orbits
            {
                get
                {
                    var node = this;
                    var orbits = 0;

                    while (node.Parent != null)
                    {
                        orbits++;
                        node = node.Parent;
                    }

                    return orbits;
                }
            }

            public int TotalOrbits
            {
                get
                {
                    return Orbits + Children.Sum(c => c.TotalOrbits);
                }
            }

            public (bool hasDescendant, int orbitsAway) HasDescendant(string descendantName)
            {
                var directChild = Children.SingleOrDefault(c => c.Name == descendantName);
                if (directChild != null)
                {
                    return (true, 0);
                }

                foreach (var child in Children)
                {
                    (var hasDescendant, int orbitsAway) = child.HasDescendant(descendantName);
                    if (hasDescendant)
                    {
                        return (true, orbitsAway + 1);
                    }
                }

                return (false, 0);
            }
        }
    }
}
