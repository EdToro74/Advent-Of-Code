using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Advent_Of_Code_2019
{
    static class Day10
    {
        public static string Part1(IEnumerable<string> input)
        {
            var maxDetected = GetMostDetected(input);

            return $"{maxDetected.coordinates} detects {maxDetected.detected.Count} asteroids";
        }

        public static string Part2(IEnumerable<string> input)
        {
            var asteroids = GetAsteroids(input);

            var maxDetected = GetMostDetected(asteroids);

            var laser = maxDetected.coordinates;
            asteroids.Remove(laser);

            var xDelta = Math.Max(laser.x, asteroids.Max(a => a.x) - laser.x);
            var yDelta = Math.Max(laser.y, asteroids.Max(a => a.y) - laser.y);
            var xLCM = LCM(Range(1, xDelta));
            var yLCM = LCM(Range(1, yDelta));

            var xRange = Range(laser.x, xLCM) // From Top Middle to Top Right
                .Concat(Repeat(laser.x + xLCM, yLCM * 2)) // Top Right to Bottom Right
                .Concat(Range(laser.x - xLCM, xLCM * 2 + 1).Reverse()) // Bottom Right to Bottom Left
                .Concat(Repeat(laser.x - xLCM, yLCM * 2)) // Bottom Left to Top Left
                .Concat(Range(laser.x - xLCM + 1, xLCM - 1)); // Top Left to Top Middle

            var yRange = Repeat(laser.y - yLCM, xLCM) // From Top Middle to Top Right
                .Concat(Range(laser.y - yLCM, yLCM * 2 + 1)) // Top Right to Bottom Right
                .Concat(Repeat(laser.y + yLCM, xLCM * 2)) // Bottom Right to Bottom Left
                .Concat(Range(laser.y - yLCM, yLCM * 2).Reverse()) // Bottom Left to Top Left
                .Concat(Repeat(laser.y - yLCM, xLCM - 1)); // Top Left to Top Middle

            var laserTargets = xRange.Zip(yRange).Select(xy => (x: xy.First, y: xy.Second));

            var destroyedCount = 0;
            var limit = (x: asteroids.Max(a => a.x), y: asteroids.Max(a => a.y));

            while (asteroids.Count > 0)
            {
                foreach (var laserTarget in laserTargets)
                {
                    var hit = GetPotentialBlockers(laser, laserTarget, limit).Where(coord => asteroids.Contains(coord)).OrderBy(a => Math.Pow(a.x - laser.x, 2) + Math.Pow(a.y - laser.y, 2)).Take(1);
                    foreach (var destroyed in hit)
                    {
                        destroyedCount++;
                        asteroids.Remove(destroyed);
                        if (destroyedCount == 200)
                        {
                            return $"{destroyed} is 100th asteroid destroyed.  Value: {destroyed.x * 100 + destroyed.y}";
                        }
                    }
                }
            }

            return "Less than 200 asteroids destroyed";
        }

        private static ((long x, long y) coordinates, HashSet<(long x, long y)> detected) GetMostDetected(IEnumerable<string> input)
        {
            var asteroids = GetAsteroids(input);

            return GetMostDetected(asteroids);
        }

        private static ((long x, long y) coordinates, HashSet<(long x, long y)> detected) GetMostDetected(HashSet<(long x, long y)> asteroids)
        {
            var limit = (x: asteroids.Max(a => a.x), y: asteroids.Max(a => a.y));

            var detectedMap = new Dictionary<(long x, long y), HashSet<(long x, long y)>>();

            for (var i = 0; i < asteroids.Count; i++)
            {
                for (var j = i + 1; j < asteroids.Count; j++)
                {
                    var asteroid1 = asteroids.ElementAt(i);
                    var asteroid2 = asteroids.ElementAt(j);

                    var potentialBlockers = GetPotentialBlockers(asteroid1, asteroid2, limit);
                    if (!potentialBlockers.Any(p => asteroids.Contains(p)))
                    {
                        if (!detectedMap.TryGetValue(asteroid1, out var detected))
                        {
                            detected = new HashSet<(long x, long y)>();
                            detectedMap[asteroid1] = detected;
                        }
                        detected.Add(asteroid2);

                        if (!detectedMap.TryGetValue(asteroid2, out detected))
                        {
                            detected = new HashSet<(long x, long y)>();
                            detectedMap[asteroid2] = detected;
                        }
                        detected.Add(asteroid1);
                    }
                }
            }

            var maxDetected = detectedMap.OrderByDescending(kvp => kvp.Value.Count).First();

            return ((maxDetected.Key.x, maxDetected.Key.y), maxDetected.Value);
        }

        private static HashSet<(long x, long y)> GetAsteroids(IEnumerable<string> input)
        {
            var asteroids = new HashSet<(long x, long y)>();

            var y = 0L;
            foreach (var line in input)
            {
                foreach (var asteroid in line.Select((c, x) => (c, x)).Where(p => p.c == '#').Select(p => ((long)p.x, y)))
                {
                    asteroids.Add(asteroid);
                }
                y++;
            }

            return asteroids;
        }

        private static IEnumerable<(long x, long y)> GetPotentialBlockers((long x, long y) coord1, (long x, long y) coord2, (long x, long y) limit)
        {
            var xSlope = Math.Abs(coord1.x - coord2.x);
            var ySlope = Math.Abs(coord1.y - coord2.y);

            if (xSlope == 0)
            {
                foreach (var y in RangeBetween(coord1.y, coord2.y, limit.y))
                {
                    yield return (coord1.x, y);
                }
                yield break;
            }
            else if (ySlope == 0)
            {
                foreach (var x in RangeBetween(coord1.x, coord2.x, limit.x))
                {
                    yield return (x, coord1.y);
                }
                yield break;
            }
            else
            {
                var commonFactors = GetCommonFactors(xSlope, ySlope, Math.Max(limit.x, limit.y));
                var xGoingUp = coord2.x > coord1.x;
                var yGoingUp = coord2.y > coord1.y;

                foreach (var commonFactor in commonFactors)
                {
                    var xDelta = xSlope / commonFactor;
                    if (xDelta > limit.x && xGoingUp)
                    {
                        continue;
                    }
                    var yDelta = ySlope / commonFactor;
                    if (yDelta > limit.y && yGoingUp)
                    {
                        continue;
                    }

                    var current = coord1;
                    while (current != coord2)
                    {
                        current.x += xGoingUp ? xDelta : -xDelta;
                        current.y += yGoingUp ? yDelta : -yDelta;

                        if (current == coord1 || current == coord2)
                        {
                            break;
                        }

                        if (current.x > limit.x || current.y > limit.y)
                        {
                            break;
                        }

                        if (current.x < 0 || current.y < 0)
                        {
                            continue;
                        }

                        yield return current;
                    }
                }
            }
        }

        private static IEnumerable<long> RangeBetween(long x, long y, long limit)
        {
            var start = Math.Max(0, Math.Min(x, y));
            var end = Math.Min(limit, Math.Max(x, y));

            for (var i = start + 1; i < end; i++)
            {
                yield return i;
            }
        }

        private static Dictionary<(long x, long y), long[]> _commonFactors = new Dictionary<(long x, long y), long[]>();
        private static IEnumerable<long> GetCommonFactors(long x, long y, long limit)
        {
            var xTarget = Math.Min(x, limit);
            var yTarget = Math.Min(y, limit);

            if (!_commonFactors.TryGetValue((xTarget, yTarget), out var commonFactors))
            {
                commonFactors = GetCommonFactorsCore(xTarget, yTarget).ToArray();
                _commonFactors[(xTarget, yTarget)] = commonFactors;
            }

            return commonFactors;
        }

        private static IEnumerable<long> GetCommonFactorsCore(long x, long y)
        {
            var xFactors = GetFactors(x);
            if (y == 0)
            {
                return xFactors;
            }
            var yFactors = GetFactors(y);
            if (y == 0)
            {
                return yFactors;
            }

            if (x == 1 || y == 1)
            {
                return new[] { 1L };
            }
            else
            {
                return xFactors.Intersect(yFactors);
            }
        }

        private static Dictionary<long, long[]> _computedFactors = new Dictionary<long, long[]>();

        private static IEnumerable<long> GetFactors(long x)
        {
            var target = (long)Math.Sqrt(Math.Abs(x));

            if (!_computedFactors.TryGetValue(target, out var factors))
            {
                factors = GetFactorsEnumerable(target).ToArray();
                _computedFactors[target] = factors;
            }

            return factors;
        }

        private static IEnumerable<long> GetFactorsEnumerable(long x)
        {
            for (long factor = 1; factor <= x; factor++)
            {
                if (x % factor == 0)
                {
                    yield return factor;
                    if (factor * factor != x)
                        yield return x / factor;
                }
            }
        }

        private static long LCM(IEnumerable<long> numbers)
        {
            return numbers.Aggregate(lcm);
        }

        private static long lcm(long a, long b)
        {
            return Math.Abs(a * b) / GCD(a, b);
        }

        private static long GCD(long a, long b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        public static IEnumerable<long> Range(long start, long count)
        {
            long max = start + count - 1;
            for (var i = 0L; i < max; i++) yield return start + i;
        }

        public static IEnumerable<long> Repeat(long value, long count)
        {
            for (var i = 0L; i < count; i++) yield return value;
        }
    }
}
