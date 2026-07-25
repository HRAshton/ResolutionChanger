namespace ResolutionChanger.Models;

internal enum DisplayChangeResult
{
    Success = 0,
    RestartRequired = 1,
    Failed = -1,
    BadMode = -2,
    NotUpdated = -3,
    BadFlags = -4,
    BadParameter = -5,
    BadDualView = -6,
    DisplayUnavailable = int.MinValue,
    UnsupportedResolution = int.MinValue + 1,
}
