using ResolutionChanger.Constants;

namespace ResolutionChanger.Models;

internal sealed record ResolutionBinding(
    Guid Id,
    string DisplayDeviceName,
    string DisplayName,
    Size Resolution,
    string HotkeyText
)
{
    public static ResolutionBinding New()
    {
        return new ResolutionBinding(
            Guid.NewGuid(),
            string.Empty,
            string.Empty,
            BindingDefaults.DefaultResolution,
            BindingDefaults.UnassignedHotkeyText
        );
    }
}
