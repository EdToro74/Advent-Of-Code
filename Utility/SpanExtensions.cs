using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class SpanExtensions
    {
        public static (bool found, IEnumerable<int> addends) FindTarget(this Span<int> numbers, int target, int numberOfAddends)
        {
            if (numberOfAddends > 0)
            {
                for (var i = 0; i < numbers.Length; i++)
                {
                    var candidate = numbers[i];
                    if (numberOfAddends == 1 && candidate == target)
                    {
                        return (true, new[] { candidate });
                    }

                    var remaining = target - candidate;
                    if (remaining > 0)
                    {
                        var result = FindTarget(numbers[(i + 1)..], remaining, numberOfAddends - 1);
                        if (result.found)
                        {
                            return (true, result.addends.Concat(new[] { candidate }));
                        }
                    }
                }
            }

            return (false, null);
        }

        public static (bool found, IEnumerable<long> addends) FindTarget(this Span<long> numbers, long target, int numberOfAddends)
        {
            if (numberOfAddends > 0)
            {
                for (var i = 0; i < numbers.Length; i++)
                {
                    var candidate = numbers[i];
                    if (numberOfAddends == 1 && candidate == target)
                    {
                        return (true, new[] { candidate });
                    }

                    var remaining = target - candidate;
                    if (remaining > 0)
                    {
                        var result = FindTarget(numbers[(i + 1)..], remaining, numberOfAddends - 1);
                        if (result.found)
                        {
                            return (true, result.addends.Concat(new[] { candidate }));
                        }
                    }
                }
            }

            return (false, null);
        }
    }
}
