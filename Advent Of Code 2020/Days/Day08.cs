using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day08
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(8).ToList();

            (bool terminated, GameState gameState) RunGameConsole()
            {
                var gameConsole = GameConsole.Parse(input);

                while (!gameConsole.HasLooped && !gameConsole.HasTerminated)
                {
                    gameConsole.MoveNext();
                }

                return (gameConsole.HasTerminated, gameConsole.GameState);
            }

            var part1 = RunGameConsole();
            if (part1.terminated)
            {
                throw new InvalidOperationException("Part 1 expected to infinite loop");
            }

            Console.WriteLine($"Accumulator: {part1.gameState.Accumulator}");

            // Change each nop to jmp or jmp to nop one by one
            for (var i = 0; i < input.Count; i++)
            {
                if (input[i].StartsWith("nop"))
                {
                    input[i] = input[i].Replace("nop ", "jmp ");
                    var attempt = RunGameConsole();
                    if (attempt.terminated)
                    {
                        Console.WriteLine($"Adjusting instruction {i} fixed the program.  Accumulator: {attempt.gameState.Accumulator}");
                        return;
                    }

                    input[i] = input[i].Replace("jmp ", "nop ");
                }

                if (input[i].StartsWith("jmp"))
                {
                    input[i] = input[i].Replace("jmp ", "nop ");
                    var attempt = RunGameConsole();
                    if (attempt.terminated)
                    {
                        Console.WriteLine($"Adjusting instruction {i} fixed the program.  Accumulator: {attempt.gameState.Accumulator}");
                        return;
                    }

                    input[i] = input[i].Replace("nop ", "jmp ");
                }
            }
        }
    }

    internal class GameConsole
    {
        private readonly GameState _gameState = new GameState();
        private readonly List<Instruction> _instructions;
        public readonly HashSet<int> _visitedIndexes = new HashSet<int>();

        public GameState GameState => _gameState;

        public bool HasLooped => _visitedIndexes.Contains(_gameState.InstructionIndex);

        public bool HasTerminated => _gameState.InstructionIndex == _instructions.Count;

        private GameConsole(List<Instruction> instructions) => _instructions = instructions;

        public static GameConsole Parse(IEnumerable<string> code)
        {
            var instructions = new List<Instruction>();

            foreach (var line in code)
            {
                var match = Regex.Match(line, "(?<command>[a-z]+) (?<argument>[+-][0-9]+)");

                var command = match.Groups["command"].Value;
                var argument = int.Parse(match.Groups["argument"].Value);

                switch (command)
                {
                    case "nop":
                        instructions.Add(new NoOp(argument));
                        break;
                    case "acc":
                        instructions.Add(new Accumulate(argument));
                        break;
                    case "jmp":
                        instructions.Add(new JumpRelative(argument));
                        break;
                    default:
                        throw new ArgumentException($"Unknown instruction: {command}");
                }
            }

            return new GameConsole(instructions);
        }

        public void MoveNext()
        {
            var instructionIndex = _gameState.InstructionIndex;
            _instructions[instructionIndex].Execute(_gameState);
            _ = _visitedIndexes.Add(instructionIndex);
        }

        private abstract class Instruction
        {
            public Instruction(int argument) => Argument = argument;

            public int Argument { get; private set; }

            public virtual void Execute(GameState gameState) => gameState.JumpRelative(1);
        }

        private class NoOp : Instruction
        {
            public NoOp(int argument) : base(argument)
            {
            }
        }

        private class Accumulate : Instruction
        {
            public Accumulate(int argument) : base(argument)
            {
            }

            public override void Execute(GameState gameState)
            {
                gameState.Accumulate(Argument);
                base.Execute(gameState);
            }
        }

        private class JumpRelative : Instruction
        {
            public JumpRelative(int argument) : base(argument)
            {
            }

            public override void Execute(GameState gameState) => gameState.JumpRelative(Argument);
        }
    }

    internal class GameState
    {
        public int Accumulator { get; private set; }
        public int InstructionIndex { get; private set; }

        public void Accumulate(int x) => Accumulator += x;

        public void JumpRelative(int x) => InstructionIndex += x;
    }
}
