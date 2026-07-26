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
    private readonly Label _ratio = new()
    {
        AutoSize = true,
        BorderStyle = BorderStyle.None,
        Padding = new Padding(3),
    };

    public ResolutionBinding? Binding { get; private set; }

    public DisplayResolutionDialog(ResolutionBinding? binding)
    {
        DisplayInfo defaultDisplay = _displays.FirstOrDefault(x => x.IsPrimary) ?? _displays[0];
        _original = binding ?? ResolutionBinding.New(defaultDisplay.CurrentResolution);
        Text = "Display and resolution";
        Icon = ApplicationIcon.Shared;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(480, 210);
        foreach (DisplayInfo item in _displays)
        {
            _display.Items.Add(item);
        }

        _display.DisplayMember = nameof(DisplayInfo.DisplayName);
        _display.SelectedItem = binding is null
            ? defaultDisplay
            : _displays.FirstOrDefault(x => x.DeviceName == _original.DisplayDeviceName) ?? defaultDisplay;
        _width.Value = _original.Resolution.Width;
        _height.Value = _original.Resolution.Height;
        _width.ValueChanged += UpdateRatio;
        _height.ValueChanged += UpdateRatio;

        Button presets = new() { Text = "Presets…", AutoSize = true };
        presets.Click += ShowKnownResolutions;
        Button ok = new()
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            AutoSizeMode = AutoSizeMode.GrowOnly,
        };
        ok.Click += Approve;
        Button cancel = new()
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            AutoSizeMode = AutoSizeMode.GrowOnly,
        };
        FlowLayoutPanel resolutionInputs = new() { AutoSize = true, WrapContents = false };
        resolutionInputs.Controls.Add(_width);
        resolutionInputs.Controls.Add(
            new Label
            {
                Text = "×",
                AutoSize = true,
                Padding = new Padding(3, 4, 3, 0),
            }
        );
        resolutionInputs.Controls.Add(_height);
        resolutionInputs.Controls.Add(presets);
        FlowLayoutPanel buttons = new() { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(ok);
        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            ColumnCount = 2,
            RowCount = 4,
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.Controls.Add(CreateLabel("Display:"), 0, 0);
        layout.Controls.Add(_display, 1, 0);
        layout.Controls.Add(CreateLabel("Resolution:"), 0, 1);
        layout.Controls.Add(resolutionInputs, 1, 1);
        layout.Controls.Add(CreateLabel("Ratio:"), 0, 2);
        layout.Controls.Add(_ratio, 1, 2);
        layout.Controls.Add(buttons, 0, 3);
        layout.SetColumnSpan(buttons, 2);
        _display.Dock = DockStyle.Fill;
        Controls.Add(layout);
        AcceptButton = ok;
        CancelButton = cancel;
        UpdateRatio(this, EventArgs.Empty);
    }

    private static Label CreateLabel(string text)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
        };
    }

    private void ShowKnownResolutions(object? sender, EventArgs e)
    {
        DisplayInfo? display = _display.SelectedItem as DisplayInfo;
        ContextMenuStrip menu = ResolutionMenuBuilder.Create(
            display?.SupportedResolutions.ToHashSet() ?? [],
            SelectResolution
        );
        Control presetsButton = (Control)sender!;
        menu.Show(presetsButton, new Point(0, presetsButton.Height));
    }

    private void SelectResolution(Size resolution)
    {
        _width.Value = resolution.Width;
        _height.Value = resolution.Height;
    }

    private void UpdateRatio(object? sender, EventArgs e)
    {
        _ratio.Text = ResolutionFormatter.FormatAspectRatio(new Size((int)_width.Value, (int)_height.Value));
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
