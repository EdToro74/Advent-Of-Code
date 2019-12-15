using System;
using System.Collections.Generic;
using System.Text;

namespace Advent_Of_Code_2019
{
    static class Day14
    {
        public static int Part1(IEnumerable<string> input)
        {
            return 0;
        }

        class Reaction
        {
            public ReactionComponent Output { get; }

            public IEnumerable<ReactionComponent> Input { get; }

            public class ReactionComponent
            {
                public string Name;
                public int Amount;
            }

            private Reaction(ReactionComponent output, IEnumerable<ReactionComponent> input)
            {
                Output = output;
                Input = input;
            }

            public static Reaction Parse(string input)
            {
                //"3 A, 4 B => 1 AB"
                var inputOutput = input.Split(" => ");
                return null;
            }
        }
    }
}
