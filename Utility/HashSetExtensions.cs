using System.Linq;

namespace System.Collections.Generic
{
    public static class HashSetExtensions
    {
        public static IEnumerable<bool> AddRange<T>(this HashSet<T> src, IEnumerable<T> items) => items.Select(src.Add);
    }
}
