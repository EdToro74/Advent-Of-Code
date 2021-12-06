using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2021.Days
{
    internal class Day04
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(4);

            var calledNumbers = input.First().Split(',').Select(int.Parse);

            var boards = new List<(int number, bool called)[,]>();

            (int number, bool called)[,] current = null;
            var row = 0;
            foreach (var line in input.Skip(1))
            {
                if (line == string.Empty)
                {
                    if (current != null)
                    {
                        boards.Add(current);
                    }

                    current = new (int number, bool called)[5, 5];
                    row = 0;
                    continue;
                }

                foreach (var item in line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select((n, index) => (number: int.Parse(n), index)))
                {
                    current.SetValue((item.number, false), item.index, row);
                }

                row++;
            }

            if (current != null)
            {
                boards.Add(current);
            }

            Part1(boards, calledNumbers);
            Part2(boards, calledNumbers);
        }

        private static void Part1(List<(int number, bool called)[,]> boards, IEnumerable<int> calledNumbers) => PlayBingo(boards, calledNumbers, (board, _, calledNumber) =>
                                                                                                              {
                                                                                                                  var score = CalculateScore(board, calledNumber);
                                                                                                                  Console.WriteLine($"Part 1: {score}");
                                                                                                                  return true;
                                                                                                              });

        private static void Part2(List<(int number, bool called)[,]> boards, IEnumerable<int> calledNumbers)
        {
            var completed = new HashSet<int>();
            PlayBingo(boards, calledNumbers, (board, boardIndex, calledNumber) =>
            {
                _ = completed.Add(boardIndex);
                if (completed.Count == boards.Count)
                {
                    var score = CalculateScore(board, calledNumber);
                    Console.WriteLine($"Part 2: {score}");
                    return true;
                }

                return false;
            });
        }

        private static void PlayBingo(List<(int number, bool called)[,]> boards, IEnumerable<int> calledNumbers, Func<(int number, bool called)[,], int, int, bool> winHandler)
        {
            foreach (var calledNumber in calledNumbers)
            {
                var boardIndex = 0;
                foreach (var board in boards)
                {
                    for (var y = 0; y < board.GetLength(1); y++)
                    {
                        for (var x = 0; x < board.GetLength(0); x++)
                        {
                            if (board[x, y].number == calledNumber)
                            {
                                board[x, y] = (calledNumber, true);
                                if (CheckRow(board, y) || CheckColumn(board, x))
                                {
                                    if (winHandler(board, boardIndex, calledNumber))
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    boardIndex++;
                }
            }

            bool CheckRow((int number, bool called)[,] board, int row)
            {
                for (var x = 0; x < board.GetLength(0); x++)
                {
                    if (!board[x, row].called)
                    {
                        return false;
                    }
                }

                return true;
            }

            bool CheckColumn((int number, bool called)[,] board, int column)
            {
                for (var y = 0; y < board.GetLength(1); y++)
                {
                    if (!board[column, y].called)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private static int CalculateScore((int number, bool called)[,] board, int lastCalledNumber)
        {
            var uncalledSum = 0;
            foreach (var value in board)
            {
                if (!value.called)
                {
                    uncalledSum += value.number;
                }
            }

            return uncalledSum * lastCalledNumber;
        }
    }
}
