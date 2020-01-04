using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Advent_Of_Code_2019
{
    public static class Day18
    {
        public static int Part1(IEnumerable<string> input)
        {
            var oldOut = Console.Out;
            Console.SetOut(TextWriter.Null);
            var map = input.ToList();

            var keys = string.Join("", map.SelectMany(c => c.ToLower()).Distinct().Where(c => c != '.' && c != '#').OrderBy(c => c));
            var keyLocations = new Dictionary<char, (int x, int y)>(
                keys.Select(key => new KeyValuePair<char, (int x, int y)>(key, FindLocation(map, key)))
            );

            var start = FindLocation(map, '@');

            var keyPaths = new Dictionary<char, Dictionary<char, (int steps, KeyMask keysNeeded, KeyMask keysCollected, bool isTeleport)>>();
            keyPaths['_'] = new Dictionary<char, (int steps, KeyMask keysNeeded, KeyMask keysCollected, bool isTeleport)>();
            FillKeyPaths(map, keys, keyLocations, keyPaths);
            _cache.Clear();
            var result = GatherAllKeys('@', keys, keyPaths, new KeyMask('@'));

            Console.SetOut(oldOut);
            Console.WriteLine(result.pathTaken);

            return result.steps;
        }

        public static int Part2(IEnumerable<string> input)
        {
            var oldOut = Console.Out;
            Console.SetOut(TextWriter.Null);
            var map = input.ToList();

            var originalStart = FindLocation(map, '@');
            map[originalStart.y] = map[originalStart.y].Replace(".@.", "###");
            map[originalStart.y - 1] = map[originalStart.y - 1].Substring(0, originalStart.x - 1) + "@#{" + map[originalStart.y - 1].Substring(originalStart.x + 2);
            map[originalStart.y + 1] = map[originalStart.y + 1].Substring(0, originalStart.x - 1) + "|#}" + map[originalStart.y + 1].Substring(originalStart.x + 2);

            foreach (var line in map)
            {
                Console.WriteLine(line);
            }

            var keys = string.Join("", map.SelectMany(c => c.ToLower()).Distinct().Where(c => c != '.' && c != '#').OrderBy(c => c));
            var keyLocations = new Dictionary<char, (int x, int y)>(
                keys.Select(key => new KeyValuePair<char, (int x, int y)>(key, FindLocation(map, key)))
            );

            var keyPaths = new Dictionary<char, Dictionary<char, (int steps, KeyMask keysNeeded, KeyMask keysCollected, bool isTeleport)>>
            {
                ['_'] = new Dictionary<char, (int steps, KeyMask keysNeeded, KeyMask keysCollected, bool isTeleport)>()
                {
                    { '@', (0, KeyMask.Empty, new KeyMask('@'), true) },
                    { '{', (0, KeyMask.Empty, new KeyMask('{'), true) },
                    { '|', (0, KeyMask.Empty, new KeyMask('|'), true) },
                    { '}', (0, KeyMask.Empty, new KeyMask('}'), true) }
                }
            };

            FillKeyPaths(map, keys, keyLocations, keyPaths);

            _cache.Clear();
            var result = GatherAllKeys('_', keys, keyPaths, KeyMask.Empty);

            Console.SetOut(oldOut);
            Console.WriteLine(result.pathTaken);

            return result.steps;
        }

        private static void FillKeyPaths(List<string> map, string keys, Dictionary<char, (int x, int y)> keyLocations, Dictionary<char, Dictionary<char, (int steps, KeyMask keysNeeded, KeyMask keysCollected, bool isTeleport)>> keyPaths)
        {
            for (var i = 0; i < keys.Length; i++)
            {
                var keyA = keys[i];
                for (var j = i + 1; j < keys.Length; j++)
                {
                    var keyB = keys[j];

                    var (success, path) = PathFinding.FindPath(keyLocations[keyA], keyLocations[keyB], (_, __) => 1, GetCost, current => GetAccessibleNeighbors(current, map, keys.ToUpper()));
                    if (!success)
                    {
                        continue;
                    }

                    var keysNeeded = string.Empty;
                    var keysCollected = string.Empty;
                    foreach (var (x, y) in path)
                    {
                        var tile = map[y][x];
                        if (char.IsLower(tile))
                        {
                            keysCollected += tile;
                        }
                        else if (char.IsUpper(tile))
                        {
                            keysNeeded += tile;
                        }
                    }

                    var steps = path.Count() - 1;

                    if (!keysNeeded.Contains(char.ToUpper(keyB)))
                    {
                        if (!keyPaths.TryGetValue(keyA, out var destinations))
                        {
                            destinations = new Dictionary<char, (int steps, KeyMask keysNeeded, KeyMask keysCollected, bool isTeleport)>();
                            keyPaths[keyA] = destinations;
                        }
                        keyPaths[keyA][keyB] = (steps, new KeyMask(keysNeeded), new KeyMask(keysCollected), false);
                    }
                    if (!keysNeeded.Contains(char.ToUpper(keyA)))
                    {
                        if (!keyPaths.TryGetValue(keyB, out var destinations))
                        {
                            destinations = new Dictionary<char, (int steps, KeyMask keysNeeded, KeyMask keysCollected, bool isTeleport)>();
                            keyPaths[keyB] = destinations;
                        }
                        keyPaths[keyB][keyA] = (steps, new KeyMask(keysNeeded), new KeyMask(keysCollected), false);
                    }
                }
            }
        }

        private static readonly Dictionary<(char start, KeyMask keysCollected, string teleports), (bool success, int steps, string path)> _cache = new Dictionary<(char start, KeyMask keysCollected, string teleports), (bool success, int steps, string path)>();
        private static int _indent = 0;

        private static (bool success, int steps, string pathTaken) GatherAllKeys(char start, string keys, Dictionary<char, Dictionary<char, (int steps, KeyMask keysNeeded, KeyMask keysCollected, bool isTeleport)>> keyPaths, KeyMask keysCollected, bool wasTeleport = false)
        {
            Console.WriteLine($"{new string(' ', _indent * 2)}Moving from: {start} with {keysCollected} {(wasTeleport ? "TELEPORT" : "")} Teleports: {string.Join("", keyPaths['_'].Select(kvp => kvp.Key))}");
            if (_cache.TryGetValue((start, keysCollected, string.Join("", keyPaths['_'].Select(kvp => kvp.Key))), out var cachedResult))
            {
                if (cachedResult.success)
                {
                    Console.WriteLine($"{new string(' ', _indent * 2)}Cached Returning {start} => {cachedResult.path} Steps: {cachedResult.steps}");
                }
                else
                {
                    Console.WriteLine($"{new string(' ', _indent * 2)}Cached No  Complete Paths from: {start} Keys Collected: {keysCollected}");
                }
                return (cachedResult.success, cachedResult.steps, cachedResult.path);
            }

            if (!keyPaths.ContainsKey(start))
            {
                Console.WriteLine($"{new string(' ', _indent * 2)}No paths from {start}");
                return (false, -1, string.Empty);
            }

            var minSteps = int.MaxValue;
            var minPath = string.Empty;

            _indent++;
            var potentialDestinations = keyPaths[start].Where(kvp => keysCollected.HasKeys(kvp.Value.keysNeeded) && !keysCollected.HasKey(kvp.Key));
            if (start != '_' && !wasTeleport)
            {
                potentialDestinations = potentialDestinations.Concat(keyPaths['_']);
            }
            var destinations = potentialDestinations.ToArray();

            foreach (var destination in destinations)
            {
                var destinationKeysCollected = keysCollected.AddKeys(destination.Value.keysCollected);
                Console.WriteLine($"{new string(' ', _indent * 2)}Attempting {start} => {destination.Key} Steps: {destination.Value.steps} KeysCollected: {destinationKeysCollected} {(destination.Value.isTeleport ? "TELEPORT" : "")}");

                if (destinationKeysCollected.HasKeys(keys))
                {
                    Console.WriteLine($"{new string(' ', _indent * 2)}Returning {start} => {minPath} Steps: {minSteps}");
                    _indent--;
                    return (true, destination.Value.steps, string.Join("", start, destination.Key));
                }

                var childKeyPaths = keyPaths;
                if (destination.Value.isTeleport)
                {
                    childKeyPaths = new Dictionary<char, Dictionary<char, (int steps, KeyMask keysNeeded, KeyMask keysCollected, bool isTeleport)>>();
                    foreach (var kvp in keyPaths)
                    {
                        childKeyPaths[kvp.Key] = new Dictionary<char, (int steps, KeyMask keysNeeded, KeyMask keysCollected, bool isTeleport)>(kvp.Value);
                    }
                    childKeyPaths['_'].Remove(destination.Key);
                    if (start != '_')
                    {
                        var entrance = keyPaths.Where(kvp => !char.IsLetter(kvp.Key) && kvp.Key != '_' && kvp.Value.Any(kvp2 => kvp2.Value.keysCollected.HasKey(start))).SingleOrDefault();
                        var sectionKeys = entrance.Value.Values.Select(v => v.keysCollected).Aggregate((a, b) => a.AddKeys(b));
                        if (!keysCollected.HasKeys(sectionKeys))
                        {
                            childKeyPaths['_'][start] = (0, KeyMask.Empty, new KeyMask(start), true);
                        }
                    }
                }
                var (success, steps, pathTaken) = GatherAllKeys(destination.Key, keys, childKeyPaths, destinationKeysCollected, destination.Value.isTeleport);
                if (success)
                {
                    if (steps + destination.Value.steps < minSteps)
                    {
                        minSteps = steps + destination.Value.steps;
                        minPath = pathTaken;
                    }
                }
            }
            _indent--;

            if (minSteps == int.MaxValue)
            {
                _cache[(start, keysCollected, string.Join("", keyPaths['_'].Select(kvp => kvp.Key)))] = (false, -1, string.Empty);
                Console.WriteLine($"{new string(' ', (_indent + 1) * 2)}No Complete Paths from: {start} Keys Collected: {keysCollected}");
                return (false, -1, string.Empty);
            }
            else
            {
                _cache[(start, keysCollected, string.Join("", keyPaths['_'].Select(kvp => kvp.Key)))] = (true, minSteps, start + minPath);
                Console.WriteLine($"{new string(' ', (_indent + 1) * 2)}Returning {start} => {minPath} Steps: {minSteps}");
                return (true, minSteps, start + minPath);
            }
        }

        struct KeyMask : IEquatable<KeyMask>
        {
            private readonly int _mask;

            public static KeyMask Empty { get; } = new KeyMask(0);

            public KeyMask(char key)
            {
                _mask = 1 << GetShift(key);
            }

            public KeyMask(IEnumerable<char> keys) : this(0, keys)
            {
            }

            private KeyMask(int mask, IEnumerable<char> keys)
            {
                _mask = mask;
                foreach (var key in keys)
                {
                    _mask |= 1 << GetShift(key);
                }
            }

            private KeyMask(int mask)
            {
                _mask = mask;
            }

            public KeyMask AddKeys(KeyMask keyMask)
            {
                return new KeyMask(_mask | keyMask._mask);
            }

            public bool HasKey(char key)
            {
                var value = 1 << GetShift(key);
                return (_mask & value) == value;
            }

            public bool HasKeys(KeyMask keyMask)
            {
                return (_mask & keyMask._mask) == keyMask._mask;
            }

            public bool HasKeys(IEnumerable<char> keys)
            {
                var value = 0;
                foreach (var key in keys)
                {
                    value |= 1 << GetShift(key);
                }
                return (_mask & value) == value;
            }

            public override string ToString()
            {
                var keys = string.Empty;
                for (var key = 'a'; key <= '}'; key++)
                {
                    if (HasKey(key))
                    {
                        keys += key;
                    }
                }

                if (HasKey('@'))
                {
                    keys += '@';
                }

                return keys;
            }

            public bool Equals([AllowNull] KeyMask other)
            {
                return _mask == other._mask;
            }

            private static int GetShift(char key)
            {
                var shift = char.ToLower(key) - 'a';
                if (key == '@')
                {
                    shift = 31;
                }
                return shift;
            }
        }

        private static IEnumerable<(int x, int y)> GetAccessibleNeighbors((int x, int y) current, List<string> map, string opened)
        {
            var candidates = new List<(int x, int y)>();

            if (current.x > 0)
            {
                candidates.Add((current.x - 1, current.y));
            }

            if (current.x < map[0].Length - 1)
            {
                candidates.Add((current.x + 1, current.y));
            }

            if (current.y > 0)
            {
                candidates.Add((current.x, current.y - 1));
            }

            if (current.y < map.Count - 1)
            {
                candidates.Add((current.x, current.y + 1));
            }

            var allowed = ".@{|}";
            var accessible = candidates.Where(c =>
               allowed.Contains(map[c.y][c.x]) ||
               char.IsLower(map[c.y][c.x]) ||
               (char.IsUpper(map[c.y][c.x]) && opened.Contains(map[c.y][c.x]))
            );

            return accessible;
        }

        private static int GetCost((int x, int y) a, (int x, int y) b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        private static (int x, int y) FindLocation(List<string> map, char item)
        {
            var y = map.FindIndex(s => s.Contains(item));
            var x = map[y].IndexOf(item);

            return (x, y);
        }
    }
}
