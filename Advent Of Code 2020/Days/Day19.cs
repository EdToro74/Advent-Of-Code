using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day19
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(19);

            var availableRules = new Rule[]
            {
                new CharacterRule(),
                new RuleRun(),
                new RuleOr()
            };

            var rules = new Dictionary<int, Rule>();

            var stage = 0;

            var matched = 0;

            foreach (var line in input)
            {
                if (stage == 0)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        stage++;
                        continue;
                    }

                    var parts = line.Split(':');
                    var id = int.Parse(parts[0]);
                    var ruleText = parts[1].Trim();

                    if (id == 8)
                    {
                        ruleText = "42 8 | 42";
                    }
                    //else if (id == 11) ruleText = "42 11 31 | 42 31";

                    var rule = availableRules.Select(ar => ar.Parse(id, ruleText)).Where(r => r != null).Single();
                    rules[id] = rule;
                }
                else if (stage == 1)
                {
                    if (Evaluate(line, 0, rules))
                    {
                        matched++;
                    }
                }
            }

            Console.WriteLine($"{matched} matched rule 0");
        }

        public static bool Evaluate(string line, int ruleId, IDictionary<int, Rule> rules)
        {
            var rule = rules[ruleId];

            var stack = new Stack<EvalState>();
            var lastResult = RuleResult.Empty;

            stack.Push(new EvalState { RuleId = ruleId, RuleBehavior = rule.Evaluate(line, rules).GetEnumerator() });

            while (stack.Any())
            {
                var current = stack.Peek();
                if (current.LastResult != null)
                {
                    current.LastResult.Consumed = lastResult.Consumed;
                    current.CurrentIndex += lastResult.Consumed;
                }

                if (!current.RuleBehavior.MoveNext())
                {
                    throw new InvalidOperationException("Not expecting any Rule to complete enumeration since we short-circuit based on result");
                }

                var result = current.RuleBehavior.Current;
                current.LastResult = result;

                if (result.SubRule == null)
                {
                    stack.Pop();
                    lastResult = result;
                }
                else
                {
                    stack.Push(new EvalState { RuleId = result.SubRule.Id, RuleBehavior = result.SubRule.Evaluate(line[current.CurrentIndex..], rules).GetEnumerator(), CurrentIndex = current.CurrentIndex });
                }
            }

            return lastResult.Consumed == line.Length;
        }

        public class EvalState
        {
            public int RuleId { get; init; }
            public IEnumerator<RuleResult> RuleBehavior { get; init; }
            public RuleResult LastResult { get; set; }
            public int CurrentIndex { get; set; }
        }

        public class RuleResult
        {
            public int Consumed { get; set; }
            public Rule SubRule { get; init; }

            public static RuleResult Empty => new RuleResult();
        }

        public abstract class Rule
        {
            public int Id { get; init; }

            public Rule() { }

            public Rule(int id)
            {
                Id = id;
            }

            public abstract Rule Parse(int id, string input);

            public abstract IEnumerable<RuleResult> Evaluate(string input, IDictionary<int, Rule> rules);
        }

        public class CharacterRule : Rule
        {
            private static readonly Regex _match = new Regex("^\"[a-z]\"$");
            private readonly char _character;

            public CharacterRule() { }

            private CharacterRule(int id, char character) : base(id)
            {
                _character = character;
            }

            public override Rule Parse(int id, string input)
            {
                if (_match.IsMatch(input))
                {
                    return new CharacterRule(id, input[1]);
                }

                return null;
            }

            public override IEnumerable<RuleResult> Evaluate(string input, IDictionary<int, Rule> rules) => new[] { new RuleResult { Consumed = input[0] == _character ? 1 : 0 } };
        }

        public class RuleRun : Rule
        {
            private static readonly Regex _match = new Regex("^(?:(?<ruleId>[0-9]+) *)+$");
            private readonly IEnumerable<int> _ruleIds;

            public RuleRun() { }

            private RuleRun(int id, IEnumerable<int> ruleIds) : base(id)
            {
                _ruleIds = ruleIds;
            }

            public override Rule Parse(int id, string input)
            {
                var match = _match.Match(input);
                if (match.Success)
                {
                    return new RuleRun(id, match.Groups["ruleId"].Captures.Select(c => int.Parse(c.Value)));
                }

                return null;
            }

            public override IEnumerable<RuleResult> Evaluate(string input, IDictionary<int, Rule> rules)
            {
                var currentIndex = 0;
                foreach (var rule in _ruleIds.Select(id => rules[id]))
                {
                    if (currentIndex >= input.Length)
                    {
                        yield return RuleResult.Empty;
                        yield break;
                    }

                    var childResult = new RuleResult { SubRule = rule };
                    yield return childResult;

                    if (childResult.Consumed == 0)
                    {
                        yield return RuleResult.Empty;
                        yield break;
                    }

                    currentIndex += childResult.Consumed;
                }

                yield return new RuleResult { Consumed = currentIndex };
            }
        }

        public class RuleOr : Rule
        {
            private static readonly Regex _match = new Regex("^(?<ruleRun>(?:(?:[0-9]) *)+) \\| (?<ruleRun>(?:(?:[0-9]) *)+)$");

            private readonly Rule _leftRule;
            private readonly Rule _rightRule;

            public RuleOr() { }

            private RuleOr(int id, Rule leftRule, Rule rightRule) : base(id)
            {
                _leftRule = leftRule;
                _rightRule = rightRule;
            }

            public override Rule Parse(int id, string input)
            {
                var match = _match.Match(input);
                if (match.Success)
                {
                    var factory = new RuleRun();
                    var leftRule = factory.Parse(id * 1000 + 1, match.Groups["ruleRun"].Captures[0].Value);
                    var rightRule = factory.Parse(id * 1000 + 1, match.Groups["ruleRun"].Captures[1].Value);

                    if (leftRule != null && rightRule != null)
                    {
                        return new RuleOr(id, leftRule, rightRule);
                    }
                }

                return null;
            }

            public override IEnumerable<RuleResult> Evaluate(string input, IDictionary<int, Rule> rules)
            {
                var childResult = new RuleResult { SubRule = _leftRule };

                yield return childResult;
                if (childResult.Consumed == 0)
                {
                    childResult = new RuleResult { SubRule = _rightRule };
                    yield return childResult;
                }

                yield return new RuleResult { Consumed = childResult.Consumed };
            }
        }
    }
}
