using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent_Of_Code_2019
{
    public static class Day20
    {
        public static int Part1(IEnumerable<string> input)
        {
            var map = input.ToArray();

            var locations = ParseMap(map);

            var result = PathFinding.FindPath(locations.start, locations.end, (_, __) => 1, (_, __) => 0, current =>
                  {
                      var candidates = new List<(int x, int y)>();

                      if (current.x > 0)
                      {
                          candidates.Add((current.x - 1, current.y));
                      }
                      if (current.x < map[0].Length - 2)
                      {
                          candidates.Add((current.x + 1, current.y));
                      }
                      if (current.y > 0)
                      {
                          candidates.Add((current.x, current.y - 1));
                      }
                      if (current.y < map.Length - 2)
                      {
                          candidates.Add((current.x, current.y + 1));
                      }
                      foreach (var teleporter in locations.teleporters.Where(t => t.source.coordinates == current))
                      {
                          candidates.Add(teleporter.destination.coordinates);
                      }
                      foreach (var teleporter in locations.teleporters.Where(t => t.destination.coordinates == current))
                      {
                          candidates.Add(teleporter.source.coordinates);
                      }

                      return candidates.Where(c => map[c.y][c.x] == '.').ToArray();
                  });

            if (result.success)
            {
                return result.path.Count() - 1;
            }

            return -1;
        }

        public static int Part2(IEnumerable<string> input)
        {
            var map = input.ToArray();

            var locations = ParseMap(map);

            var result = PathFinding.FindPath((coords: (locations.start.x, locations.start.y), level: 0), (coords: (locations.end.x, locations.end.y), level: 0), (_, __) => 1, (_, __) => 0, current =>
            {
                var candidates = new List<((int x, int y) coords, int level)>();

                if (current.coords.x > 0)
                {
                    candidates.Add(((current.coords.x - 1, current.coords.y), current.level));
                }
                if (current.coords.x < map[0].Length - 2)
                {
                    candidates.Add(((current.coords.x + 1, current.coords.y), current.level));
                }
                if (current.coords.y > 0)
                {
                    candidates.Add(((current.coords.x, current.coords.y - 1), current.level));
                }
                if (current.coords.y < map.Length - 2)
                {
                    candidates.Add(((current.coords.x, current.coords.y + 1), current.level));
                }
                foreach (var teleporter in locations.teleporters.Where(t => t.source.coordinates == current.coords))
                {
                    candidates.Add((teleporter.destination.coordinates, teleporter.source.isInner ? current.level + 1 : current.level - 1));
                }
                foreach (var teleporter in locations.teleporters.Where(t => t.destination.coordinates == current.coords))
                {
                    candidates.Add((teleporter.source.coordinates, teleporter.destination.isInner ? current.level + 1 : current.level - 1));
                }

                return candidates.Where(c => map[c.coords.y][c.coords.x] == '.' && c.level >= 0).ToArray();
            });

            if (result.success)
            {
                return result.path.Count() - 1;
            }

            return -1;
        }

        private static ((int x, int y) start, (int x, int y) end, IEnumerable<(string name, ((int x, int y) coordinates, bool isInner) source, ((int x, int y) coordinates, bool isInner) destination)> teleporters) ParseMap(string[] input)
        {
            var locations = new Dictionary<string, List<((int x, int y) coordinates, bool isInner)>>();

            void AddLocation(string name, (int x, int y) coordinates, bool isInner)
            {
                if (!locations.TryGetValue(name, out var nameLocations))
                {
                    nameLocations = new List<((int x, int y) coordinates, bool isInner)>();
                    locations[name] = nameLocations;
                }
                nameLocations.Add((coordinates, isInner));
            }

            var y = 0;
            foreach (var row in input)
            {
                foreach (var (index, letter) in row.Select((c, i) => (i, c)).Where(d => char.IsLetter(d.c)))
                {
                    var name = string.Empty;
                    var coordinates = (x: 0, y: 0);
                    var isInner = false;

                    if (index < row.Length - 1 && char.IsLetter(row[index + 1]))
                    {
                        name += letter;
                        name += row[index + 1];
                        coordinates.y = y;
                        if (index > 1 && row[index - 1] == '.')
                        {
                            coordinates.x = index - 1;
                        }
                        else
                        {
                            coordinates.x = index + 2;
                        }
                        isInner = !(index == 0 || index == row.Length - 2);
                    }
                    else if (y < input.Length - 1 && char.IsLetter(input[y + 1][index]))
                    {
                        name += letter;
                        name += input[y + 1][index];
                        coordinates.x = index;
                        if (y > 1 && input[y - 1][index] == '.')
                        {
                            coordinates.y = y - 1;
                        }
                        else
                        {
                            coordinates.y = y + 2;
                        }
                        isInner = !(y == 0 || y == input.Length - 2);
                    }

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        AddLocation(name, coordinates, isInner);
                    }
                }

                y++;
            }

            var start = locations["AA"][0].coordinates;
            var end = locations["ZZ"][0].coordinates;

            var teleporters = new List<(string name, ((int x, int y) coordinates, bool isInner) source, ((int x, int y) coordinates, bool isInner) destination)>(locations.Where(kvp => kvp.Key != "AA" && kvp.Key != "ZZ").Select(kvp => (kvp.Key, kvp.Value[0], kvp.Value[1])));

            return (start, end, teleporters);
        }
    }
}
