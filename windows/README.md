# FlickerFree PWM Dimmer for Windows

[![Support on Ko-fi](https://img.shields.io/badge/Ko--fi-Support%20Project-FF5E5B?logo=ko-fi&logoColor=white)](https://ko-fi.com/softwarebrotherhood)

Zero-PWM software screen dimmer for Windows 10 (Build 1607+) and Windows 11.

Eliminates eye strain, headaches, and migraines caused by Pulse-Width Modulation (PWM) backlight flickering on laptops and external displays by keeping hardware backlight at 100% constant DC and dimming pixels via the Windows Desktop Window Manager (DWM) composition pipeline.

---

## ✨ Features

- **DWM Deep Dimmer Engine (Default):**
  - Uses Windows Magnification API (`MagSetFullscreenColorEffect`) at post-composition stage.
  - **Dims EVERYTHING uniformly:** Windows Taskbar, Start Menu, right-click context menus, notification flyouts, tooltips, and popups with **zero bright leaks**!
- **Classic Window Overlay Engine (Alternative):**
  - 100% click-through transparent layered window (`WS_EX_TRANSPARENT | WS_EX_LAYERED`).
- **Circadian Eye Care Tints:**
  - Neutral Black, Warm Amber (night blue-light filter), Cozy Candle, and Soft Green.
- **Global Hotkeys:**
  - <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>↓</kbd> : Darken screen (+5%)
  - <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>↑</kbd> : Brighten screen (-5%)
  - <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>D</kbd> : Toggle Dimmer On/Off
  - <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>Esc</kbd> : Safety Reset (0% darkness)
- **Floating On-Screen Display (OSD):** Modern floating pill indicator showing brightness adjustments in real-time.
- **UAC Elevation Support:** One-click fix to dim "Run as administrator" elevation dialogs.

---

## 📥 Installation

### 1. Inno Setup Installer (.exe)
Download and run:
`bin/Release/net9.0-windows/installer/FlickerFreeDimmer_Setup_v1.0.0.exe`
- Single-click setup with automatic desktop and Start Menu shortcuts.
- No administrator privileges required to install.

### 2. Standalone Portable
Run directly without installing from `bin/Release/net9.0-windows/publish/AntiFlickerDimmer.exe`.

---

## 🛠️ Building from Source

### Prerequisites
- Windows 10 / 11
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- (Optional) [Inno Setup 6](https://jrsoftware.org/isdl.php) to build installer

### Quick Build & Run
```powershell
cd windows
dotnet run -c Release
```

### Compile Release & Installer
```powershell
dotnet publish AntiFlickerDimmer.csproj -c Release -r win-x64 --self-contained false -o bin/Release/net9.0-windows/publish
iscc installer.iss
```
