# Resolution Changer

A small Windows tray application that switches a chosen display to a chosen resolution with global hotkeys.

## Why use it?

- **Single-file release:** download and run one `.exe`.
- **No extra runtime dependency:** release builds are self-contained; users do not need to install .NET.
- **Per-display bindings:** map any installed display and resolution to a global hotkey.
- **Safe hotkey changes:** the app warns before replacing an existing hotkey binding.
- **User-controlled startup:** enable or disable “Start when I sign in” from the tray menu.
- **Local-only configuration:** bindings are stored in the current user’s AppData profile.

## Download and run

1. Open the repository’s [Releases](../../releases) page.
2. Download `ResolutionChanger.exe` and `ResolutionChanger.exe.sha256` from the latest release.
3. Optionally verify the download in PowerShell:

   ```powershell
   $expected = (Get-Content .\ResolutionChanger.exe.sha256).Split(' ')[0]
   $actual = (Get-FileHash .\ResolutionChanger.exe -Algorithm SHA256).Hash.ToLowerInvariant()
   if ($actual -eq $expected) { 'Checksum verified.' } else { throw 'Checksum mismatch.' }
   ```

4. Run `ResolutionChanger.exe`.

The application starts in the notification area. Right-click its icon and choose **Open bindings**.

> Windows may show a reputation warning for an unsigned executable. Only run files downloaded from a release you trust,
> and verify the checksum above.

## Create a resolution hotkey

1. In **Open bindings**, right-click the grid and select **Add binding**.
2. Click the **Display & Resolution** cell to select an installed display and resolution.
    - Enter a width and height manually, or choose from the grouped resolution picker.
    - Resolutions supported by the selected display are marked.
3. Click the **Hotkey** cell and press the desired shortcut.
4. Approve the change. If the shortcut is already assigned, confirm whether to replace the old binding.

The shortcut works globally while Resolution Changer is running. To launch it automatically after you sign in, select *
*Start when I sign in** in the tray menu.

## Requirements and limitations

- Windows only; the app uses Windows display and global-hotkey APIs.
- A resolution must be supported by the target display and graphics driver for Windows to apply it.
- Resolution Changer changes the display mode only; it does not manage refresh rate, HDR, or monitor layout.

## Build from source

Install the .NET 10 SDK, then run:

```powershell
dotnet tool restore
dotnet csharpier check ResolutionChanger
dotnet format ResolutionChanger.slnx --verify-no-changes --no-restore
dotnet build ResolutionChanger.slnx --configuration Release --no-restore
```

Create the distributable executable with:

```powershell
dotnet publish ResolutionChanger/ResolutionChanger.csproj --configuration Release
```

The self-contained single-file output is written under:

```text
ResolutionChanger/bin/Release/net10.0-windows/win-x64/publish/ResolutionChanger.exe
```

## Project layout

- `ResolutionChanger/Models` — immutable binding and display data.
- `ResolutionChanger/NativeMethods` — isolated Windows API interop.
- `ResolutionChanger` root — tray, UI, persistence, display, and hotkey services.
- `.github/workflows` — reusable validation and release publication workflows.

## Quality gates

Pull requests run the same checks listed above: CSharpier formatting, .NET style validation, structural validation, and
a Release build. Releases validate the tagged source before publishing the executable and its SHA-256 checksum.

# Attribution

- The app icon is from [Magnific - Flaticon](https://www.flaticon.com/free-icons/display-size) and licensed for free use
  with attribution.
