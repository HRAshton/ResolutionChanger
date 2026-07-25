namespace ResolutionChanger;

internal static class ApplicationIcon
{
    private const string ResourceName = "ResolutionChanger.Assets.appicon.ico";

    private static readonly Lazy<Icon> SharedIcon = new(Load);

    public static Icon Shared => SharedIcon.Value;

    private static Icon Load()
    {
        using Stream stream =
            typeof(ApplicationIcon).Assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException("The application icon resource could not be loaded.");
        using Icon icon = new(stream);
        return (Icon)icon.Clone();
    }
}
