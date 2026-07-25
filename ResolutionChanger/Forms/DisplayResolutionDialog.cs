using ResolutionChanger.Constants;
using ResolutionChanger.Formatting;
using ResolutionChanger.Models;
using ResolutionChanger.Services;

namespace ResolutionChanger.Forms;

internal sealed class DisplayResolutionDialog : Form
{
    private const int MaximumResolutionDimension = short.MaxValue;

    private readonly IReadOnlyList<DisplayInfo> _displays = DisplayService.GetDisplays();
    private readonly ResolutionBinding _original;
    private readonly ComboBox _display = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown _width = new() { Minimum = 1, Maximum = MaximumResolutionDimension };
    private readonly NumericUpDown _height = new() { Minimum = 1, Maximum = MaximumResolutionDimension };
    public ResolutionBinding? Binding { get; private set; }

    public DisplayResolutionDialog(ResolutionBinding? binding)
    {
        _original = binding ?? ResolutionBinding.New();
        Text = "Display and resolution";
        Icon = ApplicationIcon.Shared;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(460, 190);
        foreach (DisplayInfo item in _displays)
        {
            _display.Items.Add(item);
        }

        _display.DisplayMember = nameof(DisplayInfo.DisplayName);
        _display.SelectedIndex = Math.Max(
            0,
            _displays.ToList().FindIndex(x => x.DeviceName == _original.DisplayDeviceName)
        );
        _width.Value = _original.Resolution.Width;
        _height.Value = _original.Resolution.Height;
        Button select = new() { Text = "Select…", AutoSize = true };
        select.Click += ShowKnownResolutions;
        Button ok = new()
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
        };
        ok.Click += Approve;
        Button cancel = new()
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
        };
        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            ColumnCount = 3,
            RowCount = 4,
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.Controls.Add(
            new Label
            {
                Text = "Display:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
            },
            0,
            0
        );
        layout.Controls.Add(_display, 1, 0);
        layout.SetColumnSpan(_display, 2);
        layout.Controls.Add(
            new Label
            {
                Text = "Width:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
            },
            0,
            1
        );
        layout.Controls.Add(_width, 1, 1);
        layout.Controls.Add(
            new Label
            {
                Text = "Height:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
            },
            0,
            2
        );
        layout.Controls.Add(_height, 1, 2);
        layout.Controls.Add(select, 2, 2);
        FlowLayoutPanel buttons = new() { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(ok);
        layout.Controls.Add(buttons, 0, 3);
        layout.SetColumnSpan(buttons, 3);
        Controls.Add(layout);
        AcceptButton = ok;
        CancelButton = cancel;
    }

    private void ShowKnownResolutions(object? sender, EventArgs e)
    {
        ContextMenuStrip menu = new();
        DisplayInfo? display = _display.SelectedItem as DisplayInfo;
        HashSet<Size> supported = display?.SupportedResolutions.ToHashSet() ?? [];
        foreach (
            IGrouping<string, Size> group in ResolutionCatalog.All.GroupBy(x =>
                ResolutionFormatter.FormatAspectRatio(x)
            )
        )
        {
            ToolStripMenuItem ratio = new(group.Key);
            foreach (Size resolution in group)
            {
                ToolStripMenuItem item = new(
                    $"{resolution.Width} × {resolution.Height}{(supported.Contains(resolution) ? "  ✓ supported" : string.Empty)}"
                )
                {
                    Font = new Font(menu.Font, supported.Contains(resolution) ? FontStyle.Bold : FontStyle.Regular),
                };
                item.Click += (_, _) =>
                {
                    _width.Value = resolution.Width;
                    _height.Value = resolution.Height;
                };
                ratio.DropDownItems.Add(item);
            }

            menu.Items.Add(ratio);
        }

        menu.Show((Control)sender!, new Point(0, ((Control)sender!).Height));
    }

    private void Approve(object? sender, EventArgs e)
    {
        if (_display.SelectedItem is not DisplayInfo display)
        {
            MessageBox.Show(
                this,
                "Choose an installed display first.",
                Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            DialogResult = DialogResult.None;
            return;
        }

        Binding = _original with
        {
            DisplayDeviceName = display.DeviceName,
            DisplayName = display.DisplayName,
            Resolution = new Size((int)_width.Value, (int)_height.Value),
        };
    }
}
