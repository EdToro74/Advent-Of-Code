using System.Collections.Generic;
using System.IO;

namespace Utility
{
    public static class Utility
    {
        public static IEnumerable<string> GetDayFile(int day) => File.ReadLines(Path.Combine(Directory.GetCurrentDirectory(), $@"..\..\..\Input\Day{day:D2}.txt"));
    }
}

