namespace ResolutionChanger;

internal sealed record DisplayInfo(string DeviceName, string DisplayName, IReadOnlyList<Size> SupportedResolutions);
