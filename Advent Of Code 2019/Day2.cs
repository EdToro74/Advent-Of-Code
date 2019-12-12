using System.Collections.Generic;

namespace Advent_Of_Code_2019
{
    static class Day2
    {
        public static long Part1(IEnumerable<string> input)
        {
            var programState = IntCodeProcessor.ParseProgram(input);

            programState.SetMemory(1, 12);
            programState.SetMemory(2, 2);

            IntCodeProcessor.ProcessProgram(programState);

            return programState.GetMemory(0);
        }

        public static long Part2(IEnumerable<string> input)
        {
            var program = IntCodeProcessor.ParseProgram(input);

            for (int noun = 0; noun < 100; noun++)
            {
                for (int verb = 0; verb < 100; verb++)
                {
                    var copy = IntCodeProcessor.CopyProgram(program);

                    copy.SetMemory(1, noun);
                    copy.SetMemory(2, verb);

                    IntCodeProcessor.ProcessProgram(copy);
                    if (copy.GetMemory(0) == 19690720)
                    {
                        return 100 * noun + verb;
                    }
                }
            }

            return 0;
        }
    }
}
