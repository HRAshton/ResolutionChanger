namespace ResolutionChanger.Models;

internal sealed record DisplayInfo(
    string DeviceName,
    string DisplayName,
    Size CurrentResolution,
    IReadOnlyList<Size> SupportedResolutions,
    bool IsPrimary
);
