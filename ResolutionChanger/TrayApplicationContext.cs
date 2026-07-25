using ResolutionChanger.Configuration;
using ResolutionChanger.Forms;
using ResolutionChanger.Models;
using ResolutionChanger.Services;

namespace ResolutionChanger;

internal sealed class TrayApplicationContext : ApplicationContext
{
    private const int ErrorBalloonTipTimeoutMilliseconds = 5000;
    private const string ApplicationName = "Resolution Changer";

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
        menu.Items.Add(CreateStartupMenuItem());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => Exit());

        _trayIcon = new NotifyIcon
        {
            Icon = ApplicationIcon.Shared,
            Text = ApplicationName,
            ContextMenuStrip = menu,
            Visible = true,
        };
        _trayIcon.DoubleClick += (_, _) => Open();
    }

    private ToolStripMenuItem CreateStartupMenuItem()
    {
        ToolStripMenuItem item = new("Start when I sign in")
        {
            CheckOnClick = true,
            Checked = StartupRegistrationService.IsEnabled(),
        };
        bool isUpdating = false;
        item.CheckedChanged += (_, _) =>
        {
            if (isUpdating)
            {
                return;
            }

            try
            {
                StartupRegistrationService.SetEnabled(item.Checked);
            }
            catch (Exception exception)
            {
                isUpdating = true;
                item.Checked = !item.Checked;
                isUpdating = false;
                ShowError(exception.Message);
            }
        };
        return item;
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
            DisplayService.ChangeResolution(binding.DisplayDeviceName, binding.Resolution);
        }
        catch (InvalidOperationException exception)
        {
            ShowError(exception.Message);
        }
    }

    private void ShowError(string message)
    {
        _trayIcon.ShowBalloonTip(ErrorBalloonTipTimeoutMilliseconds, ApplicationName, message, ToolTipIcon.Error);
    }
}
