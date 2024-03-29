﻿using System;
using System.Collections.Generic;

namespace Advent_Of_Code_2019
{
    internal static class SpaceImageFormat
    {
        private const int TRANSPARENT = 2;

        public static IEnumerable<int[,]> GetLayers(string input, int width, int height)
        {
            var layerSize = width * height;

            if (input.Length % layerSize != 0)
            {
                throw new Exception("Input does not conform to image size");
            }

            for (var i = 0; i < input.Length / layerSize; i++)
            {
                var layer = new int[height, width];

                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        layer[y, x] = input[(i * width * height) + (y * width) + x] - '0';
                    }
                }

                yield return layer;
            }
        }

        public static int[,] DecodeImage(string input, int width, int height)
        {
            var layers = GetLayers(input, width, height);

            var image = new int[height, width];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    foreach (var layer in layers)
                    {
                        if (layer[y, x] != TRANSPARENT)
                        {
                            image[y, x] = layer[y, x];
                            break;
                        }
                    }
                }
            }

            return image;
        }

        public static string DisplayImage(int[,] image) => Utility.DisplayImage(image, v => v == 0 ? ' ' : '█');
    }
}
