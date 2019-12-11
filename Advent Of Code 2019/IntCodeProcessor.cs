using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    static class IntCodeProcessor
    {
        public static void ProcessProgram(IEnumerable<string> programText, int[] inputs = null)
        {
            var program = programText.First().Split(',').Select(s => int.Parse(s)).ToArray();

            ProcessProgram(program, inputs);
        }

        public static void ProcessProgram(int[] program, int[] inputs = null)
        {
            var instructionPointer = 0;
            var inputPointer = 0;
            var outputted = false;

            while (true)
            {
                var instruction = GetInstruction(program, instructionPointer);
                if (instruction == 99)
                {
                    if (outputted)
                    {
                        Console.WriteLine();
                    }
                    return;
                }

                var parameters = GetParameters(program, instruction, instructionPointer).ToArray();

                switch (instruction)
                {
                    case 1:
                        // Add
                        program[parameters[2].value] = GetParameterValue(program, parameters[0]) + GetParameterValue(program, parameters[1]);
                        instructionPointer += 4;
                        break;
                    case 2:
                        // Multiple
                        program[parameters[2].value] = GetParameterValue(program, parameters[0]) * GetParameterValue(program, parameters[1]);
                        instructionPointer += 4;
                        break;
                    case 3:
                        // Input
                        var input = inputs[inputPointer++];
                        program[parameters[0].value] = input;
                        instructionPointer += 2;
                        break;
                    case 4:
                        // Output
                        Console.Write(GetParameterValue(program, parameters[0]));
                        outputted = true;
                        instructionPointer += 2;
                        break;
                    case 5:
                        // Jump If True
                        if (GetParameterValue(program, parameters[0]) != 0)
                        {
                            instructionPointer = GetParameterValue(program, parameters[1]);
                        }
                        else
                        {
                            instructionPointer += 3;
                        }
                        break;
                    case 6:
                        // Jump If False
                        if (GetParameterValue(program, parameters[0]) == 0)
                        {
                            instructionPointer = GetParameterValue(program, parameters[1]);
                        }
                        else
                        {
                            instructionPointer += 3;
                        }
                        break;
                    case 7:
                        // Less Than
                        program[parameters[2].value] = GetParameterValue(program, parameters[0]) < GetParameterValue(program, parameters[1]) ? 1 : 0;
                        instructionPointer += 4;
                        break;
                    case 8:
                        // Equals
                        program[parameters[2].value] = GetParameterValue(program, parameters[0]) == GetParameterValue(program, parameters[1]) ? 1 : 0;
                        instructionPointer += 4;
                        break;
                    default:
                        throw new Exception($"Invalid opCode [{instruction}] at index [{instructionPointer}]");
                };
            }
        }

        private static int GetInstruction(int[] program, int instructionPointer)
        {
            return program[instructionPointer] % 100;
        }

        private static IEnumerable<(bool isImmediate, int value)> GetParameters(int[] program, int instruction, int instructionPointer)
        {
            int parameterCount;

            switch (instruction)
            {
                case 1:
                case 2:
                    parameterCount = 3;
                    break;
                case 3:
                case 4:
                    parameterCount = 1;
                    break;
                case 5:
                case 6:
                    parameterCount = 2;
                    break;
                case 7:
                case 8:
                    parameterCount = 3;
                    break;
                default:
                    throw new Exception($"Invalid opCode [{instruction}] at index [{instructionPointer}]");
            }

            var parameterModes = program[instructionPointer];
            var mask = 100;

            for (var i = 1; i <= parameterCount; i++)
            {
                var isImmediate = parameterModes / mask % 10 == 1;
                var parameterValue = program[instructionPointer + i];
                yield return (isImmediate, parameterValue);
                mask *= 10;
            }
        }

        private static int GetParameterValue(int[] program, (bool isImmediate, int value) parameter)
        {
            if (parameter.isImmediate)
            {
                return parameter.value;
            }

            return program[parameter.value];
        }
    }
}
