namespace Lct2023.Helpers;

public static class FileSizeExtension
{
    private const long KB_UNIT = 1024;
    private const long MB_UNIT = 1024 * 1024;
    private const long GB_UNIT = 1024 * 1024 * 1024;

    public static string GetHumanReadableString(this long bytesSize) =>
        bytesSize switch
        {
            >= GB_UNIT => $"{(float)bytesSize / GB_UNIT:F1} Гб",
            >= MB_UNIT => $"{(float)bytesSize / MB_UNIT:F1} Мб",
            >= KB_UNIT => $"{(float)bytesSize / KB_UNIT:F1} Кб",
            _ => $"{bytesSize} б",
        };
}
