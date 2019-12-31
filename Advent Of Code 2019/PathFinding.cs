using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    public static class PathFinding
    {
        public static (bool success, IEnumerable<T> path) FindPath<T>(T start, T target, Func<T, T, int> getCost, Func<T, IEnumerable<T>> getAccessibleNeighbors)
        {
            T current = start;

            var parents = new Dictionary<T, T>();
            var costs = new Dictionary<T, int>();

            var open = new HashSet<T>();
            var closed = new HashSet<T>();
            var g = 0;

            open.Add(start);
            costs[start] = 0;

            while (open.Count > 0)
            {
                current = open.OrderBy(t => costs[t]).First();

                closed.Add(current);
                open.Remove(current);

                if (current.Equals(target))
                {
                    break;
                }

                g++;
                var neighbors = getAccessibleNeighbors(current);
                foreach (var neighbor in neighbors)
                {
                    if (closed.Contains(neighbor))
                    {
                        continue;
                    }

                    if (!open.Contains(neighbor))
                    {
                        var cost = getCost(neighbor, target);
                        costs[neighbor] = g + cost;
                        parents[neighbor] = current;
                        open.Add(neighbor);
                    }
                    else
                    {
                        var cost = getCost(neighbor, target);
                        if (g + cost < costs[neighbor])
                        {
                            costs[neighbor] = g + cost;
                            parents[neighbor] = current;
                        }
                    }
                }
            }

            var success = target.Equals(current);
            var path = new List<T>();

            do
            {
                path.Add(current);

            } while (parents.TryGetValue(current, out current));

            return (success, path);
        }
    }
}
