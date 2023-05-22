namespace Lct2023.Commons.Extensions
{
    public static class StringExtensions
    {
        public static string FormatEnding(this int number, string end1, string end234, string defaultEnd) =>
            FormatEnding((long)number, end1, end234, defaultEnd);

        public static string FormatEnding(this long number, string end1, string end234, string defaultEnd)
        {
            long mod = number <= 20 ? number : number % 10;

            switch (mod)
            {
                case 1:
                    return end1;
                case 2:
                case 3:
                case 4:
                    return end234;
                default:
                    return defaultEnd;
            }
        }
    }
}