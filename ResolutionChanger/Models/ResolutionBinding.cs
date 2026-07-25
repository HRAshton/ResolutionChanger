namespace ResolutionChanger;

internal sealed record ResolutionBinding(
    Guid Id,
    string DisplayDeviceName,
    string DisplayName,
    int Width,
    int Height,
    string HotkeyText
)
{
    public static ResolutionBinding New()
    {
        return new ResolutionBinding(
            Guid.NewGuid(),
            string.Empty,
            string.Empty,
            BindingDefaults.DefaultWidth,
            BindingDefaults.DefaultHeight,
            BindingDefaults.UnassignedHotkeyText
        );
    }
}
