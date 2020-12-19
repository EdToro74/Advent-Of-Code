using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day18
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(18);

            var sum = input.Select(expression => new StringReader(expression)).Sum(EvaluateExpression);

            Console.WriteLine($"Sum: {sum}");

            var sum2 = input.Select(expression => new StringReader(expression)).Sum(EvaluateExpressionPart2);

            Console.WriteLine($"Sum Part 2: {sum2}");
        }

        private static long EvaluateExpression(StringReader reader)
        {
            var currentTotal = 0L;
            var currentNumber = 0L;
            int currentOperator = '+';

            int currentCharacter;

            void applyOperator()
            {
                currentTotal = currentOperator switch
                {
                    '+' => currentTotal + currentNumber,
                    '*' => currentTotal * currentNumber,
                    _ => throw new InvalidOperationException($"Unhandled operator {currentOperator}")
                };

                currentOperator = currentCharacter;
                currentNumber = 0;
            }

            while ((currentCharacter = reader.Read()) != -1)
            {
                if (currentCharacter == ' ')
                {
                    continue;
                }
                else if (currentCharacter >= '0' && currentCharacter <= '9')
                {
                    currentNumber = (currentNumber * 10) + (currentCharacter - '0');
                }
                else if (currentCharacter == '+' || currentCharacter == '*')
                {
                    applyOperator();
                }
                else if (currentCharacter == '(')
                {
                    currentNumber = EvaluateExpression(reader);
                }
                else if (currentCharacter == ')')
                {
                    break;
                }
            }

            applyOperator();

            return currentTotal;
        }

        private static long EvaluateExpressionPart2(StringReader reader)
        {
            var currentTotal = 0L;
            var currentNumber = 0L;
            int currentOperator = '+';

            var multiplicationQueue = new Queue<long>();

            int currentCharacter;

            void applyOperator()
            {
                currentTotal = currentOperator switch
                {
                    '+' => currentTotal + currentNumber,
                    '*' => currentTotal == 0 ? currentNumber : currentTotal * currentNumber,
                    _ => throw new InvalidOperationException($"Unhandled operator {currentOperator}")
                };

                currentOperator = currentCharacter;
                currentNumber = 0;
            }

            while ((currentCharacter = reader.Read()) != -1)
            {
                if (currentCharacter == ' ')
                {
                    continue;
                }
                else if (currentCharacter >= '0' && currentCharacter <= '9')
                {
                    currentNumber = (currentNumber * 10) + (currentCharacter - '0');
                }
                else if (currentCharacter == '*')
                {
                    var wasAdding = currentOperator == '+';
                    applyOperator();
                    if (wasAdding && multiplicationQueue.Any())
                    {
                        currentTotal *= multiplicationQueue.Dequeue();
                    }
                }
                else if (currentCharacter == '+')
                {
                    if (currentOperator == '+')
                    {
                        applyOperator();
                    }
                    else
                    {
                        multiplicationQueue.Enqueue(currentTotal);
                        currentTotal = currentNumber;
                        currentNumber = 0;
                        currentOperator = currentCharacter;
                    }
                }
                else if (currentCharacter == '(')
                {
                    currentNumber = EvaluateExpressionPart2(reader);
                }
                else if (currentCharacter == ')')
                {
                    break;
                }
            }

            applyOperator();

            while (multiplicationQueue.Any())
            {
                currentTotal *= multiplicationQueue.Dequeue();
            }

            return currentTotal;
        }
    }
}
