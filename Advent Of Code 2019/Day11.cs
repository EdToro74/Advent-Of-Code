using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    static class Day11
    {
        public static int Part1(IEnumerable<string> input)
        {
            return RunPaintingRobot(input, Color.Black).Count;
        }

        public static string Part2(IEnumerable<string> input)
        {
            var coloredSquares = RunPaintingRobot(input, Color.White);

            var minX = coloredSquares.Keys.Min(c => c.x);
            var maxX = coloredSquares.Keys.Max(c => c.x);
            var xOffset = maxX - minX;

            var minY = coloredSquares.Keys.Min(c => c.y);
            var maxY = coloredSquares.Keys.Max(c => c.y);
            var yOffset = maxY - minY;

            var hull = new int[maxY + yOffset + 1, maxX + xOffset + 1];

            foreach (var coloredSquare in coloredSquares)
            {
                hull[coloredSquare.Key.y + yOffset, coloredSquare.Key.x + xOffset] = (int)coloredSquare.Value;
            }

            return SpaceImageFormat.DisplayImage(hull);
        }

        private static Dictionary<(int x, int y), Color> RunPaintingRobot(IEnumerable<string> input, Color initialColor)
        {
            var state = (
                Position: (x: 0, y: 0),
                Direction: Direction.Up,
                OutputState: OutputState.Color
            );

            var colors = new Dictionary<(int x, int y), Color>();
            colors[(0, 0)] = initialColor;

            Func<long> inputHandler = () =>
            {
                colors.TryGetValue(state.Position, out var color);

                return (long)color;
            };

            foreach (var output in IntCodeProcessor.ProcessProgramEnumerable(input, inputHandler))
            {
                switch (state.OutputState)
                {
                    case OutputState.Color:
                        colors[state.Position] = (Color)output;
                        state.OutputState = OutputState.Direction;
                        break;
                    case OutputState.Direction:
                        var delta = 0;
                        if (output == 0)
                        {
                            delta = -1;
                        }
                        else if (output == 1)
                        {
                            delta = 1;
                        }
                        else
                        {
                            throw new Exception($"Unknown output: {output}");
                        }

                        state.Direction = (Direction)((int)(state.Direction + delta + 4) % 4);

                        switch (state.Direction)
                        {
                            case Direction.Up:
                                state.Position.y--;
                                break;
                            case Direction.Left:
                                state.Position.x--;
                                break;
                            case Direction.Down:
                                state.Position.y++;
                                break;
                            case Direction.Right:
                                state.Position.x++;
                                break;
                        }
                        state.OutputState = OutputState.Color;
                        break;
                    default:
                        throw new Exception($"Uknown OutputState: {state.OutputState}");
                }
            }

            return colors;
        }

        enum Direction
        {
            Up,
            Right,
            Down,
            Left
        }

        enum OutputState
        {
            Color,
            Direction
        }

        enum Color
        {
            Black = 0,
            White = 1
        }
    }
}
