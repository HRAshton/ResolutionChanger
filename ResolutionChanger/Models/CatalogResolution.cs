namespace ResolutionChanger.Models;

internal readonly record struct CatalogResolution(string Ratio, int Width, int Height)
{
    public Size Size => new(Width, Height);
}
