namespace System.Text.RegularExpressions
{
    public static class RegexExtensions
    {
        public static bool TryMatch(this Regex src, string input, out Match match)
        {
            match = src.Match(input);
            return match.Success;
        }
    }
}
