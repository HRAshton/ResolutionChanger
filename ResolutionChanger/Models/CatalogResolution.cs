namespace ResolutionChanger.Models;

internal readonly record struct CatalogResolution(int Width, int Height)
{
    public Size Size => new(Width, Height);
}
