using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
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

        struct Permutation : IEnumerable<int>
        {
            public static Permutation Empty { get; } = new Permutation(Array.Empty<int>());

            private readonly int[] _permutation;

            private Permutation(int[] permutation)
            {
                _permutation = permutation;
            }

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

            public int this[int index]
            {
                get { return _permutation[index]; }
            }

            public IEnumerator<int> GetEnumerator()
            {
                foreach (int item in _permutation)
                    yield return item;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int Count { get { return _permutation.Length; } }

            public override string ToString()
            {
                return string.Join(",", _permutation);
            }

            private static IEnumerable<Permutation> HamiltonianPermutationsIterator(int n)
            {
                if (n == 0)
                {
                    yield return Empty;
                    yield break;
                }
                bool forwards = false;
                foreach (Permutation permutation in HamiltonianPermutationsIterator(n - 1))
                {
                    for (int index = 0; index < n; index += 1)
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
                    throw new ArgumentNullException(nameof(items));
                if (position < 0)
                    throw new ArgumentOutOfRangeException(nameof(position));
                return InsertAtIterator<T>(items, position, newItem);
            }

            private static IEnumerable<T> InsertAtIterator<T>(IEnumerable<T> items, int position, T newItem)
            {
                int index = 0;
                bool yieldedNew = false;
                foreach (T item in items)
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
                    throw new ArgumentOutOfRangeException(nameof(position));
            }
        }
    }
}
