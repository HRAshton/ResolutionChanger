using ResolutionChanger.Models;

namespace ResolutionChanger.Constants;

internal static class ResolutionCatalog
{
    public static readonly IReadOnlyList<CatalogResolution> All =
    [
        new(640, 480),
        new(800, 600),
        new(1024, 768),
        new(1280, 960),
        new(1600, 1200),
        new(1280, 1024),
        new(1280, 800),
        new(1440, 900),
        new(1680, 1050),
        new(1920, 1200),
        new(1280, 720),
        new(1366, 768),
        new(1600, 900),
        new(1920, 1080),
        new(2560, 1440),
        new(3840, 2160),
        new(2560, 1080),
        new(3440, 1440),
        new(3840, 1080),
        new(5120, 1440),
    ];
}
