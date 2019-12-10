using System;

namespace Advent_Of_Code_2019
{
    static class IntCodeProcessor
    {
        public static void ProcessProgram(int[] program)
        {
            var instructionPointer = 0;

            while (true)
            {
                var instruction = program[instructionPointer];
                if (instruction == 99)
                {
                    return;
                }

                var parameters = GetParameters(program, instruction, instructionPointer);

                switch (instruction)
                {
                    case 1:
                        program[parameters[2]] = program[parameters[0]] + program[parameters[1]];
                        break;
                    case 2:
                        program[parameters[2]] = program[parameters[0]] * program[parameters[1]];
                        break;
                    default:
                        throw new Exception($"Invalid opCode [{instruction}] at index [{instructionPointer}]");
                };

                instructionPointer += parameters.Length + 1;
            }
        }

        private static int[] GetParameters(int[] program, int instruction, int instructionPointer)
        {
            switch (instruction)
            {
                case 1:
                case 2:
                    return new[] { program[instructionPointer + 1], program[instructionPointer + 2], program[instructionPointer + 3] };
                default:
                    throw new Exception($"Invalid opCode [{instruction}] at index [{instructionPointer}]");
            }
        }
    }
}
