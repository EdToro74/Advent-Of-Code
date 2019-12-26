using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    static class Day15
    {
        public static int Part1(IEnumerable<string> input)
        {
            Console.CursorVisible = false;
            Console.WindowHeight = Console.LargestWindowHeight - 3;
            Console.SetWindowPosition(0, 0);

            var (map, deadEnds) = TraverseMap(input);

            var stepCount = 0;

            var lastCoords = (0, 0);

            var currentCoords = (0, 0);

            var allowedTiles = new[] { '.', 'O' };

            while (map[currentCoords] != 'O')
            {
                var nextCoords = GetNeighbors(currentCoords).Where(neighbor => map.ContainsKey(neighbor.coords) && allowedTiles.Contains(map[neighbor.coords]) && neighbor.coords != lastCoords && !deadEnds.Contains(neighbor.coords)).Single().coords;
                lastCoords = currentCoords;
                currentCoords = nextCoords;
                stepCount++;
            }

            return stepCount;
        }

        public static int Part2(IEnumerable<string> input)
        {
            Console.CursorVisible = false;
            Console.WindowHeight = Console.LargestWindowHeight - 3;
            Console.SetWindowPosition(0, 0);

            var (map, _) = TraverseMap(input, true);

            var oxygenated = new HashSet<(int x, int y)>();
            oxygenated.Add(map.Single(tile => tile.Value == 'O').Key);

            var producers = new HashSet<(int x, int y)>(oxygenated);

            var steps = 0;

            var needed = map.Where(tile => tile.Value == '.' || tile.Value == 'O').ToArray();

            while (needed.Length != oxygenated.Count)
            {
                var newProducers = new HashSet<(int x, int y)>();

                foreach (var neighbor in producers.SelectMany(source => GetNeighbors(source).Select(neighbor => neighbor.coords)).Distinct().Where(neighbor => map[neighbor] == '.' && !oxygenated.Contains(neighbor)))
                {
                    oxygenated.Add(neighbor);
                    newProducers.Add(neighbor);
                    map[neighbor] = 'O';
                }

                steps++;
                //DisplayMap(map, (-1, -1), null);
                producers = newProducers;
            }

            return steps;
        }

        private static void DisplayMap(Dictionary<(int x, int y), char> map, (int x, int y) currentCoords, Dictionary<(int x, int y), int> deadEnds)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(Utility.DisplayImage(Utility.ToJaggedArray(map, kvp => (kvp.Key.x, kvp.Key.y, kvp.Key == (0, 0) ? 'S' : kvp.Key == currentCoords ? 'C' : deadEnds?.ContainsKey(kvp.Key) == true ? 'D' : kvp.Value)), c => c));
        }

        private static IEnumerable<((int x, int y) coords, int direction)> GetNeighbors((int x, int y) coords)
        {
            yield return ((coords.x, coords.y + 1), 1);
            yield return ((coords.x, coords.y - 1), 2);
            yield return ((coords.x - 1, coords.y), 3);
            yield return ((coords.x + 1, coords.y), 4);
        }

        private static bool? IsWall(Dictionary<(int x, int y), char> map, (int x, int y) coords)
        {
            if (map.ContainsKey(coords))
            {
                return map[coords] == '#';
            }

            return null;
        }

        private static (bool leadsToDeadEnd, int escapeDirection) LeadsToDeadEnd(Dictionary<(int x, int y), char> map, (int x, int y) coords, Dictionary<(int x, int y), int> deadEnds)
        {
            var wallCount = 0;
            var deadEndCount = 0;
            var escapeDirection = 0;

            if (IsWall(map, coords) == true)
            {
                return (false, 0);
            }

            foreach (var neighbor in GetNeighbors(coords))
            {
                if (IsWall(map, neighbor.coords) == true)
                {
                    wallCount++;
                }
                else if (deadEnds.ContainsKey(neighbor.coords))
                {
                    deadEndCount++;
                }
                else
                {
                    escapeDirection = neighbor.direction;
                }
            }

            return (deadEndCount == wallCount - 1, escapeDirection);
        }

        private static void MarkDeadEnd(Dictionary<(int x, int y), char> map, (int x, int y) coords, int escapeDirection, Dictionary<(int x, int y), int> deadEnds)
        {
            if (deadEnds.ContainsKey(coords))
            {
                return;
            }

            if (map[coords] == '#')
            {
                throw new Exception("Marking non space as dead end");
            }

            deadEnds[coords] = escapeDirection;

            foreach (var neighbor in GetNeighbors(coords).Select(neighbor => (neighbor.coords, deadEnd: LeadsToDeadEnd(map, neighbor.coords, deadEnds))).Where(neighbor => neighbor.deadEnd.leadsToDeadEnd))
            {
                MarkDeadEnd(map, neighbor.coords, neighbor.deadEnd.escapeDirection, deadEnds);
            }
        }

        private static (Dictionary<(int x, int y), char> map, HashSet<(int x, int y)> deadEnds) TraverseMap(IEnumerable<string> input, bool fullTraverse = false)
        {
            var random = new Random();

            var currentCoords = (x: 0, y: 0);
            var lastInput = 0;

            var map = new Dictionary<(int x, int y), char>();
            var missing = new HashSet<(int x, int y)>();

            map[currentCoords] = '.';

            var deadEnds = new Dictionary<(int x, int y), int>();

            var program = IntCodeProcessor.ProcessProgramEnumerable(input, () =>
            {
                // If in a dead end, follow escape direction
                if (deadEnds.ContainsKey(currentCoords))
                {
                    lastInput = deadEnds[currentCoords];
                    return lastInput;
                }

                var nonWall = new List<int>();
                var preferred = new List<int>();

                foreach (var (coords, direction) in GetNeighbors(currentCoords))
                {
                    // Don't move back into a wall or dead-end
                    if (IsWall(map, coords) != true && !deadEnds.ContainsKey(coords))
                    {
                        nonWall.Add(direction);
                    }

                    if (!map.ContainsKey(coords))
                    {
                        missing.Add(coords);
                    }

                    // Prefer an unknown direction
                    if (!map.ContainsKey(coords))
                    {
                        preferred.Add(direction);
                    }
                }

                // Dead-end
                if (nonWall.Count == 1 && !deadEnds.ContainsKey(currentCoords) && map[currentCoords] == '.')
                {
                    MarkDeadEnd(map, currentCoords, nonWall[0], deadEnds);
                    lastInput = nonWall[0];
                    return lastInput;
                }

                var available = nonWall.Intersect(preferred).ToList();
                // No preferred available
                if (available.Count == 0)
                {
                    available = nonWall;
                }

                // If we can continue moving in the same direction, do so
                if (available.Contains(lastInput))
                {
                    return lastInput;
                }

                lastInput = available[random.Next(available.Count)];
                return lastInput;
            });

            var allowedRevisitTiles = new[] { '.', 'O' };

            foreach (var output in program)
            {
                var nextCoords = (currentCoords.x, currentCoords.y);

                switch (lastInput)
                {
                    case 1:
                        nextCoords.y++;
                        break;
                    case 2:
                        nextCoords.y--;
                        break;
                    case 3:
                        nextCoords.x--;
                        break;
                    case 4:
                        nextCoords.x++;
                        break;
                    default:
                        throw new Exception($"Unknown lastInput {lastInput}");
                }

                missing.Remove(nextCoords);

                if (output == 0)
                {
                    if (map.ContainsKey(nextCoords) && map[nextCoords] != '#')
                    {
                        throw new Exception("Value changed!");
                    }

                    map[nextCoords] = '#';
                    //DisplayMap(map, currentCoords, deadEnds);
                    continue;
                }

                if (map.ContainsKey(nextCoords) && !allowedRevisitTiles.Contains(map[nextCoords]))
                {
                    throw new Exception("Value changed!");
                }

                currentCoords = nextCoords;

                if (!map.ContainsKey(currentCoords))
                {
                    map[currentCoords] = '.';
                    foreach (var neighbor in GetNeighbors(currentCoords).Where(neighbor => !map.ContainsKey(neighbor.coords)))
                    {
                        missing.Add(neighbor.coords);
                    }
                }

                //DisplayMap(map, currentCoords, deadEnds);

                if (output == 2)
                {
                    map[currentCoords] = 'O';

                    //DisplayMap(map, currentCoords, deadEnds);

                    if (!fullTraverse)
                    {
                        return (map, new HashSet<(int x, int y)>(deadEnds.Keys));
                    }
                }

                if (fullTraverse && missing.Count == 0)
                {
                    //DisplayMap(map, (-1, -1), deadEnds);
                    return (map, new HashSet<(int x, int y)>(deadEnds.Keys));
                }
            }

            throw new Exception("Completed without finding the oxygen system");
        }
    }
}
