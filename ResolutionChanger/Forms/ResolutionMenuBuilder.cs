using ResolutionChanger.Constants;
using ResolutionChanger.Formatting;

namespace ResolutionChanger.Forms;

internal static class ResolutionMenuBuilder
{
    public static ContextMenuStrip Create(IReadOnlySet<Size> supportedResolutions, Action<Size> selectResolution)
    {
        ContextMenuStrip menu = new();
        foreach (IGrouping<string, Size> group in ResolutionCatalog.All.GroupBy(ResolutionFormatter.FormatAspectRatio))
        {
            ToolStripMenuItem ratio = new(group.Key);
            foreach (Size resolution in group)
            {
                bool isSupported = supportedResolutions.Contains(resolution);
                string supportLabel = isSupported ? "  ✓ supported" : string.Empty;
                ToolStripMenuItem item = new($"{resolution.Width} × {resolution.Height}{supportLabel}")
                {
                    Font = new Font(menu.Font, isSupported ? FontStyle.Bold : FontStyle.Regular),
                };
                item.Click += (_, _) => selectResolution(resolution);
                ratio.DropDownItems.Add(item);
            }

            menu.Items.Add(ratio);
        }

        return menu;
    }
}
