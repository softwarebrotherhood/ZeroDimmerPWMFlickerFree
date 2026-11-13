# FlickerFree Dimmer 🛡️🌙

[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%7C%20Windows%2011-0078D6?logo=windows&logoColor=white)](https://github.com/salamimarketer/FlikerFreePWMDimmer)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Download](https://img.shields.io/badge/Download-Installer%20(.exe)-0284C7?style=for-the-badge&logo=windows&logoColor=white)](https://github.com/salamimarketer/FlikerFreePWMDimmer/raw/main/bin/Release/net9.0-windows/installer/FlickerFreeDimmer_Setup_v1.0.0.exe)

> ### 📥 **[Download FlickerFree Dimmer v1.0.0 (Installer .exe)](https://github.com/salamimarketer/FlikerFreePWMDimmer/raw/main/bin/Release/net9.0-windows/installer/FlickerFreeDimmer_Setup_v1.0.0.exe)**
> Click the button or link above to download the installer for Windows 10 & 11. Includes high-resolution desktop and start menu shortcuts with no administrator rights required to install.

---

A modern, high-performance Windows desktop application designed specifically for people sensitive to **screen flickering (PWM - Pulse-Width Modulation)** on laptop screens and external displays.

---

## ⚡ The Science: Why Hardware Brightness Flickers

Many modern laptop displays (OLED, mini-LED, and IPS panels) control hardware backlight brightness using **Pulse-Width Modulation (PWM)**:
- **At lower hardware brightness (20% – 60%):** The backlight rapidly cycles between 100% ON and completely OFF hundreds of times every second. Even when invisible to the conscious eye, this rapid strobing causes severe eye fatigue, dry eyes, tension headaches, and migraines.
- **At 100% hardware brightness:** The PWM duty cycle is **100% constant ON**. There is **ZERO backlight strobing or flickering**!

### 💡 The FlickerFree Solution
1. Keep your laptop's physical screen brightness (via Windows slider or keyboard `Fn` brightness keys) at **100%**.
2. **FlickerFree Dimmer** dims the screen using software-level color processing via the Windows Desktop Window Manager (DWM) compositor.
3. You enjoy a comfortably dark display **with zero PWM flicker and zero eye strain**!

---

## ✨ Key Features

- **Full Desktop & Taskbar Deep Dimming (DWM Engine - Default):**
  - Operates inside the Windows DWM fullscreen color matrix pipeline.
  - **Dims EVERYTHING uniformly:** The Windows Taskbar, Start Menu, right-click context menus, notification flyouts, tooltips, popups, and all running applications.
  - **Zero bright leaks!** No blinding flashbangs when clicking taskbar icons or opening menus.
- **Classic Window Overlay Mode (Alternative):**
  - 100% click-through transparent layer (`WS_EX_TRANSPARENT | WS_EX_LAYERED`).
- **Global Hotkeys:** Adjust screen darkness instantly from any application or full-screen game:
  - <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>↓</kbd> : Darken screen (+5%)
  - <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>↑</kbd> : Brighten screen (-5%)
  - <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>D</kbd> : Quick Toggle Dimmer On/Off
  - <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>Esc</kbd> : Emergency Safety Reset (0% darkness)
- **Floating On-Screen Display (OSD):** Sleek floating visual pill indicating the current darkness level when pressing hotkeys.
- **Eye-Care Color Tints:**
  - **Neutral Black:** True software dimming, preserving color fidelity while reducing overall light output.
  - **Warm Amber:** Gentle night-reading blue-light filter.
  - **Cozy Candle:** Soft warm evening glow.
  - **Soft Green:** Soothing contrast tint for reading comfort.
- **Safety Dark Cap:** Capped at 90% max darkness to prevent accidental pitch-black lockouts.
- **System Tray Integration:** Runs silently in the background with quick presets (`Off`, `20%`, `40%`, `60%`, `80%`).
- **Autostart:** Optional one-click "Start with Windows" feature.

---

## 📦 Download & Installation

### Option 1: Windows Installer (Recommended)

[![Download Installer](https://img.shields.io/badge/Download_Installer-v1.0.0_(.exe)-0284C7?style=for-the-badge&logo=windows&logoColor=white)](https://github.com/salamimarketer/FlikerFreePWMDimmer/raw/main/bin/Release/net9.0-windows/installer/FlickerFreeDimmer_Setup_v1.0.0.exe)

Direct Link: **[FlickerFreeDimmer_Setup_v1.0.0.exe](https://github.com/salamimarketer/FlikerFreePWMDimmer/raw/main/bin/Release/net9.0-windows/installer/FlickerFreeDimmer_Setup_v1.0.0.exe)**

- Single-click setup with automatic desktop and Start Menu shortcuts.
- No administrator elevation needed to install.
- Clean uninstallation via Windows Settings > Installed Apps.

### Option 2: Portable Package
Run directly without installing:
- Navigate to `bin/Release/net9.0-windows/publish/` and run `AntiFlickerDimmer.exe`.

---

## 🔒 Dimming "Run as Administrator" (UAC) Popups

By default, Windows switches to an isolated "Secure Desktop" for administrator elevation prompts, which temporarily bypasses all user software.

To make administrator confirmation prompts stay dimmed:
- Click **"Dim Administrator Popups"** directly inside the app's settings card, or run `Fix-UAC-Dimming.bat` as administrator.
- This sets Windows policy `PromptOnSecureDesktop = 0`, keeping elevation prompts directly on your normal desktop where **FlickerFree Dimmer** dims them seamlessly.

---

## 🚀 Building & Running from Source

### Prerequisites
- Windows 10 (Build 1607+) or Windows 11
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Quick Start
```powershell
# Clone the repository
git clone https://github.com/salamimarketer/FlikerFreePWMDimmer.git
cd FlikerFreePWMDimmer

# Build and run
dotnet run -c Release
```

### Publish Self-Contained Executable
```powershell
dotnet publish -c Release -o ./publish
```
