using System;
using System.Collections.Generic;
using System.IO;

namespace Utility
{
    public static class Utility
    {
        public static IEnumerable<string> GetDayFile(int day)
        {
            return File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), $@"..\..\..\Input\Day{day:D2}.txt"));
        }

        public static IEnumerable<(T, T)> EnumeratePairs<T>(this IEnumerable<T> enumerable)
        {
            var queue = new Queue<T>(enumerable);
            
            while (queue.Count > 1)
            {
                yield return (queue.Dequeue(), queue.Peek());
            }
        }
    }
}

