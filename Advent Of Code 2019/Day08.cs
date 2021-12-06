using System.Collections.Generic;
using System.Linq;

namespace Advent_Of_Code_2019
{
    internal static class Day08
    {
        public static int Part1(IEnumerable<string> input, int width, int height)
        {
            var image = input.First();

            var layers = SpaceImageFormat.GetLayers(image, width, height);

            var fewest0Digits = layers.Select(layer => layer.Cast<int>()).OrderBy(layer => layer.Count(pixel => pixel == 0)).First();

            var numberOf1Digits = fewest0Digits.Count(pixel => pixel == 1);
            var numberOf2Digits = fewest0Digits.Count(pixel => pixel == 2);

            return numberOf1Digits * numberOf2Digits;
        }

        public static string Part2(IEnumerable<string> input, int width, int height)
        {
            var image = input.First();

            var decoded = SpaceImageFormat.DecodeImage(image, width, height);

            return SpaceImageFormat.DisplayImage(decoded);
        }
    }
}