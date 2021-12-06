using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace Advent_Of_Code_2019
{
    internal static class Day13
    {
        private enum TileType
        {
            Empty = 0,
            Wall = 1,
            Block = 2,
            Paddle = 3,
            Ball = 4
        }

        private class GameObject
        {
            public Point Position { get; }
            public TileType Type { get; }

            public GameObject(Point position, TileType type)
            {
                Position = position;
                Type = type;
            }
        }

        public static int Part1(IEnumerable<string> input)
        {
            var gameObjects = InitializeGame(input).ToList();

            var display = gameObjects.ToJaggedArray(gameObject => (gameObject.Position.X, gameObject.Position.Y, gameObject.Type));

            Console.CursorTop = 0;
            Console.CursorLeft = 0;
            Console.WriteLine(Utility.DisplayImage(display, type =>
            {
                return type switch
                {
                    TileType.Ball => 'o',
                    TileType.Block => '█',
                    TileType.Empty => ' ',
                    TileType.Paddle => '▄',
                    TileType.Wall => '▌',
                    _ => throw new Exception($"Unknown TileType: {type}"),
                };
            }));

            return gameObjects.Count(go => go.Type == TileType.Block);
        }

        public static long Part2(IEnumerable<string> input)
        {
            var program = IntCodeProcessor.ParseProgram(input);
            program.SetMemory(0, 2);

            var ballPosition = Point.Empty;
            var paddlePosition = Point.Empty;
            var leftWallX = int.MaxValue;
            var rightWallX = int.MinValue;
            var started = false;
            Func<long> inputHandler = () =>
            {
                started = true;
                if (paddlePosition.X < ballPosition.X && ballPosition.X < rightWallX - 1)
                {
                    return 1;
                }
                else if (paddlePosition.X > ballPosition.X && ballPosition.X > leftWallX)
                {
                    return -1;
                }

                return 0;
            };

            void DrawObject(int x, int y, char obj)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(obj);
            }

            Console.CursorVisible = false;
            var game = IntCodeProcessor.ProcessProgramEnumerable(program, inputHandler).GetEnumerator();
            var score = 0L;
            bool more;
            do
            {
                more = game.MoveNext();
                var x = (int)game.Current;
                more = more && game.MoveNext();
                var y = (int)game.Current;
                more = more && game.MoveNext();
                if (score > 0 && x == score && y == score)
                {
                    break;
                }

                if (x == -1 && y == 0)
                {
                    score = game.Current;
                }
                else
                {
                    var tile = (TileType)game.Current;

                    DrawObject(x, y, MapTile(tile, x, y));

                    if (tile == TileType.Ball)
                    {
                        ballPosition = new Point(x, y);
                    }
                    else if (tile == TileType.Paddle)
                    {
                        paddlePosition = new Point(x, y);
                    }
                    else if (tile == TileType.Wall)
                    {
                        leftWallX = Math.Min(x, leftWallX);
                        rightWallX = Math.Max(x, rightWallX);
                    }
                }

                if (started)
                {
                    Thread.Sleep(1);
                }
            } while (more);

            return score;
        }

        private static char MapTile(TileType type, int x, int y) => type switch
        {
            TileType.Ball => 'o',
            TileType.Block => '█',
            TileType.Empty => ' ',
            TileType.Paddle => '▄',
            TileType.Wall => y == 0 ? '▄' : x == 0 ? '▌' : '▐',
            _ => throw new Exception($"Unknown TileType: {type}"),
        };

        private static IEnumerable<GameObject> InitializeGame(IEnumerable<string> input)
        {
            var program = IntCodeProcessor.ProcessProgramEnumerable(input).GetEnumerator();
            bool more;
            do
            {
                _ = program.MoveNext();
                var x = (int)program.Current;
                _ = program.MoveNext();
                var y = (int)program.Current;
                more = program.MoveNext();
                var tile = (TileType)program.Current;

                yield return new GameObject(new Point(x, y), tile);
            } while (more);
        }
    }
}
