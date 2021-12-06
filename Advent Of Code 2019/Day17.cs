using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    internal static class Day17
    {
        public static int Part1(IEnumerable<string> input)
        {
            Console.CursorVisible = false;
            if (OperatingSystem.IsWindows())
            {
                Console.WindowHeight = Console.LargestWindowHeight - 3;
                Console.SetWindowPosition(0, 0);
            }

            var output = IntCodeProcessor.ProcessProgram(input);

            var map = GetMap(IntCodeProcessor.ParseProgram(input));

            var alignmentParameterSum = 0;

            var maxX = map.Max(kvp => kvp.Key.x);
            var maxY = map.Max(kvp => kvp.Key.y);

            foreach (var tile in map)
            {
                var coords = tile.Key;
                if (tile.Value == '#')
                {
                    var neighbors = GetNeighbors(coords.x, coords.y, maxX, maxY).ToArray();
                    if (neighbors.All(coords => map[(coords.x, coords.y)] == '#'))
                    {
                        var alignmentParameter = coords.x * coords.y;
                        alignmentParameterSum += alignmentParameter;
                    }
                }
            }

            DisplayMap(map);

            return alignmentParameterSum;
        }

        public static long Part2(IEnumerable<string> input)
        {
            Console.CursorVisible = false;
            if (OperatingSystem.IsWindows())
            {
                Console.WindowHeight = Console.LargestWindowHeight - 3;
                Console.SetWindowPosition(0, 0);
            }

            var program = IntCodeProcessor.ParseProgram(input);
            program.SetMemory(0, 2);

            var mapTiles = new[] { '#', '.', '^', '<', '>', 'v', '\n' };

            var inputs = "A,C,A,B,C,B,A,C,A,B\nR,6,L,10,R,8,R,8\nR,12,L,10,R,6,L,10\nR,12,L,8,L,10\ny\n".Select(c => (long)c).ToArray();
            var inputCounter = 0;

            // 0 = map
            // 1 = main routine query
            // 2 = function A query
            // 3  = function B query
            // 4 = function c query
            // 5 = video feed query
            // 6 = video feed
            var mode = 0;
            long lastOutput = 0;

            var output = new List<long>();

            void ProcessOutputToken(long outputToken)
            {
                switch (mode)
                {
                    // Map mode
                    case 0:
                        if (outputToken >= char.MinValue && outputToken <= char.MaxValue && mapTiles.Contains((char)outputToken))
                        {
                            output.Add(outputToken);
                        }
                        else
                        {
                            DisplayMap(GetMap(output.ToArray()));
                            output = new List<long>();
                            mode++;
                            ProcessOutputToken(outputToken);
                        }

                        break;
                    // Main routine query
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        Console.Write((char)outputToken);
                        if (outputToken == 10)
                        {
                            mode++;
                        }

                        break;
                    case 6:
                        if (outputToken >= char.MinValue && outputToken <= char.MaxValue && mapTiles.Contains((char)outputToken) && (lastOutput != 10 || outputToken != 10))
                        {
                            output.Add(outputToken);
                        }
                        else
                        {
                            if (output.Count > 1)
                            {
                                DisplayMap(GetMap(output.ToArray()));
                            }

                            output = new List<long>();
                        }

                        break;
                }

                lastOutput = outputToken;
            }

            foreach (var outputToken in IntCodeProcessor.ProcessProgramEnumerable(program, () =>
            {
                Console.Write((char)inputs[inputCounter]);
                return inputs[inputCounter++];
            }))
            {
                ProcessOutputToken(outputToken);
            }

            return lastOutput;
        }

        private static void DisplayMap(Dictionary<(int x, int y), char> map)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(Utility.DisplayImage(map.ToJaggedArray(kvp => (kvp.Key.x, kvp.Key.y, kvp.Value)), c => c));
        }

        private static IEnumerable<(int x, int y)> GetNeighbors(int x, int y, int maxX, int maxY)
        {
            if (y < maxY - 1)
            {
                yield return (x, y + 1);
            }

            if (y > 0)
            {
                yield return (x, y - 1);
            }

            if (x > 0)
            {
                yield return (x - 1, y);
            }

            if (x < maxX - 1)
            {
                yield return (x + 1, y);
            }
        }

        private static Dictionary<(int x, int y), char> GetMap(IntCodeProcessor.IProgramState program, long[] inputs = null)
        {
            var output = IntCodeProcessor.ProcessProgram(program, inputs);

            return GetMap(output);
        }

        private static Dictionary<(int x, int y), char> GetMap(long[] output)
        {
            var map = new Dictionary<(int x, int y), char>();

            var x = 0;
            var y = 0;
            foreach (var tile in output)
            {
                switch (tile)
                {
                    case 10:
                        y++;
                        x = 0;
                        break;
                    default:
                        map[(x, y)] = (char)tile;
                        x++;
                        break;
                }
            }

            return map;
        }
    }
}
