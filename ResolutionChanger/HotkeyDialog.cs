namespace ResolutionChanger;

internal sealed class HotkeyDialog : Form
{
    private readonly ResolutionBinding _original;
    private readonly Func<string, ResolutionBinding?> _findBinding;
    private readonly Label _value = new()
    {
        AutoSize = true,
        Text = "Press a shortcut…",
        BorderStyle = BorderStyle.Fixed3D,
        Padding = new Padding(8),
        MinimumSize = new Size(230, 35),
    };

    private string _hotkey;

    public ResolutionBinding? Binding { get; private set; }

    public HotkeyDialog(ResolutionBinding binding, Func<string, ResolutionBinding?> findBinding)
    {
        _original = binding;
        _findBinding = findBinding;
        _hotkey = binding.HotkeyText;
        Text = "Set hotkey";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(330, 140);
        KeyPreview = true;
        KeyDown += CaptureKey;

        Button approveButton = new() { Text = "Approve", DialogResult = DialogResult.OK };
        approveButton.Click += Approve;
        Button cancelButton = new() { Text = "Cancel", DialogResult = DialogResult.Cancel };
        FlowLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(14),
            FlowDirection = FlowDirection.TopDown,
        };
        layout.Controls.Add(new Label { Text = "Press the new global hotkey:", AutoSize = true });
        layout.Controls.Add(_value);
        FlowLayoutPanel buttons = new() { AutoSize = true };
        buttons.Controls.Add(approveButton);
        buttons.Controls.Add(cancelButton);
        layout.Controls.Add(buttons);
        Controls.Add(layout);
        UpdateText();
        AcceptButton = approveButton;
        CancelButton = cancelButton;
    }

    private void CaptureKey(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode is Keys.ControlKey or Keys.ShiftKey or Keys.Menu or Keys.LWin or Keys.RWin)
        {
            return;
        }

        if (e.KeyCode == Keys.Escape)
        {
            _hotkey = BindingDefaults.UnassignedHotkeyText;
            UpdateText();
            return;
        }

        List<string> parts = [];
        if (e.Control)
        {
            parts.Add("Ctrl");
        }

        if (e.Alt)
        {
            parts.Add("Alt");
        }

        if (e.Shift)
        {
            parts.Add("Shift");
        }

        if (e.KeyCode is Keys.LWin or Keys.RWin)
        {
            parts.Add("Win");
        }

        parts.Add(e.KeyCode.ToString());
        _hotkey = string.Join('+', parts);
        UpdateText();
        e.SuppressKeyPress = true;
    }

    private void UpdateText()
    {
        _value.Text = _hotkey == BindingDefaults.UnassignedHotkeyText ? "No hotkey" : _hotkey;
    }

    private void Approve(object? sender, EventArgs e)
    {
        ResolutionBinding? existing = _findBinding(_hotkey);
        if (
            _hotkey != BindingDefaults.UnassignedHotkeyText
            && existing is not null
            && existing.Id != _original.Id
            && MessageBox.Show(
                this,
                $"{_hotkey} is assigned to {existing.DisplayName} - {existing.Width} × {existing.Height}. Override it?",
                Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            ) != DialogResult.Yes
        )
        {
            DialogResult = DialogResult.None;
            return;
        }

        Binding = _original with { HotkeyText = _hotkey };
    }
}
