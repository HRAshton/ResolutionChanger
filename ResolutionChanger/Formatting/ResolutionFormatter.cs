namespace ResolutionChanger.Formatting;

internal static class ResolutionFormatter
{
    public static string Format(Size resolution)
    {
        return $"{resolution.Width} × {resolution.Height} ({FormatAspectRatio(resolution)})";
    }

    public static string FormatAspectRatio(Size resolution)
    {
        int divisor = GreatestCommonDivisor(resolution.Width, resolution.Height);
        string exactRatio = $"{resolution.Width / divisor}:{resolution.Height / divisor}";
        if (resolution.Width % resolution.Height == 0)
        {
            return exactRatio;
        }

        decimal approximateRatio = (decimal)resolution.Width / resolution.Height;
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
