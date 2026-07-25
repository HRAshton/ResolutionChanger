using System.Runtime.InteropServices;
using ResolutionChanger.Formatting;
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
                    [.. resolutions.OrderBy(x => x.Width * x.Height)]
                )
            );
        }

        return displays;
    }

    public static void ChangeResolution(string deviceName, int width, int height)
    {
        DevMode mode = DevMode.Create();
        if (!User32DisplayNativeMethods.EnumDisplaySettings(deviceName, uint.MaxValue, ref mode))
        {
            throw new InvalidOperationException("The selected display is no longer available.");
        }

        mode.PelsWidth = width;
        mode.PelsHeight = height;
        mode.Fields = User32DisplayNativeMethods.DmPelsWidth | User32DisplayNativeMethods.DmPelsHeight;
        if (
            User32DisplayNativeMethods.ChangeDisplaySettingsEx(
                deviceName,
                ref mode,
                IntPtr.Zero,
                User32DisplayNativeMethods.NoFlags,
                IntPtr.Zero
            ) != User32DisplayNativeMethods.ChangeDisplaySettingsSuccess
        )
        {
            throw new InvalidOperationException(
                $"Windows could not set {ResolutionFormatter.Format(width, height)} on the selected display."
            );
        }
    }
}
