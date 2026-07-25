using ResolutionChanger.NativeMethods;

namespace ResolutionChanger;

internal sealed class HotkeyManager : NativeWindow, IDisposable
{
    private const int WmHotkey = 0x0312;

    private const uint ModAlt = 0x0001,
        ModControl = 0x0002,
        ModShift = 0x0004,
        ModWin = 0x0008;

    private readonly Action<ResolutionBinding> _onPressed;
    private readonly Dictionary<int, ResolutionBinding> _registrations = [];
    private int _nextId = 1;

    public HotkeyManager(Action<ResolutionBinding> onPressed)
    {
        _onPressed = onPressed;
        CreateHandle(new CreateParams());
    }

    public ResolutionBinding? FindBinding(string hotkey)
    {
        return _registrations.Values.FirstOrDefault(x =>
            string.Equals(x.HotkeyText, hotkey, StringComparison.OrdinalIgnoreCase)
        );
    }

    public void Register(IEnumerable<ResolutionBinding> bindings)
    {
        foreach (int id in _registrations.Keys)
        {
            User32HotkeyNativeMethods.UnregisterHotKey(Handle, id);
        }

        _registrations.Clear();
        _nextId = 1;
        foreach (ResolutionBinding binding in bindings.Where(x => x.HotkeyText != BindingDefaults.UnassignedHotkeyText))
        {
            if (
                !TryParse(binding.HotkeyText, out uint modifiers, out Keys key)
                || !User32HotkeyNativeMethods.RegisterHotKey(Handle, _nextId, modifiers, (uint)key)
            )
            {
                continue;
            }

            _registrations.Add(_nextId++, binding);
        }
    }

    protected override void WndProc(ref Message message)
    {
        if (
            message.Msg == WmHotkey
            && _registrations.TryGetValue(message.WParam.ToInt32(), out ResolutionBinding? binding)
        )
        {
            _onPressed(binding);
        }

        base.WndProc(ref message);
    }

    public void Dispose()
    {
        foreach (int id in _registrations.Keys)
        {
            User32HotkeyNativeMethods.UnregisterHotKey(Handle, id);
        }

        DestroyHandle();
        GC.SuppressFinalize(this);
    }

    private static bool TryParse(string text, out uint modifiers, out Keys key)
    {
        modifiers = 0;
        key = Keys.None;
        foreach (string part in text.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            switch (part.ToUpperInvariant())
            {
                case "CTRL":
                case "CONTROL":
                    modifiers |= ModControl;
                    break;
                case "ALT":
                    modifiers |= ModAlt;
                    break;
                case "SHIFT":
                    modifiers |= ModShift;
                    break;
                case "WIN":
                    modifiers |= ModWin;
                    break;
                default:
                    if (!Enum.TryParse(part, true, out key))
                    {
                        return false;
                    }

                    break;
            }
        }

        return key != Keys.None;
    }
}
