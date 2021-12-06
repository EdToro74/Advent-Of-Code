using System;
using System.Collections.Generic;

namespace Utility
{
    public static class PowerSet
    {
        public static T[][] FastPowerSet<T>(T[] seq)
        {
            var powerSet = new T[1 << seq.Length][];
            powerSet[0] = Array.Empty<T>(); // starting only with empty set
            for (var i = 0; i < seq.Length; i++)
            {
                var cur = seq[i];
                var count = 1 << i; // doubling list each time
                for (var j = 0; j < count; j++)
                {
                    var source = powerSet[j];
                    var destination = powerSet[count + j] = new T[source.Length + 1];
                    for (var q = 0; q < source.Length; q++)
                    {
                        destination[q] = source[q];
                    }

                    destination[source.Length] = cur;
                }
            }

            return powerSet;
        }

        public static T[][] FastPowerSet<T>(IList<T> seq)
        {
            var powerSet = new T[1 << seq.Count][];
            powerSet[0] = Array.Empty<T>(); // starting only with empty set
            for (var i = 0; i < seq.Count; i++)
            {
                var cur = seq[i];
                var count = 1 << i; // doubling list each time
                for (var j = 0; j < count; j++)
                {
                    var source = powerSet[j];
                    var destination = powerSet[count + j] = new T[source.Length + 1];
                    for (var q = 0; q < source.Length; q++)
                    {
                        destination[q] = source[q];
                    }

                    destination[source.Length] = cur;
                }
            }

            return powerSet;
        }
    }
}
