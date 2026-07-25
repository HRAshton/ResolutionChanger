namespace ResolutionChanger;

internal sealed class TrayApplicationContext : ApplicationContext
{
    private const int ErrorBalloonTipTimeoutMilliseconds = 5000;

    private readonly NotifyIcon _trayIcon;
    private readonly BindingStore _store = new();
    private readonly HotkeyManager _hotkeys;
    private Bindings? _bindingsForm;

    public TrayApplicationContext()
    {
        _hotkeys = new HotkeyManager(ApplyBinding);
        _hotkeys.Register(_store.Bindings);
        ContextMenuStrip menu = new();
        menu.Items.Add("Open bindings", null, (_, _) => Open());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => Exit());
        _trayIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = "Resolution Changer",
            ContextMenuStrip = menu,
            Visible = true,
        };
        _trayIcon.DoubleClick += (_, _) => Open();
    }

    private void Open()
    {
        if (_bindingsForm is null || _bindingsForm.IsDisposed)
        {
            _bindingsForm = new Bindings(_store, _hotkeys);
            _bindingsForm.FormClosed += (_, _) => _bindingsForm = null;
        }

        _bindingsForm.Show();
        _bindingsForm.WindowState = FormWindowState.Normal;
        _bindingsForm.Activate();
    }

    private void Exit()
    {
        _trayIcon.Visible = false;
        _trayIcon.Dispose();
        _hotkeys.Dispose();
        ExitThread();
    }

    private void ApplyBinding(ResolutionBinding binding)
    {
        try
        {
            DisplayService.ChangeResolution(binding.DisplayDeviceName, binding.Width, binding.Height);
        }
        catch (InvalidOperationException exception)
        {
            _trayIcon.ShowBalloonTip(
                ErrorBalloonTipTimeoutMilliseconds,
                "Resolution Changer",
                exception.Message,
                ToolTipIcon.Error
            );
        }
    }
}
