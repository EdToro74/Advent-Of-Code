namespace System.Collections.Generic
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<(T, T)> EnumeratePairs<T>(this IEnumerable<T> enumerable)
        {
            var queue = new Queue<T>(enumerable);

            while (queue.Count > 1)
            {
                yield return (queue.Dequeue(), queue.Peek());
            }
        }

        public static IEnumerable<T2> EnumeratePairs<T, T2>(this IEnumerable<T> enumerable, Func<T, T, T2> selector)
        {
            var queue = new Queue<T>(enumerable);

            while (queue.Count > 1)
            {
                yield return selector(queue.Dequeue(), queue.Peek());
            }
        }

        public static T SingleOrFallback<T>(this IEnumerable<T> source, T fallback)
        {
            T value = fallback;

            var enumerator = source.GetEnumerator();

            if (enumerator.MoveNext())
            {
                var first = enumerator.Current;
                if (!enumerator.MoveNext())
                {
                    value = first;
                }
            }

            return value;
        }

        public static IEnumerable<IEnumerable<T>> SlidingWindow<T>(this IEnumerable<T> source, int windowSize)
        {
            var buffer = new Queue<T>(windowSize);

            foreach (var item in source)
            {
                if (buffer.Count == windowSize)
                {
                    yield return buffer;
                    buffer.Dequeue();
                }
                buffer.Enqueue(item);
            }

            if (buffer.Count == windowSize)
            {
                yield return buffer;
            }
        }
    }
}
