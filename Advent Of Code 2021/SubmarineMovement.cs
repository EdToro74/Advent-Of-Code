using System.Collections.Generic;

namespace Advent_Of_Code_2021
{
    public static class SubmarineMovement
    {
        public static (int horizontal, int depth) MoveBasic(IEnumerable<string> commands)
        {
            var horizontal = 0;
            var depth = 0;

            foreach (var command in commands)
            {
                if (command.StartsWith("forward"))
                {
                    horizontal += int.Parse(command.Split(' ')[1]);
                }
                else if (command.StartsWith("up"))
                {
                    depth -= int.Parse(command.Split(' ')[1]);
                }
                else if (command.StartsWith("down"))
                {
                    depth += int.Parse(command.Split(' ')[1]);
                }
            }

            return (horizontal, depth);
        }

        public static (int horizontal, int depth) Move(IEnumerable<string> commands)
        {
            var horizontal = 0;
            var depth = 0;
            var aim = 0;

            foreach (var command in commands)
            {
                if (command.StartsWith("forward"))
                {
                    var units = int.Parse(command.Split(' ')[1]);
                    horizontal += units;
                    depth += aim * units;
                }
                else if (command.StartsWith("up"))
                {
                    aim -= int.Parse(command.Split(' ')[1]);
                }
                else if (command.StartsWith("down"))
                {
                    aim += int.Parse(command.Split(' ')[1]);
                }
            }

            return (horizontal, depth);
        }
    }
}
