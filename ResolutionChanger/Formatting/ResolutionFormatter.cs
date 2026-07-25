namespace ResolutionChanger.Formatting;

internal static class ResolutionFormatter
{
    public static string Format(int width, int height)
    {
        return $"{width} × {height} ({FormatAspectRatio(width, height)})";
    }

    public static string FormatAspectRatio(int width, int height)
    {
        int divisor = GreatestCommonDivisor(width, height);
        string exactRatio = $"{width / divisor}:{height / divisor}";
        if (width % height == 0)
        {
            return exactRatio;
        }

        decimal approximateRatio = (decimal)width / height;
        return $"{exactRatio} · {approximateRatio:0.##}:1";
    }

    private static int GreatestCommonDivisor(int first, int second)
    {
        while (second != 0)
        {
            int remainder = first % second;
            first = second;
            second = remainder;
        }

        return first;
    }
}
