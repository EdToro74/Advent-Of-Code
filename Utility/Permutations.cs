using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    public static class Permutations
    {
        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this T[] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return GetPermutationsIterator(source);
        }

        private static IEnumerable<IEnumerable<T>> GetPermutationsIterator<T>(T[] source)
        {
            foreach (var permutation in Permutation.HamiltonianPermutations(source.Length))
            {
                yield return permutation.Select(index => source[index]);
            }
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this List<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return GetPermutationsIterator(source);
        }

        private static IEnumerable<IEnumerable<T>> GetPermutationsIterator<T>(List<T> source)
        {
            foreach (var permutation in Permutation.HamiltonianPermutations(source.Count))
            {
                yield return permutation.Select(index => source[index]);
            }
        }

        private struct Permutation : IEnumerable<int>
        {
            public static Permutation Empty { get; } = new Permutation(Array.Empty<int>());

            private readonly int[] _permutation;

            private Permutation(int[] permutation) => _permutation = permutation;

            private Permutation(IEnumerable<int> permutation)
                : this(permutation.ToArray())
            {
            }

            public static IEnumerable<Permutation> HamiltonianPermutations(int n)
            {
                if (n < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(n));
                }

                return HamiltonianPermutationsIterator(n);
            }

            public int this[int index] => _permutation[index];

            public IEnumerator<int> GetEnumerator()
            {
                foreach (var item in _permutation)
                {
                    yield return item;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

            public int Count => _permutation.Length;

            public override string ToString() => string.Join(",", _permutation);

            private static IEnumerable<Permutation> HamiltonianPermutationsIterator(int n)
            {
                if (n == 0)
                {
                    yield return Empty;
                    yield break;
                }

                var forwards = false;
                foreach (var permutation in HamiltonianPermutationsIterator(n - 1))
                {
                    for (var index = 0; index < n; index += 1)
                    {
                        yield return new Permutation(
                            InsertAt(permutation, forwards ? index : n - index - 1, n - 1));
                    }

                    forwards = !forwards;
                }
            }

            private static IEnumerable<T> InsertAt<T>(IEnumerable<T> items, int position, T newItem)
            {
                if (items == null)
                {
                    throw new ArgumentNullException(nameof(items));
                }

                return position < 0 ? throw new ArgumentOutOfRangeException(nameof(position)) : InsertAtIterator<T>(items, position, newItem);
            }

            private static IEnumerable<T> InsertAtIterator<T>(IEnumerable<T> items, int position, T newItem)
            {
                var index = 0;
                var yieldedNew = false;
                foreach (var item in items)
                {
                    if (index == position)
                    {
                        yield return newItem;
                        yieldedNew = true;
                    }

                    yield return item;
                    index += 1;
                }

                if (index == position)
                {
                    yield return newItem;
                    yieldedNew = true;
                }

                if (!yieldedNew)
                {
                    throw new ArgumentOutOfRangeException(nameof(position));
                }
            }
        }
    }
}