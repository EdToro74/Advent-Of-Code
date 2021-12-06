using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day11
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(11);

            var automata = SeatAutomata.Parse(input);

            var done = false;
            while (!done)
            {
                var next = automata.Step(Step1StepFunction);
                done = !next.changed;
                automata = next.next;
            }

            Console.WriteLine($"Part 1: {automata.TakenSeatCount}");

            automata = SeatAutomata.Parse(input);

            done = false;
            while (!done)
            {
                var next = automata.Step(Step2StepFunction);
                done = !next.changed;
                automata = next.next;
            }

            Console.WriteLine($"Part 2: {automata.TakenSeatCount}");
        }

        private static bool Step1StepFunction(int seatIndex, int width, int height, HashSet<int> takenIndices, HashSet<int> emptySeatIndices)
        {
            var columnIndex = seatIndex % width;

            var takenNeighborCount =
                // Top Left
                (columnIndex > 0 && takenIndices.Contains(seatIndex - (width + 1)) ? 1 : 0) +
                // Top
                (takenIndices.Contains(seatIndex - width) ? 1 : 0) +
                // Top Right
                (columnIndex < width - 1 && takenIndices.Contains(seatIndex - (width - 1)) ? 1 : 0) +
                // Left
                (columnIndex > 0 && takenIndices.Contains(seatIndex - 1) ? 1 : 0) +
                // Right
                (columnIndex < width - 1 && takenIndices.Contains(seatIndex + 1) ? 1 : 0) +
                // Bottom Left
                (columnIndex > 0 && takenIndices.Contains(seatIndex + (width - 1)) ? 1 : 0) +
                // Bottom
                (takenIndices.Contains(seatIndex + width) ? 1 : 0) +
                // Bottom Right
                (columnIndex < width - 1 && takenIndices.Contains(seatIndex + (width + 1)) ? 1 : 0);

            if (takenIndices.Contains(seatIndex))
            {
                return takenNeighborCount < 4;
            }
            else
            {
                return takenNeighborCount == 0;
            }
        }

        private static bool Step2StepFunction(int seatIndex, int width, int height, HashSet<int> takenIndices, HashSet<int> emptySeatIndices)
        {
            bool FirstSeatIsTaken(int xDelta, int yDelta)
            {
                var columnIndex = seatIndex % width;
                var rowIndex = seatIndex / width;

                columnIndex += xDelta;
                rowIndex += yDelta;

                while (columnIndex > -1 && columnIndex < width && rowIndex > -1 && rowIndex < height)
                {
                    var currentIndex = rowIndex * width + columnIndex;

                    if (takenIndices.Contains(currentIndex))
                    {
                        return true;
                    }
                    else if (emptySeatIndices.Contains(currentIndex))
                    {
                        return false;
                    }

                    columnIndex += xDelta;
                    rowIndex += yDelta;
                }

                return false;
            }

            var takenNeighborCount =
                // Top Left
                (FirstSeatIsTaken(-1, -1) ? 1 : 0) +
                // Top
                (FirstSeatIsTaken(0, -1) ? 1 : 0) +
                // Top Right
                (FirstSeatIsTaken(1, -1) ? 1 : 0) +
                // Left
                (FirstSeatIsTaken(-1, 0) ? 1 : 0) +
                // Right
                (FirstSeatIsTaken(1, 0) ? 1 : 0) +
                // Bottom Left
                (FirstSeatIsTaken(-1, 1) ? 1 : 0) +
                // Bottom
                (FirstSeatIsTaken(0, 1) ? 1 : 0) +
                // Bottom Right
                (FirstSeatIsTaken(1, 1) ? 1 : 0);

            if (takenIndices.Contains(seatIndex))
            {
                return takenNeighborCount < 5;
            }
            else
            {
                return takenNeighborCount == 0;
            }
        }

        private class SeatAutomata
        {
            public int _width;
            public int _height;
            public HashSet<int> _takenIndices = new HashSet<int>();
            public HashSet<int> _emptySeatIndices = new HashSet<int>();

            public int TakenSeatCount => _takenIndices.Count;

            private SeatAutomata(int width, int height, HashSet<int> takenIndices, HashSet<int> emptySeatIndices)
            {
                _width = width;
                _height = height;
                _takenIndices = takenIndices;
                _emptySeatIndices = emptySeatIndices;
            }

            public static SeatAutomata Parse(IEnumerable<string> grid)
            {
                var takenIndices = new HashSet<int>();
                var emptySeatIndices = new HashSet<int>();

                var width = grid.First().Length;
                var height = grid.Count();

                var rowIndex = 0;
                foreach (var row in grid)
                {
                    var columnIndex = 0;
                    foreach (var cell in row)
                    {
                        var cellIndex = columnIndex + (width * rowIndex);
                        switch (cell)
                        {
                            case 'L':
                                _ = emptySeatIndices.Add(cellIndex);
                                break;
                            case '#':
                                _ = takenIndices.Add(cellIndex);
                                break;
                            case '.':
                                break;
                            default:
                                throw new InvalidOperationException($"Unknown cell type: {cell}");
                        }

                        columnIndex++;
                    }

                    rowIndex++;
                }

                return new SeatAutomata(width, height, takenIndices, emptySeatIndices);
            }

            public (bool changed, SeatAutomata next) Step(Func<int, int, int, HashSet<int>, HashSet<int>, bool> stepFunction)
            {
                var nextTakenIndices = new HashSet<int>();
                var nextEmptySeatIndices = new HashSet<int>();

                foreach (var seatIndex in _takenIndices)
                {
                    if (stepFunction(seatIndex, _width, _height, _takenIndices, _emptySeatIndices))
                    {
                        _ = nextTakenIndices.Add(seatIndex);
                    }
                    else
                    {
                        _ = nextEmptySeatIndices.Add(seatIndex);
                    }
                }

                foreach (var seatIndex in _emptySeatIndices)
                {
                    if (stepFunction(seatIndex, _width, _height, _takenIndices, _emptySeatIndices))
                    {
                        _ = nextTakenIndices.Add(seatIndex);
                    }
                    else
                    {
                        _ = nextEmptySeatIndices.Add(seatIndex);
                    }
                }

                var changed = !nextTakenIndices.SequenceEqual(_takenIndices) || !nextEmptySeatIndices.SequenceEqual(_emptySeatIndices);
                return (changed, new SeatAutomata(_width, _height, nextTakenIndices, nextEmptySeatIndices));
            }

            public string Draw()
            {
                var sb = new StringBuilder();
                for (var row = 0; row < _height; row++)
                {
                    for (var column = 0; column < _width; column++)
                    {
                        var seatIndex = (row * _width) + column;
                        if (_emptySeatIndices.Contains(seatIndex))
                        {
                            _ = sb.Append('L');
                        }
                        else if (_takenIndices.Contains(seatIndex))
                        {
                            _ = sb.Append('#');
                        }
                        else
                        {
                            _ = sb.Append('.');
                        }
                    }

                    _ = sb.AppendLine();
                }

                return sb.ToString();
            }
        }
    }
}
