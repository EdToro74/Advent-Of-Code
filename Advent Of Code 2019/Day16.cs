using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    public static class Day16
    {
        public static string Part1(IEnumerable<string> input)
        {
            var phaseCount = 100;
            var digits = input.First().Select(c => int.Parse($"{c}")).ToArray();
            var pattern = new[] { 0, 1, 0, -1 };

            for (var phase = 0; phase < phaseCount; phase++)
            {
                var newDigits = new int[digits.Length];

                for (var outputIndex = 0; outputIndex < digits.Length; outputIndex++)
                {
                    var value = 0;

                    for (var inputIndex = outputIndex; inputIndex < digits.Length; inputIndex++)
                    {
                        var operation = pattern[(inputIndex + 1) / (outputIndex + 1) % pattern.Length];
                        if (operation == 0)
                        {
                            continue;
                        }

                        value += digits[inputIndex] * operation;
                    }

                    newDigits[outputIndex] = Math.Abs(value % 10);
                }

                digits = newDigits;
                //Console.WriteLine($"Phase {phase + 1}: {string.Join("", digits)}");
            }

            return string.Join("", digits.ToArray()).Substring(0, 8);
        }

        public static string Part2(IEnumerable<string> input)
        {
            var phaseCount = 100;
            var repeatCount = 10_000;

            var digits = Enumerable.Repeat(input.First().Select(c => int.Parse($"{c}")), repeatCount).SelectMany(c => c).ToArray();

            var offset = int.Parse(input.First().Substring(0, 7));

            for (var phase = 0; phase < phaseCount; phase++)
            {
                var newDigits = new int[digits.Length];

                for (var i = digits.Length - 1; i >= offset; i--)
                {
                    if (i == digits.Length - 1)
                    {
                        newDigits[i] = digits[i];
                    }
                    else
                    {
                        newDigits[i] = (digits[i] + newDigits[i + 1]) % 10;
                    }
                }

                digits = newDigits;
            }

            return string.Join("", digits).Substring(offset, 8);
        }
    }
}
