namespace ResolutionChanger;

internal static class ResolutionCatalog
{
    public static readonly IReadOnlyList<CatalogResolution> All =
    [
        new("4:3", 640, 480),
        new("4:3", 800, 600),
        new("4:3", 1024, 768),
        new("4:3", 1280, 960),
        new("4:3", 1600, 1200),
        new("5:4", 1280, 1024),
        new("16:10", 1280, 800),
        new("16:10", 1440, 900),
        new("16:10", 1680, 1050),
        new("16:10", 1920, 1200),
        new("16:9", 1280, 720),
        new("16:9", 1366, 768),
        new("16:9", 1600, 900),
        new("16:9", 1920, 1080),
        new("16:9", 2560, 1440),
        new("16:9", 3840, 2160),
        new("21:9", 2560, 1080),
        new("21:9", 3440, 1440),
        new("32:9", 3840, 1080),
        new("32:9", 5120, 1440),
    ];
}
