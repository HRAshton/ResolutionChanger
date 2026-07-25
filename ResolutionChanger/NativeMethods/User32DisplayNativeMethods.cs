using System.Runtime.InteropServices;

namespace ResolutionChanger.NativeMethods;

internal static class User32DisplayNativeMethods
{
    internal const uint NoFlags = 0;
    internal const int DisplayDeviceAttachedToDesktop = 0x00000001;
    internal const int DisplayDevicePrimaryDevice = 0x00000004;
    internal const int ChangeDisplaySettingsSuccess = 0;
    internal const int DmPelsWidth = 0x00080000;
    internal const int DmPelsHeight = 0x00100000;
    private const int DeviceNameLength = 32;
    private const int DeviceStringLength = 128;

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport(NativeLibraryNames.User32, EntryPoint = "EnumDisplayDevicesW", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool EnumDisplayDevices(
        string? deviceName,
        uint deviceIndex,
        ref DisplayDevice displayDevice,
        uint flags
    );

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport(NativeLibraryNames.User32, EntryPoint = "EnumDisplaySettingsW", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool EnumDisplaySettings(string deviceName, uint modeIndex, ref DevMode mode);

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport(NativeLibraryNames.User32, EntryPoint = "ChangeDisplaySettingsExW", CharSet = CharSet.Unicode)]
    internal static extern int ChangeDisplaySettingsEx(
        string deviceName,
        ref DevMode mode,
        IntPtr windowHandle,
        uint flags,
        IntPtr parameter
    );

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DisplayDevice
    {
        internal int cb;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DeviceNameLength)]
        internal string DeviceName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DeviceStringLength)]
        internal string DeviceString;

        internal int StateFlags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DeviceStringLength)]
        internal string DeviceId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DeviceStringLength)]
        internal string DeviceKey;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DevMode
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DeviceNameLength)]
        internal string DeviceName;

        internal short SpecVersion;
        internal short DriverVersion;
        internal short Size;
        internal short DriverExtra;
        internal int Fields;
        internal short Orientation;
        internal short PaperSize;
        internal short PaperLength;
        internal short PaperWidth;
        internal short Scale;
        internal short Copies;
        internal short DefaultSource;
        internal short PrintQuality;
        internal short Color;
        internal short Duplex;
        internal short YResolution;
        internal short TTOption;
        internal short Collate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DeviceNameLength)]
        internal string FormName;

        internal short LogPixels;
        internal int BitsPerPel;
        internal int PelsWidth;
        internal int PelsHeight;
        internal int DisplayFlags;
        internal int DisplayFrequency;
        internal int IcmMethod;
        internal int IcmIntent;
        internal int MediaType;
        internal int DitherType;
        internal int Reserved1;
        internal int Reserved2;
        internal int PanningWidth;
        internal int PanningHeight;

        internal static DevMode Create()
        {
            return new DevMode
            {
                DeviceName = string.Empty,
                FormName = string.Empty,
                Size = (short)Marshal.SizeOf<DevMode>(),
            };
        }
    }
}
