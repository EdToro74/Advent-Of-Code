using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day20
    {
        public static void Run()
        {
            var input = new StringReader(string.Join(Environment.NewLine, Utility.Utility.GetDayFile(20)));

            var tiles = new List<Tile>();

            while (input.Peek() != -1)
            {
                var tile = Tile.Parse(input);
                tiles.Add(tile);
            }

            var gridSize = (int)Math.Sqrt(tiles.Count);

            var preCalculatedNeighbors = PreCalculateNeighbors(tiles);

            var solutions = GetSolutions(tiles.ToImmutableList(), preCalculatedNeighbors, gridSize);

            var part1Solution = solutions.Skip(1).First();

            Console.WriteLine($"Part 1: {(long)part1Solution[0].tile.Id * part1Solution[gridSize - 1].tile.Id * part1Solution[(gridSize - 1) * gridSize].tile.Id * part1Solution[gridSize * gridSize - 1].tile.Id}");

            var tileSize = tiles[0].Size;

            var image = Tile.Combine(part1Solution.Select(item => item.tile.Arrangements[item.arrangement]).ToList(), tileSize, gridSize);

            var dragon = new[]
            {
                new [] {1, 4, 7, 10, 13, 16},
                new [] {0, 5, 6, 11, 12, 17, 18, 19},
                new [] {18}
            };

            foreach (var arrangement in image.Arrangements)
            {
                var dragonCount = 0;
                var dragonTiles = new HashSet<int>();

                for (var y = image.Size - 1; y >= dragon.Length - 1; y--)
                {
                    for (var x = 0; x < image.Size - dragon.Max(d => d.Max()); x++)
                    {
                        var index = y * image.Size + x;
                        if (MatchPattern(arrangement, index, dragon[0]))
                        {
                            var foundDragon = true;
                            for (var dragonIndex = 1; dragonIndex < dragon.Length; dragonIndex++)
                            {
                                if (!MatchPattern(arrangement, index - image.Size * dragonIndex, dragon[dragonIndex]))
                                {
                                    foundDragon = false;
                                    break;
                                }
                            }

                            if (foundDragon)
                            {
                                dragonCount++;
                                for (var dragonIndex = 0; dragonIndex < dragon.Length; dragonIndex++)
                                {
                                    foreach (var offset in dragon[dragonIndex])
                                    {
                                        dragonTiles.Add(index - (image.Size * dragonIndex) + offset);
                                    }
                                }
                            }
                        }
                    }
                }

                if (dragonTiles.Any())
                {
                    Console.WriteLine($"Water roughness: {arrangement.Except(dragonTiles).Count()}");
                }
            }
        }

        private static bool MatchPattern(HashSet<int> arrangement, int startIndex, int[] offsets) => offsets.All(offset => arrangement.Contains(startIndex + offset));

        private static Dictionary<(int tileId, int arrangement), (IEnumerable<(int tileId, int arrangement)> topNeighbors, IEnumerable<(int tileId, int arrangement)> leftNeighbors)> PreCalculateNeighbors(List<Tile> tiles)
        {
            var cache = new Dictionary<(int tileId, int arrangement), (IEnumerable<(int tileId, int arrangement)> topNeighbors, IEnumerable<(int tileId, int arrangement)> leftNeighbors)>();

            foreach (var tile in tiles)
            {
                for (var i = 0; i < tile.Arrangements.Count; i++)
                {
                    var topNeighbors = new List<(int tileId, int arrangement)>();
                    var leftNeighbors = new List<(int tileId, int arrangement)>();

                    cache[(tile.Id, i)] = (topNeighbors, leftNeighbors);

                    foreach (var potentialNeighbor in tiles)
                    {
                        if (potentialNeighbor == tile)
                        {
                            continue;
                        }

                        for (var j = 0; j < tile.Arrangements.Count; j++)
                        {
                            if (CanFitTop(tile.Arrangements[i], potentialNeighbor.Arrangements[j], tile.Size))
                            {
                                topNeighbors.Add((potentialNeighbor.Id, j));
                            }
                            if (CanFitLeft(tile.Arrangements[i], potentialNeighbor.Arrangements[j], tile.Size))
                            {
                                leftNeighbors.Add((potentialNeighbor.Id, j));
                            }
                        }
                    }
                }
            }

            return cache;
        }

        private static IEnumerable<ImmutableList<(Tile tile, int arrangement)>> GetSolutions(ImmutableList<Tile> availableTiles, Dictionary<(int tileId, int arrangement), (IEnumerable<(int tileId, int arrangement)> topNeighbors, IEnumerable<(int tileId, int arrangement)> leftNeighbors)> preCalculatedNeighbors, int gridSize)
        {
            var workingGrid = new List<(Tile tile, int)>(Enumerable.Range(0, gridSize * gridSize).Select(_ => ((Tile)null, -1))).ToImmutableList();

            var potentialSolutions = new List<(ImmutableList<Tile> availableTiles, ImmutableList<(Tile tile, int arrangement)> workingGrid)>(
                availableTiles.SelectMany(tile => Enumerable.Range(0, tile.Arrangements.Count).Select(arrangement => (availableTiles.Remove(tile), workingGrid.SetItem(0, (tile, arrangement)))))
            );

            for (var i = 1; i < gridSize * gridSize; i++)
            {
                var nextPotentialSolutions = new List<(ImmutableList<Tile> availableTiles, ImmutableList<(Tile tile, int arrangement)> workingGrid)>();

                foreach (var potentialSolution in potentialSolutions)
                {
                    var next = GetMatchingArrangements(potentialSolution.availableTiles, i, gridSize, preCalculatedNeighbors, potentialSolution.workingGrid).ToList();
                    if (next.Any())
                    {
                        nextPotentialSolutions.AddRange(next.Select(item => (potentialSolution.availableTiles.Remove(item.tile), potentialSolution.workingGrid.SetItem(i, (item.tile, item.arrangement)))));
                    }
                }

                potentialSolutions = nextPotentialSolutions;
            }

            // Make sure all solutions are flips/rotations of eachother
            _ = potentialSolutions.Select(solution => (long)solution.workingGrid[0].tile.Id * solution.workingGrid[gridSize - 1].tile.Id * solution.workingGrid[(gridSize - 1) * gridSize].tile.Id * solution.workingGrid[gridSize * gridSize - 1].tile.Id).Distinct().Single();

            return potentialSolutions.Select(potentialSolution => potentialSolution.workingGrid);
        }

        private static IEnumerable<(Tile tile, int arrangement)> GetMatchingArrangements(ImmutableList<Tile> availableTiles, int gridIndex, int gridSize, Dictionary<(int tileId, int arrangement), (IEnumerable<(int tileId, int arrangement)> topNeighbors, IEnumerable<(int tileId, int arrangement)> leftNeighbors)> preCalculatedNeighbors, ImmutableList<(Tile tile, int arrangement)> placedTiles)
        {
            var x = gridIndex % gridSize;
            var y = gridIndex / gridSize;

            var topNeighbor = y > 0 ? placedTiles[gridIndex - gridSize] : (null, -1);
            var leftNeighbor = x > 0 ? placedTiles[gridIndex - 1] : (null, -1);

            foreach (var availableTile in availableTiles)
            {
                for (var i = 0; i < availableTile.Arrangements.Count; i++)
                {
                    if (
                        (topNeighbor.tile == null || preCalculatedNeighbors[(availableTile.Id, i)].topNeighbors.Contains((topNeighbor.tile.Id, topNeighbor.arrangement))) &&
                        (leftNeighbor.tile == null || preCalculatedNeighbors[(availableTile.Id, i)].leftNeighbors.Contains((leftNeighbor.tile.Id, leftNeighbor.arrangement)))
                    )
                    {
                        yield return (availableTile, i);
                    }
                }
            }
        }

        private static bool CanFitTop(HashSet<int> arrangement, HashSet<int> neighbor, int tileSize)
        {
            var offset = (tileSize - 1) * tileSize;

            var topRow = arrangement.Where(cell => cell / tileSize == 0).ToHashSet();
            var neightborbottomRow = neighbor.Where(cell => cell / tileSize == tileSize - 1).Select(cell => cell - offset).ToHashSet();

            return topRow.SetEquals(neightborbottomRow);
        }

        private static bool CanFitLeft(HashSet<int> arrangement, HashSet<int> neighbor, int tileSize)
        {
            var leftColumn = arrangement.Where(cell => cell % tileSize == 0).Select(cell => cell / tileSize).OrderBy(i => i);
            var neightborRightColumn = neighbor.Where(cell => cell % tileSize == tileSize - 1).Select(cell => cell / tileSize).OrderBy(i => i);

            return leftColumn.SequenceEqual(neightborRightColumn);
        }

        private static void DisplayGrid(ImmutableList<(Tile tile, int arrangement)> grid, int gridSize)
        {
            var tileSize = grid.First().tile.Size;

            for (var y = 0; y < gridSize; y++)
            {
                for (var tileY = 0; tileY < tileSize; tileY++)
                {
                    var line = string.Empty;
                    for (var x = 0; x < gridSize; x++)
                    {
                        var gridIndex = y * gridSize + x;
                        var gridCell = grid[gridIndex];

                        line += string.Join("", Enumerable.Range(0, tileSize).Select(cellIndex => gridCell.tile.Arrangements[gridCell.arrangement].Contains(cellIndex + (tileY * tileSize)) ? '#' : '.')) + " ";
                    }

                    Console.WriteLine(line);
                }

                Console.WriteLine();
            }

            for (var y = 0; y < gridSize; y++)
            {
                var line = string.Empty;
                for (var x = 0; x < gridSize; x++)
                {
                    var index = y * gridSize + x;
                    var tile = grid[index];

                    line += tile.tile.Id + " ";
                }

                Console.WriteLine(line);
            }
        }

        class Tile
        {
            public int Id { get; init; }
            public int Size { get; init; }

            public IList<HashSet<int>> Arrangements { get; init; }

            private static readonly Regex _header = new Regex("^Tile (?<id>[0-9]+):$");
            private static readonly Regex _line = new Regex("^[.#]+$");

            public static Tile Parse(StringReader input)
            {
                var id = int.Parse(_header.Match(input.ReadLine()).Groups["id"].Value);

                var size = 0;
                var index = 0;
                var setCells = new HashSet<int>();

                string line;
                while ((line = input.ReadLine()) != string.Empty)
                {
                    if (line == null)
                    {
                        break;
                    }

                    if (size == 0)
                    {
                        size = line.Length;
                    }
                    else if (size != line.Length)
                    {
                        throw new InvalidOperationException($"Inconsistent line sizes for tile {id}, first line was {size} cells, current line is {line.Length}");
                    }
                    else if (!_line.IsMatch(line))
                    {
                        throw new InvalidOperationException($"Invalid line: {line}");
                    }

                    foreach (var setCell in line.Select((c, i) => (c, i)).Where(item => item.c == '#').Select(item => index + item.i))
                    {
                        setCells.Add(setCell);
                    }
                    index += size;
                }

                var flipped = Flip(setCells, size);

                return new Tile
                {
                    Id = id,
                    Size = size,
                    Arrangements = new HashSet<int>[]
                    {
                        setCells,
                        Rotate90(setCells, size),
                        Rotate180(setCells, size),
                        Rotate270(setCells, size),
                        flipped,
                        Rotate90(flipped, size),
                        Rotate180(flipped, size),
                        Rotate270(flipped, size),
                    }
                };
            }

            public static Tile Combine(IList<HashSet<int>> tiles, int tileSize, int gridSize)
            {
                var combined = new HashSet<int>();

                for (var gridY = 0; gridY < gridSize; gridY++)
                {
                    for (var gridX = 0; gridX < gridSize; gridX++)
                    {
                        var tile = tiles[gridY * gridSize + gridX];

                        for (var tileY = 1; tileY < tileSize - 1; tileY++)
                        {
                            for (var tileX = 1; tileX < tileSize - 1; tileX++)
                            {
                                if (tile.Contains(tileY * tileSize + tileX))
                                {
                                    var combinedX = (tileSize - 2) * gridX + (tileX - 1);
                                    var combinedY = (tileSize - 2) * gridY + (tileY - 1);

                                    var combinedIndex = combinedY * (tileSize - 2) * gridSize + combinedX;
                                    if (!combined.Add(combinedIndex)) throw new InvalidOperationException();
                                }
                            }
                        }
                    }
                }

                var combinedSize = (tileSize - 2) * gridSize;
                var flipped = Flip(combined, combinedSize);

                return new Tile
                {
                    Size = combinedSize,
                    Arrangements = new HashSet<int>[]
                    {
                        combined,
                        Rotate90(combined, combinedSize),
                        Rotate180(combined, combinedSize),
                        Rotate270(combined, combinedSize),
                        flipped,
                        Rotate90(flipped, combinedSize),
                        Rotate180(flipped, combinedSize),
                        Rotate270(flipped, combinedSize),
                    }
                };
            }

            private static HashSet<int> Flip(HashSet<int> setCells, int size) => setCells.Select(cell => (size - 1 - cell / size) * size + (cell % size)).ToHashSet();

            private static HashSet<int> Rotate270(HashSet<int> setCells, int size) => setCells.Select(cell => (size - 1 - cell % size) * size + (cell / size)).ToHashSet();

            private static HashSet<int> Rotate180(HashSet<int> setCells, int size) => setCells.Select(cell => (size - 1 - cell / size) * size + (size - 1 - cell % size)).ToHashSet();

            private static HashSet<int> Rotate90(HashSet<int> setCells, int size) => setCells.Select(cell => cell % size * size + (size - 1 - cell / size)).ToHashSet();

            public List<string> Draw(int arrangement) => Tile.Draw(Arrangements[arrangement], Size);

            internal static List<string> Draw(HashSet<int> setCells, int size)
            {
                var lines = new List<string>();

                for (var y = 0; y < size; y++)
                {
                    lines.Add(string.Join("", Enumerable.Range(0, size).Select(x => y * size + x).Select(index => setCells.Contains(index) ? '#' : '.')));
                }

                return lines;
            }

            public override string ToString() => $"Id: {Id}";
        }
    }
}