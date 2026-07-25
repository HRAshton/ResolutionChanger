using System.Runtime.InteropServices;

namespace ResolutionChanger.NativeMethods;

internal static partial class User32HotkeyNativeMethods
{
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [LibraryImport(NativeLibraryNames.User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool RegisterHotKey(IntPtr windowHandle, int identifier, uint modifiers, uint virtualKey);

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [LibraryImport(NativeLibraryNames.User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UnregisterHotKey(IntPtr windowHandle, int identifier);
}
