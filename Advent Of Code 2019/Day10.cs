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

            Console.WriteLine(laser);

            var inOrder = asteroids.Select(a =>
            {
                var angle = (180 / Math.PI) * Math.Atan2(a.y - laser.y, a.x - laser.x) + 90;
                if (angle < 0)
                {
                    angle += 360;
                }

                var distance = Math.Sqrt(Math.Pow(a.y - laser.y, 2) + Math.Pow(a.x - laser.x, 2));

                return (coords: a, angle, distance);
            }).OrderBy(a => a.angle).ThenBy(a => a.distance).ToList();

            var destroyed = new List<((int x, int y), double angle, double distance)>();
            var lastAngle = double.MinValue;

            foreach (var asteroid in inOrder)
            {
                if (asteroid.angle == lastAngle)
                {
                    continue;
                }

                destroyed.Add(asteroid);
                if (destroyed.Count == 200)
                {
                    return $"200th asteroid: {asteroid.coords} Value: {asteroid.coords.x * 100 + asteroid.coords.y}";
                }
                lastAngle = asteroid.angle;
            }

            return "Less than 200 asteroids destroyed";
        }

        private static ((int x, int y) coordinates, HashSet<(int x, int y)> detected) GetMostDetected(IEnumerable<string> input)
        {
            var asteroids = GetAsteroids(input);

            return GetMostDetected(asteroids);
        }

        private static ((int x, int y) coordinates, HashSet<(int x, int y)> detected) GetMostDetected(HashSet<(int x, int y)> asteroids)
        {
            var detectedMap = new Dictionary<(int x, int y), HashSet<(int x, int y)>>();

            for (var i = 0; i < asteroids.Count; i++)
            {
                for (var j = i + 1; j < asteroids.Count; j++)
                {
                    var asteroid1 = asteroids.ElementAt(i);
                    var asteroid2 = asteroids.ElementAt(j);

                    var potentialBlockers = GetPotentialBlockers(asteroid1, asteroid2);
                    if (!potentialBlockers.Any(p => asteroids.Contains(p)))
                    {
                        if (!detectedMap.TryGetValue(asteroid1, out var detected))
                        {
                            detected = new HashSet<(int x, int y)>();
                            detectedMap[asteroid1] = detected;
                        }
                        detected.Add(asteroid2);

                        if (!detectedMap.TryGetValue(asteroid2, out detected))
                        {
                            detected = new HashSet<(int x, int y)>();
                            detectedMap[asteroid2] = detected;
                        }
                        detected.Add(asteroid1);
                    }
                }
            }

            var maxDetected = detectedMap.OrderByDescending(kvp => kvp.Value.Count).First();

            return ((maxDetected.Key.x, maxDetected.Key.y), maxDetected.Value);
        }

        private static HashSet<(int x, int y)> GetAsteroids(IEnumerable<string> input)
        {
            var asteroids = new HashSet<(int x, int y)>();

            var y = 0;
            foreach (var line in input)
            {
                foreach (var asteroid in line.Select((c, x) => (c, x)).Where(p => p.c == '#').Select(p => (p.x, y)))
                {
                    asteroids.Add(asteroid);
                }
                y++;
            }

            return asteroids;
        }

        private static IEnumerable<(int x, int y)> GetPotentialBlockers((int x, int y) coord1, (int x, int y) coord2)
        {
            var xSlope = Math.Abs(coord1.x - coord2.x);
            var ySlope = Math.Abs(coord1.y - coord2.y);

            if (xSlope == 0)
            {
                foreach (var y in RangeBetween(coord1.y, coord2.y))
                {
                    yield return (coord1.x, y);
                }
            }
            else if (ySlope == 0)
            {
                foreach (var x in RangeBetween(coord1.x, coord2.x))
                {
                    yield return (x, coord1.y);
                }
            }
            else
            {
                var commonFactors = GetCommonFactors(xSlope, ySlope);
                var xGoingUp = coord2.x > coord1.x;
                var yGoingUp = coord2.y > coord1.y;

                foreach (var commonFactor in commonFactors)
                {
                    var xDelta = xSlope / commonFactor;
                    var yDelta = ySlope / commonFactor;

                    var current = coord1;
                    while (current != coord2)
                    {
                        current.x += xGoingUp ? xDelta : -xDelta;
                        current.y += yGoingUp ? yDelta : -yDelta;

                        if (current == coord1 || current == coord2)
                        {
                            break;
                        }

                        yield return current;
                    }
                }
            }
        }

        private static IEnumerable<int> RangeBetween(int x, int y)
        {
            var start = Math.Min(x, y);
            var end = Math.Max(x, y);

            for (var i = start + 1; i < end; i++)
            {
                yield return i;
            }
        }

        private static IEnumerable<int> GetCommonFactors(int x, int y)
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

            return xFactors.Intersect(yFactors);
        }

        private static IEnumerable<int> GetFactors(int x)
        {
            for (int factor = 1; factor * factor <= x; factor++)
            {
                if (x % factor == 0)
                {
                    yield return factor;
                    if (factor * factor != x)
                        yield return x / factor;
                }
            }
        }
    }
}
