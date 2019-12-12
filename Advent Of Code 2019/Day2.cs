using System.Collections.Generic;

namespace Advent_Of_Code_2019
{
    static class Day2
    {
        public static int Part1(IEnumerable<string> input)
        {
            var program = IntCodeProcessor.ParseProgram(input);

            program[1] = 12;
            program[2] = 2;

            IntCodeProcessor.ProcessProgram(program);
            
            return program[0];
        }

        public static int Part2(IEnumerable<string> input)
        {
            var program = IntCodeProcessor.ParseProgram(input);

            for (int noun = 0; noun < 100; noun++)
            {
                for (int verb = 0; verb < 100; verb++)
                {
                    var copy = IntCodeProcessor.CopyProgram(program);

                    copy[1] = noun;
                    copy[2] = verb;

                    IntCodeProcessor.ProcessProgram(copy);
                    if (copy[0] == 19690720)
                    {
                        return 100 * noun + verb;
                    }
                }
            }

            return 0;
        }
    }
}
