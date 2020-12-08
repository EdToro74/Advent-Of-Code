using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    static class Day14
    {
        public static long Part1(IEnumerable<string> input)
        {
            var reactions = input.Select(Reaction.Parse).ToArray();

            return Convert("ORE", "FUEL", 1, reactions);
        }

        public static long Part2(IEnumerable<string> input)
        {
            var reactions = input.Select(Reaction.Parse).ToArray();

            var costFor1 = Convert("ORE", "FUEL", 1, reactions);

            var oreStorage = 1_000_000_000_000;

            var low = oreStorage / costFor1 - 1_000;
            var high = oreStorage / costFor1 + 1_000_000;

            return Utility.BinarySearch(low, high, guess => Convert("ORE", "FUEL", guess, reactions).CompareTo(oreStorage));
        }

        private static long Convert(string fromChemical, string toChemical, long amount, IEnumerable<Reaction> reactions)
        {
            var ordered = new List<Reaction>();
            var marked = new HashSet<Reaction>();

            void DepthFirstTraverse(Reaction reaction)
            {
                marked.Add(reaction);

                foreach (var childReaction in reactions.Where(r => r.Inputs.Any(i => i.Chemical == reaction.Output.Chemical)))
                {
                    if (!marked.Contains(childReaction))
                    {
                        DepthFirstTraverse(childReaction);
                    }
                }

                ordered.Add(reaction);
            }

            foreach (var reaction in reactions)
            {
                if (!marked.Contains(reaction))
                {
                    DepthFirstTraverse(reaction);
                }
            }

            var chemicalsNeeded = new Dictionary<string, long>
            {
                [toChemical] = amount
            };

            foreach (var reaction in ordered)
            {
                var toMake = (long)Math.Ceiling(1.0 * chemicalsNeeded[reaction.Output.Chemical] / reaction.Output.Amount);

                foreach (var input in reaction.Inputs)
                {
                    chemicalsNeeded.TryGetValue(input.Chemical, out var existingAmount);
                    chemicalsNeeded[input.Chemical] = existingAmount + input.Amount * toMake;
                }
            }

            return chemicalsNeeded[fromChemical];
        }

        private class Reaction
        {
            public ReactionComponent Output { get; }

            public IEnumerable<ReactionComponent> Inputs { get; }

            public class ReactionComponent
            {
                public string Chemical;
                public long Amount;

                private ReactionComponent(string chemical, long amount)
                {
                    Chemical = chemical;
                    Amount = amount;
                }

                public static ReactionComponent Parse(string input)
                {
                    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var amount = long.Parse(parts[0]);
                    var chemical = parts[1];

                    return new ReactionComponent(chemical, amount);
                }

                public override string ToString()
                {
                    return $"{Amount} {Chemical}";
                }
            }

            private Reaction(ReactionComponent output, IEnumerable<ReactionComponent> inputs)
            {
                Output = output;
                Inputs = inputs;
            }

            public static Reaction Parse(string s)
            {
                var inputOutput = s.Split(" => ");

                var inputs = inputOutput[0].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Select(ReactionComponent.Parse).ToArray();

                var output = ReactionComponent.Parse(inputOutput[1]);

                return new Reaction(output, inputs);
            }

            public override string ToString()
            {
                return $"{string.Join(", ", Inputs)} => {Output}";
            }
        }
    }
}
