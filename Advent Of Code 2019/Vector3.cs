using System;
using System.Diagnostics.CodeAnalysis;

namespace Advent_Of_Code_2019
{
    public class Vector3 : IEquatable<Vector3>
    {
        public long X { get; set; } = 0;
        public long Y { get; set; } = 0;
        public long Z { get; set; } = 0;

        public bool Equals([AllowNull] Vector3 other)
        {
            return other != null && X == other.X && Y == other.Y && Z == other.Z;
        }

        public override string ToString()
        {
            return $"<x={X}, y={Y}, z={Z}>";
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3 other && X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19;
                hash = hash * 31 + X.GetHashCode();
                hash = hash * 31 + Y.GetHashCode();
                hash = hash * 31 + Z.GetHashCode();
                return hash;
            }
        }

        public static Vector3 Zero { get; } = new Vector3();
    }
}
