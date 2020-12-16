using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Utility
{
    public static class Utility
    {
        public static IEnumerable<string> GetDayFile(int day)
        {
            return File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), $@"..\..\..\Input\Day{day:D2}.txt"));
        }
    }
}

