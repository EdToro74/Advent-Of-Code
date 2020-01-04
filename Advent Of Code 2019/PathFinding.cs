using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    public static class PathFinding
    {
        public static (bool success, IEnumerable<T> path) FindPath<T>(T start, T target, Func<T, T, int> getCost, Func<T, T, int> getHeuristic, Func<T, IEnumerable<T>> getAccessibleNeighbors)
        {
            var cameFrom = new Dictionary<T, T>();
            var costSoFar = new Dictionary<T, int>();

            var frontier = new Priority_Queue.SimplePriorityQueue<T>();
            frontier.Enqueue(start, 0);

            costSoFar[start] = 0;

            T current = start;

            while (frontier.Count() > 0)
            {
                current = frontier.Dequeue();

                if (current.Equals(target))
                {
                    break;
                }

                foreach (var next in getAccessibleNeighbors(current))
                {
                    var newCost = costSoFar[current] + getCost(current, next);
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        var priority = newCost + getHeuristic(next, target);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }

            var success = target.Equals(current);
            var path = new List<T>();

            do
            {
                path.Add(current);
            } while (cameFrom.TryGetValue(current, out current));

            return (success, path);
        }
    }
}
