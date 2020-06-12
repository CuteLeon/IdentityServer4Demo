namespace ConsoleClient
{
    public static class StringExtension
    {
        public static string Left(this string input, int maxLength = 50)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            if (input.Length <= maxLength) return input;
            return $"{input.Substring(0, maxLength)}...";
        }
    }
}
