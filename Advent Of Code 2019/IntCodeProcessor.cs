using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    static class IntCodeProcessor
    {
        public interface IProgramState
        {
            void SetMemory(long index, long value);
            long GetMemory(long index);
            IProgramState Copy();
        }

        protected class ProgramState : IProgramState
        {
            private long[] Program { get; }
            protected Dictionary<long, long> Memory { get; } = new Dictionary<long, long>();
            public long RelativeIndex { get; set; }
            public long InstructionPointer { get; set; }

            public ProgramState(long[] program)
            {
                Program = program;
            }

            public void SetMemory(long index, long value)
            {
                if (index < Program.Length)
                {
                    Program[index] = value;
                }
                else
                {
                    Memory[index] = value;
                }
            }

            public long GetMemory(long index)
            {
                if (index < Program.Length)
                {
                    return Program[index];
                }
                else
                {
                    Memory.TryGetValue(index, out var value);
                    return value;
                }
            }

            public IProgramState Copy()
            {
                var copy = new long[Program.Length];
                Program.CopyTo(copy, 0);
                return new ProgramState(copy);
            }
        }

        public static IProgramState ParseProgram(IEnumerable<string> programText)
        {
            var program = programText.First().Split(',').Select(s => long.Parse(s)).ToArray();
            return new ProgramState(program);
        }

        public static IProgramState CopyProgram(IProgramState programState)
        {
            return programState.Copy();
        }

        public static long[] ProcessProgram(IEnumerable<string> programText, params long[] inputs)
        {
            var program = ParseProgram(programText);

            return ProcessProgram(program, inputs);
        }

        public static long[] ProcessProgram(IProgramState programState, params long[] inputs)
        {
            var inputPointer = 0;
            return ProcessProgramEnumerable(programState, () => inputs[inputPointer++]).ToArray();
        }

        public static IEnumerable<long> ProcessProgramEnumerable(IEnumerable<string> programText, Func<long> inputHandler = null)
        {
            var program = ParseProgram(programText);

            return ProcessProgramEnumerable(program, inputHandler);
        }

        public static IEnumerable<long> ProcessProgramEnumerable(IProgramState program, Func<long> inputHandler)
        {
            var programState = (ProgramState)program;
            while (true)
            {
                var (instruction, parameters) = GetInstruction(programState);

                switch (instruction)
                {
                    case 1:
                        // Add
                        programState.SetMemory(parameters[2], parameters[0] + parameters[1]);
                        programState.InstructionPointer += 4;
                        break;
                    case 2:
                        // Multiple
                        programState.SetMemory(parameters[2], parameters[0] * parameters[1]);
                        programState.InstructionPointer += 4;
                        break;
                    case 3:
                        // Input
                        var input = inputHandler();
                        programState.SetMemory(parameters[0], input);
                        programState.InstructionPointer += 2;
                        break;
                    case 4:
                        // Output
                        yield return parameters[0];
                        programState.InstructionPointer += 2;
                        break;
                    case 5:
                        // Jump If True
                        if (parameters[0] != 0)
                        {
                            programState.InstructionPointer = parameters[1];
                        }
                        else
                        {
                            programState.InstructionPointer += 3;
                        }
                        break;
                    case 6:
                        // Jump If False
                        if (parameters[0] == 0)
                        {
                            programState.InstructionPointer = parameters[1];
                        }
                        else
                        {
                            programState.InstructionPointer += 3;
                        }
                        break;
                    case 7:
                        // Less Than
                        programState.SetMemory(parameters[2], parameters[0] < parameters[1] ? 1 : 0);
                        programState.InstructionPointer += 4;
                        break;
                    case 8:
                        // Equals
                        programState.SetMemory(parameters[2], parameters[0] == parameters[1] ? 1 : 0);
                        programState.InstructionPointer += 4;
                        break;
                    case 9:
                        // Adjust Relative Base
                        programState.RelativeIndex += parameters[0];
                        programState.InstructionPointer += 2;
                        break;
                    case 99:
                        yield break;
                    default:
                        throw new Exception($"Invalid opCode [{instruction}] at index [{programState.InstructionPointer}]");
                };
            }
        }

        private static (long instruction, long[] parameters) GetInstruction(ProgramState programState)
        {
            var instruction = programState.GetMemory(programState.InstructionPointer) % 100;

            ParameterDirection[] parameterDirections;

            switch (instruction)
            {
                case 1:
                case 2:
                    parameterDirections = new[] { ParameterDirection.Read, ParameterDirection.Read, ParameterDirection.Write };
                    break;
                case 3:
                    parameterDirections = new[] { ParameterDirection.Write };
                    break;
                case 4:
                    parameterDirections = new[] { ParameterDirection.Read };
                    break;
                case 5:
                case 6:
                    parameterDirections = new[] { ParameterDirection.Read, ParameterDirection.Read };
                    break;
                case 7:
                case 8:
                    parameterDirections = new[] { ParameterDirection.Read, ParameterDirection.Read, ParameterDirection.Write };
                    break;
                case 9:
                    parameterDirections = new[] { ParameterDirection.Read };
                    break;
                case 99:
                    parameterDirections = Array.Empty<ParameterDirection>();
                    break;
                default:
                    throw new Exception($"Invalid opCode [{instruction}] at index [{programState.InstructionPointer}]");
            }

            var parameterModes = programState.GetMemory(programState.InstructionPointer);
            var mask = 100;

            var parameters = new long[parameterDirections.Length];

            for (var i = 0; i < parameterDirections.Length; i++)
            {
                var parameterMode = (ParameterMode)(parameterModes / mask % 10);
                var parameterValue = programState.GetMemory(programState.InstructionPointer + i + 1);

                if (parameterDirections[i] == ParameterDirection.Read)
                {
                    switch (parameterMode)
                    {
                        case ParameterMode.Immediate:
                            break;
                        case ParameterMode.Positional:
                            parameterValue = programState.GetMemory(parameterValue);
                            break;
                        case ParameterMode.Relative:
                            parameterValue = programState.GetMemory(programState.RelativeIndex + parameterValue);
                            break;
                        default:
                            throw new Exception($"Unsupported parameter mode: {parameterMode}");
                    }
                }
                else
                {
                    switch (parameterMode)
                    {
                        case ParameterMode.Immediate:
                            throw new Exception("Immediate mode not supported for Write parameters");
                        case ParameterMode.Positional:
                            break;
                        case ParameterMode.Relative:
                            parameterValue = programState.RelativeIndex + parameterValue;
                            break;
                        default:
                            throw new Exception($"Unsupported parameter mode: {parameterMode}");
                    }
                }

                parameters[i] = parameterValue;
                mask *= 10;
            }

            return (instruction, parameters);
        }

        private enum ParameterMode
        {
            Positional = 0,
            Immediate = 1,
            Relative = 2
        }

        private enum ParameterDirection
        {
            Read,
            Write
        }
    }
}
