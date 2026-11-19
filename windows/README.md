# ZeroDimmer for Windows (10 & 11) — Zero-PWM Screen Dimmer & Taskbar Darkener

> **Zero-Flicker PWM Software Screen Dimmer for Windows 10 (Build 1607+) & Windows 11.**  
> Completely eliminates backlight strobing and eye strain by maintaining 100% constant DC backlight output and applying hardware-accelerated software dimming directly to the Windows Desktop Window Manager (DWM).

[![Windows 10 & 11 Compatible](https://img.shields.io/badge/Windows-10%20%7C%2011-0078D6?logo=windows&logoColor=white)](https://github.com/softwarebrotherhood/ZeroDimmerPWMFlickerFree)
[![.NET 9.0 WPF](https://img.shields.io/badge/.NET-9.0%20WPF-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Support on Ko-fi](https://img.shields.io/badge/Ko--fi-Support%20Project-FF5E5B?logo=ko-fi&logoColor=white)](https://ko-fi.com/softwarebrotherhood)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

---

## 📌 Table of Contents
- [Why Windows Users Need ZeroDimmer](#-why-windows-users-need-zerodimmer)
- [How It Works: DWM Magnification Pipeline](#-how-it-works-dwm-magnification-pipeline)
- [Key Features](#-key-features)
- [Installation Options](#-installation-options)
- [Global Keyboard Shortcuts](#-global-keyboard-shortcuts)
- [Dimming Administrator (UAC) Popups](#-dimming-administrator-uac-popups)
- [Window Mobility & Positioning](#-window-mobility--positioning)
- [Light & Dark Mode Integration](#-light--dark-mode-integration)
- [Building from Source](#-building-from-source)
- [Troubleshooting & FAQ](#-troubleshooting--faq)

---

## ⚡ Why Windows Users Need ZeroDimmer

Most modern laptops (Dell XPS, Lenovo ThinkPad/Legion, ASUS ROG/ZenBook, HP Spectre, Microsoft Surface) and desktop gaming monitors use **Pulse-Width Modulation (PWM)** to dim screen backlights.

- **The Problem:** Pressing your laptop's brightness keys (<kbd>Fn</kbd> + <kbd>F5</kbd>/<kbd>F6</kbd>) reduces the backlight duty cycle, causing it to cycle on and off hundreds of times per second. Even when imperceptible, this rapid strobing causes **severe eye strain, tension headaches, photophobia, dry eyes, and migraines**.
- **The ZeroDimmer Solution:** Keep Windows hardware brightness at **100%** (constant direct current with zero PWM strobing), and use ZeroDimmer to comfortably darken pixels at the desktop compositor level.

---

## 🔬 How It Works: DWM Magnification Pipeline

Unlike primitive window overlay tools that merely place a transparent black window on top of running applications, ZeroDimmer provides two dimming engines:

### 1. DWM Deep Dimmer Engine (Default / Recommended)
ZeroDimmer interfaces with the Windows **Magnification API** (`MagSetFullscreenColorEffect` via `magnification.dll`):
- Operates at the **post-composition stage** of the Windows Desktop Window Manager.
- Dims **everything uniformly**: the Windows Taskbar, Start Menu, Alt+Tab switcher, right-click context menus, volume flyouts, and tooltips.
- **Zero bright leaks**: No blinding white flashes when opening menus or dragging windows across displays.
- Consumes **0% CPU** and runs on your GPU’s native composition pipeline.

### 2. Classic Transparent Overlay Engine (Fallback)
- Creates a hardware-accelerated, transparent, click-through layered window (`WS_EX_TRANSPARENT | WS_EX_LAYERED | WS_EX_TOOLWINDOW`).
- Useful for specialized virtual environments or legacy setups where DWM composition is restricted.

---

## ✨ Key Features

- 🛡️ **Zero PWM Flicker:** Stops backlight pulsing completely on OLED, mini-LED, and IPS displays.
- 🖥️ **Connected Screens & Monitors Control:** Option to dim all connected monitors simultaneously, primary screen only, or target specific individual displays.
- 🪟 **Complete Taskbar & System Coverage:** Full desktop dimming without taskbar or context menu brightness leaks.
- 🌓 **Windows Theme Integration:** Detects Windows light/dark theme live (`AppsUseLightTheme`) and updates automatically in real-time via `SystemEvents.UserPreferenceChanged`.
- 📍 **Tray-Anchored Bottom-Right Placement:** Launches docked above the system tray; freely draggable with multi-monitor memory.
- ⌨️ **Global Hotkeys & Floating OSD:** Instant dimming adjustments with a sleek on-screen display pill.
- 🔒 **Dim "Run as Admin" Dialogs:** One-click fix to ensure administrator elevation prompts are dimmed.
- 🎨 **Eye Care & Blue-Light Filters:** Neutral Black, Warm Amber, Cozy Candle, and Soft Green.
- 🔒 **100% Offline & Private:** Zero network telemetry or data collection.

---

## 📥 Installation Options

### Option 1: Standalone Windows Installer (Recommended)
1. Download [**`ZeroDimmer_Setup_v1.1.0.exe`**](https://github.com/softwarebrotherhood/ZeroDimmerPWMFlickerFree/raw/main/windows/bin/Release/net9.0-windows/installer/ZeroDimmer_Setup_v1.1.0.exe).
2. Run the installer:
   - Installs to local app data (`%LOCALAPPDATA%\Programs\ZeroDimmer`).
   - **No administrator rights required** to install.
   - Automatically creates Desktop and Start Menu shortcuts.
   - Includes optional auto-start with Windows.

### Option 2: Portable Standalone ZIP
1. Download the [**Portable Package**](https://github.com/softwarebrotherhood/ZeroDimmerPWMFlickerFree/tree/main/windows/bin/Release/net9.0-windows/publish).
2. Extract and run `ZeroDimmer.exe` directly from any folder or USB drive.

---

## ⌨️ Global Keyboard Shortcuts

Control screen darkness anywhere across Windows without bringing the control window to focus:

| Shortcut | Action |
| :--- | :--- |
| <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>↓</kbd> | Darken screen (+5%) |
| <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>↑</kbd> | Brighten screen (-5%) |
| <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>D</kbd> | Toggle Dimmer On / Off |
| <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>Esc</kbd> | **Emergency Reset** (Instantly restores 0% darkness) |

---

## 🔒 Dimming Administrator (UAC) Popups

By default, Windows switches to an isolated "Secure Desktop" when displaying User Account Control (UAC) "Run as administrator" elevation prompts. Because the Secure Desktop runs in an isolated session, it appears at un-dimmed 100% brightness.

To dim administrator prompts seamlessly:
1. Open the **⚙️ System** tab in ZeroDimmer.
2. Under **DIM "RUN AS ADMIN" POPUPS**, click **Dim Administrator Popups**.
3. *(Alternative)* Run `Fix-UAC-Dimming.bat` as administrator.

*Technical Detail:* This configures `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System\PromptOnSecureDesktop = 0`, allowing elevation dialogs to appear on your active desktop where ZeroDimmer dims them completely.

---

## 📍 Window Mobility & Positioning

- **Tray-Anchored Default:** On first launch, ZeroDimmer opens docked neatly in the bottom-right corner of your primary screen (16px above the taskbar and 16px from the right edge).
- **Draggable Anywhere:** Drag the top title bar to position the window anywhere across single or multi-monitor setups.
- **Position Persistence:** Coordinates are remembered automatically across restarts.
- **Reset Button:** Under the **⚙️ System** tab, click **`📍 Reset Position to Bottom-Right`** to quickly re-dock the window above the system tray.

---

## 🛠️ Building from Source

### Prerequisites
- Windows 10 (Build 1607+) or Windows 11
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- (Optional) [Inno Setup 6](https://jrsoftware.org/isdl.php) for installer generation

### Build & Run
```powershell
cd windows
dotnet run -c Release
```

### Publish Release Package
```powershell
dotnet publish ZeroDimmer.csproj -c Release -r win-x64 --self-contained false -o bin/Release/net9.0-windows/publish /p:DebugType=none /p:DebugSymbols=false /p:PathMap="$PWD=/_"
```

### Compile Installer
```powershell
iscc installer.iss
```

---

## 📄 License
Distributed under the [MIT License](https://opensource.org/licenses/MIT).
