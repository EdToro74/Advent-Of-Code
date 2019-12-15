using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Advent_Of_Code_2019
{
    static class Day03
    {
        public static int Part1(IEnumerable<string> input)
        {
            var wire1 = ParseWire(input.First());
            var wire2 = ParseWire(input.Skip(1).First());

            return GetIntersections(wire1, wire2).Select(p => Math.Abs(p.X) + Math.Abs(p.Y)).OrderBy(d => d).First();
        }

        public static int Part2(IEnumerable<string> input)
        {
            var wire1 = ParseWire(input.First());
            var wire2 = ParseWire(input.Skip(1).First());

            return GetIntersectionsWithDistance(wire1, wire2).Select(i => i.distance1 + i.distance2).OrderBy(d => d).First();
        }

        private struct Segment
        {
            public Point Start;
            public Point End;
        }

        private static IEnumerable<Point> GetIntersections(IEnumerable<Segment> wire1, IEnumerable<Segment> wire2)
        {
            var first1 = true;
            var first2 = true;
            foreach (var segment1 in wire1)
            {
                foreach (var segment2 in wire2)
                {
                    // First segments intersect at origin but don't count
                    if (first1 && first2)
                    {
                        continue;
                    }

                    first2 = false;

                    var intersection = GetIntersection(segment1, segment2);
                    if (intersection.HasValue)
                    {
                        yield return intersection.Value;
                    }
                }

                first1 = false;
            }
        }

        private static IEnumerable<(Point intersection, int distance1, int distance2)> GetIntersectionsWithDistance(IEnumerable<Segment> wire1, IEnumerable<Segment> wire2)
        {
            var first1 = true;
            var first2 = true;

            var distance1 = 0;

            foreach (var segment1 in wire1)
            {
                var distance2 = 0;
                foreach (var segment2 in wire2)
                {
                    // First segments intersect at origin but don't count
                    if (first1 && first2)
                    {
                        distance2 += Math.Abs(segment2.Start.X - segment2.End.X) + Math.Abs(segment2.Start.Y - segment2.End.Y);
                        continue;
                    }

                    first2 = false;

                    var intersection = GetIntersection(segment1, segment2);
                    if (intersection.HasValue)
                    {
                        var intersectionDistance1 = distance1 + Math.Abs(segment1.Start.X - intersection.Value.X) + Math.Abs(segment1.Start.Y - intersection.Value.Y);
                        var intersectionDistance2 = distance2 + Math.Abs(segment2.Start.X - intersection.Value.X) + Math.Abs(segment2.Start.Y - intersection.Value.Y);
                        yield return (intersection.Value, intersectionDistance1, intersectionDistance2);
                    }

                    distance2 += Math.Abs(segment2.Start.X - segment2.End.X) + Math.Abs(segment2.Start.Y - segment2.End.Y);
                }

                first1 = false;
                distance1 += Math.Abs(segment1.Start.X - segment1.End.X) + Math.Abs(segment1.Start.Y - segment1.End.Y);
            }
        }

        private static Point? GetIntersection(Segment segment1, Segment segment2)
        {
            // Use matrix to determine any intersection
            var x1 = segment1.Start.X;
            var x2 = segment1.End.X;
            var y1 = segment1.Start.Y;
            var y2 = segment1.End.Y;

            var x3 = segment2.Start.X;
            var x4 = segment2.End.X;
            var y3 = segment2.Start.Y;
            var y4 = segment2.End.Y;

            var denominator = (x4 - x3) * (y1 - y2) - (x1 - x2) * (y4 - y3);

            if (denominator != 0)
            {

                var numerator1 = (y3 - y4) * (x1 - x3) + (x4 - x3) * (y1 - y3);
                var numerator2 = (y1 - y2) * (x1 - x3) + (x2 - x1) * (y1 - y3);

                var t1 = numerator1 * 1.0 / denominator * 1.0;
                var t2 = numerator2 * 1.0 / denominator * 1.0;

                if (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
                {
                    return new Point(
                        segment1.Start.X + (int)(t1 * (segment1.End.X - segment1.Start.X)),
                        segment1.Start.Y + (int)(t1 * (segment1.End.Y - segment1.Start.Y))
                    );
                }
            }

            return null;
        }

        private static IEnumerable<Segment> ParseWire(string input)
        {
            var x = 0;
            var y = 0;

            var startPoint = new Point(x, y);

            foreach (var line in input.Split(','))
            {
                var direction = line[0];
                var distance = int.Parse(line.Substring(1));

                switch (direction)
                {
                    case 'R':
                        x += distance;
                        break;
                    case 'L':
                        x -= distance;
                        break;
                    case 'U':
                        y += distance;
                        break;
                    case 'D':
                        y -= distance;
                        break;
                    default:
                        throw new Exception($"Unknown direction [{direction}]");
                }

                var endPoint = new Point(x, y);
                yield return new Segment() { Start = startPoint, End = endPoint };
                startPoint = endPoint;
            }
        }
    }
}
