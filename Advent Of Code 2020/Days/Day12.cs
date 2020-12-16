using System;
using System.Collections.Generic;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day12
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(12);

            Part1(input);
            Part2(input);
        }

        private static void Part1(IEnumerable<string> input)
        {
            var currentDirection = Direction.East;
            var currentPosition = (x: 0, y: 0);

            foreach (var line in input)
            {
                var command = line[0];
                var magnitude = int.Parse(line[1..]);

                switch (command)
                {
                    case 'N':
                        currentPosition = MoveNorth(currentPosition, magnitude);
                        break;
                    case 'S':
                        currentPosition = MoveSouth(currentPosition, magnitude);
                        break;
                    case 'E':
                        currentPosition = MoveEast(currentPosition, magnitude);
                        break;
                    case 'W':
                        currentPosition = MoveWest(currentPosition, magnitude);
                        break;
                    case 'L':
                        currentDirection = TurnLeft(currentDirection, magnitude);
                        break;
                    case 'R':
                        currentDirection = TurnRight(currentDirection, magnitude);
                        break;
                    case 'F':
                        currentPosition = MoveForward(currentPosition, currentDirection, magnitude);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown command: {command}");
                }
            }

            Console.WriteLine($"Manhattan distance travelled: {Math.Abs(currentPosition.x) + Math.Abs(currentPosition.y)}");

            static (int x, int y) MoveForward((int x, int y) currentPosition, Direction currentDirection, int magnitude) => currentDirection switch
            {
                Direction.East => MoveEast(currentPosition, magnitude),
                Direction.West => MoveWest(currentPosition, magnitude),
                Direction.North => MoveNorth(currentPosition, magnitude),
                Direction.South => MoveSouth(currentPosition, magnitude),
                _ => throw new InvalidOperationException($"Unknown direction: {currentDirection}"),
            };

            static Direction TurnLeft(Direction currentDirection, int degrees) => (Direction)(((int)currentDirection - (degrees / 90) + 4) % 4);

            static Direction TurnRight(Direction currentDirection, int degrees) => (Direction)(((int)currentDirection + (degrees / 90)) % 4);
        }

        private static void Part2(IEnumerable<string> input)
        {
            var shipPosition = (x: 0, y: 0);
            var waypointPosition = (x: 10, y: -1);

            foreach (var line in input)
            {
                var command = line[0];
                var magnitude = int.Parse(line[1..]);

                switch (command)
                {
                    case 'N':
                        waypointPosition = MoveNorth(waypointPosition, magnitude);
                        break;
                    case 'S':
                        waypointPosition = MoveSouth(waypointPosition, magnitude);
                        break;
                    case 'E':
                        waypointPosition = MoveEast(waypointPosition, magnitude);
                        break;
                    case 'W':
                        waypointPosition = MoveWest(waypointPosition, magnitude);
                        break;
                    case 'L':
                        waypointPosition = RotateWaypointLeft(waypointPosition, magnitude);
                        break;
                    case 'R':
                        waypointPosition = RotateWaypointRight(waypointPosition, magnitude);
                        break;
                    case 'F':
                        shipPosition = MoveToWaypoint(shipPosition, waypointPosition, magnitude);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown command: {command}");
                }
            }

            Console.WriteLine($"Manhattan distance travelled: {Math.Abs(shipPosition.x) + Math.Abs(shipPosition.y)}");

            static (int x, int y) MoveToWaypoint((int x, int y) shipPosition, (int x, int y) waypointPosition, int magnitude) => (shipPosition.x + (waypointPosition.x * magnitude), shipPosition.y + (waypointPosition.y * magnitude));

            static (int x, int y) RotateWaypointRight((int x, int y) waypointPosition, int magnitude) => magnitude switch
            {
                90 => (-waypointPosition.y, waypointPosition.x),
                180 => (-waypointPosition.x, -waypointPosition.y),
                270 => (waypointPosition.y, -waypointPosition.x),
                _ => throw new InvalidOperationException($"Unknown turn degrees: {magnitude}"),
            };

            static (int x, int y) RotateWaypointLeft((int x, int y) waypointPosition, int magnitude) => magnitude switch
            {
                90 => (waypointPosition.y, -waypointPosition.x),
                180 => (-waypointPosition.x, -waypointPosition.y),
                270 => (-waypointPosition.y, waypointPosition.x),
                _ => throw new InvalidOperationException($"Unknown turn degrees: {magnitude}"),
            };
        }

        private static (int x, int y) MoveNorth((int x, int y) currentPosition, int magnitude) => (currentPosition.x, currentPosition.y - magnitude);

        private static (int x, int y) MoveSouth((int x, int y) currentPosition, int magnitude) => (currentPosition.x, currentPosition.y + magnitude);

        private static (int x, int y) MoveEast((int x, int y) currentPosition, int magnitude) => (currentPosition.x + magnitude, currentPosition.y);

        private static (int x, int y) MoveWest((int x, int y) currentPosition, int magnitude) => (currentPosition.x - magnitude, currentPosition.y);

        public enum Direction
        {
            East,
            South,
            West,
            North
        }
    }
}
