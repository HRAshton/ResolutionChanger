using System.Runtime.InteropServices;
using ResolutionChanger.Models;
using ResolutionChanger.NativeMethods;
using DevMode = ResolutionChanger.NativeMethods.User32DisplayNativeMethods.DevMode;
using DisplayDevice = ResolutionChanger.NativeMethods.User32DisplayNativeMethods.DisplayDevice;

namespace ResolutionChanger.Services;

internal static class DisplayService
{
    public static IReadOnlyList<DisplayInfo> GetDisplays()
    {
        List<DisplayInfo> displays = [];
        for (uint index = 0; ; index++)
        {
            DisplayDevice device = new() { cb = Marshal.SizeOf<DisplayDevice>() };
            if (
                !User32DisplayNativeMethods.EnumDisplayDevices(
                    null,
                    index,
                    ref device,
                    User32DisplayNativeMethods.NoFlags
                )
            )
            {
                break;
            }

            if ((device.StateFlags & User32DisplayNativeMethods.DisplayDeviceAttachedToDesktop) == 0)
            {
                continue;
            }

            DevMode currentMode = DevMode.Create();
            if (!User32DisplayNativeMethods.EnumDisplaySettings(device.DeviceName, uint.MaxValue, ref currentMode))
            {
                continue;
            }

            HashSet<Size> resolutions = [];
            DevMode mode = DevMode.Create();
            for (
                uint modeIndex = 0;
                User32DisplayNativeMethods.EnumDisplaySettings(device.DeviceName, modeIndex, ref mode);
                modeIndex++
            )
            {
                if (mode is { PelsWidth: > 0, PelsHeight: > 0 })
                {
                    resolutions.Add(new Size(mode.PelsWidth, mode.PelsHeight));
                }
            }

            displays.Add(
                new DisplayInfo(
                    device.DeviceName,
                    device.DeviceString,
                    new Size(currentMode.PelsWidth, currentMode.PelsHeight),
                    [.. resolutions.OrderBy(x => x.Width * x.Height)],
                    (device.StateFlags & User32DisplayNativeMethods.DisplayDevicePrimaryDevice) != 0
                )
            );
        }

        return displays;
    }

    public static DisplayChangeResult ChangeResolution(string deviceName, Size resolution)
    {
        DisplayInfo? display = GetDisplays().FirstOrDefault(x => x.DeviceName == deviceName);
        if (display is null)
        {
            return DisplayChangeResult.DisplayUnavailable;
        }

        if (!display.SupportedResolutions.Contains(resolution))
        {
            return DisplayChangeResult.UnsupportedResolution;
        }

        DevMode mode = DevMode.Create();
        if (!User32DisplayNativeMethods.EnumDisplaySettings(deviceName, uint.MaxValue, ref mode))
        {
            return DisplayChangeResult.DisplayUnavailable;
        }

        mode.PelsWidth = resolution.Width;
        mode.PelsHeight = resolution.Height;
        mode.Fields = User32DisplayNativeMethods.DmPelsWidth | User32DisplayNativeMethods.DmPelsHeight;
        int result = User32DisplayNativeMethods.ChangeDisplaySettingsEx(
            deviceName,
            ref mode,
            IntPtr.Zero,
            User32DisplayNativeMethods.NoFlags,
            IntPtr.Zero
        );
        return (DisplayChangeResult)result;
    }
}
