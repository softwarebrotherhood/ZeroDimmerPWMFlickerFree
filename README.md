# ZeroDimmer 🛡️🌙

[![Windows](https://img.shields.io/badge/Windows-10%20%7C%2011-0078D6?logo=windows&logoColor=white)](windows/)
[![macOS](https://img.shields.io/badge/macOS-12%2B%20(Monterey%20--%20Sequoia)-000000?logo=apple&logoColor=white)](macos/)
[![Linux](https://img.shields.io/badge/Linux-X11%20%26%20Wayland-FCC624?logo=linux&logoColor=black)](linux/)
[![Support on Ko-fi](https://img.shields.io/badge/Ko--fi-Support%20Project-FF5E5B?logo=ko-fi&logoColor=white)](https://ko-fi.com/softwarebrotherhood)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

---

### 📥 Instant Downloads

| Platform | Download Package | Format | Architecture |
| :--- | :--- | :--- | :--- |
| **Windows** | [**Download Windows Installer**](https://github.com/softwarebrotherhood/FlikerFreePWMDimmer/raw/main/windows/bin/Release/net9.0-windows/installer/ZeroDimmer_Setup_v1.0.0.exe) | `.exe` (Inno Setup) | x64 / ARM64 |
| **Windows Portable** | [**Download Portable ZIP**](https://github.com/softwarebrotherhood/FlikerFreePWMDimmer/tree/main/windows/bin/Release/net9.0-windows/publish) | `.zip` Standalone | x64 |
| **macOS** | [**macOS Package & Build Guide**](macos/) | `.app` / Swift SPM | Apple Silicon & Intel |
| **Linux** | [**Linux 1-Click Installer**](linux/) | `.sh` / Desktop App | X11 & Wayland (Universal) |

> ☕ **Enjoying comfortable, flicker-free screens?** Support ongoing multi-platform development on **[Ko-fi: ko-fi.com/softwarebrotherhood](https://ko-fi.com/softwarebrotherhood)**!

---

## ⚡ The Science: Why Hardware Brightness Flickers

Modern laptop screens and desktop monitors (OLED, mini-LED Liquid Retina XDR, and fast IPS panels) control backlight output via **Pulse-Width Modulation (PWM)**:

- **At lower hardware brightness (10% – 70%):** The backlight rapidly cycles between 100% ON and 0% OFF hundreds or thousands of times every second. Even when invisible to the naked eye, this rapid strobing causes severe **eye strain, tension headaches, dry eyes, and migraines**.
- **At 100% hardware brightness:** The duty cycle is **100% constant direct current (DC)**. There is **ZERO backlight strobing or flickering**!

```
Hardware Dimming (<70%):   █   █   █   █   █   █   (PWM Backlight Strobing -> Eye Fatigue)
ZeroDimmer (100% DC):      ═════════════════════════ (100% Constant DC Backlight + Software Dimming)
```

### 💡 The Solution
1. Keep physical screen brightness at **100%** (via your keyboard brightness keys or system settings).
2. Use **ZeroDimmer** to dim pixels at the operating system compositor level.
3. Enjoy a dark, comfortable screen with **0 Hz PWM flicker**!

---

## 🖥️ Platform Comparison & Architecture

| Feature | Windows 🪟 | macOS 🍎 | Linux 🐧 |
| :--- | :--- | :--- | :--- |
| **Primary Engine** | DWM Magnification API (`MagSetFullscreenColorEffect`) | CoreGraphics Transfer Table (`CGSetDisplayTransferByTable`) | `xrandr` Gamma Ramps (0% CPU) |
| **Fallback Engine** | `WS_EX_TRANSPARENT` Overlay Window | Multi-Screen `.screenSaver` Overlay | GTK 3 Click-Through Layer Overlay |
| **Taskbar / Panel Dimmed** | ✅ Yes (Post-DWM Composition) | ✅ Yes (Menu Bar + Dock) | ✅ Yes (Panels, Docks, Trays) |
| **Context Menus & Popups** | ✅ Uniformly Dimmed | ✅ Uniformly Dimmed | ✅ Uniformly Dimmed |
| **Administrator Dialogs** | ✅ Yes (via Secure Desktop bypass) | ✅ Yes | ✅ Yes (Polkit / Sudo) |
| **Technology Stack** | C# 12 / .NET 9 WPF | Swift 5.7+ / SwiftUI / AppKit | Python 3 / GTK 3 / PyGObject |

---

## 🪟 Windows Version

### Installation
- Download and run [**ZeroDimmer_Setup_v1.0.0.exe**](https://github.com/softwarebrotherhood/FlikerFreePWMDimmer/raw/main/windows/bin/Release/net9.0-windows/installer/ZeroDimmer_Setup_v1.0.0.exe).
- Installs to local app data (no admin rights needed), creates Desktop & Start Menu shortcuts.

Read the full [Windows Documentation](windows/README.md).

### Global Hotkeys
- <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>↓</kbd> : Darken screen (+5%)
- <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>↑</kbd> : Brighten screen (-5%)
- <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>D</kbd> : Toggle Dimmer On/Off
- <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>Esc</kbd> : Emergency Safety Reset (0% darkness)

### Dimming Administrator (UAC) Popups
Windows normally shifts to an isolated "Secure Desktop" for UAC prompts. To dim elevation prompts seamlessly:
- Click **"Dim Administrator Popups"** in the app UI, or run `Fix-UAC-Dimming.bat` as administrator.

---

## 🍎 macOS Version

Zero-PWM software screen dimmer for MacBook Pro (M1/M2/M3/M4 Liquid Retina XDR mini-LED), MacBook Air, and external displays.

### Quick Run / Build
```bash
cd macos
swift run
```
Or generate the `.app` bundle:
```bash
chmod +x build-app.sh
./build-app.sh
open ZeroDimmer.app
```

### Hotkeys
- <kbd>⌥ Option</kbd> + <kbd>⌘ Command</kbd> + <kbd>↓</kbd> : Darken screen
- <kbd>⌥ Option</kbd> + <kbd>⌘ Command</kbd> + <kbd>↑</kbd> : Brighten screen

Read the full [macOS Documentation](macos/README.md).

---

## 🐧 Linux Version

Zero-PWM screen dimmer for Ubuntu, Debian, Fedora, Arch, Linux Mint, Pop!_OS, and other distributions.

### 1-Click Install (No Sudo Required)
```bash
cd linux
chmod +x install.sh
./install.sh
```

### Dependencies
- **Ubuntu / Debian / Mint:**
  ```bash
  sudo apt install python3 python3-gi python3-gi-cairo gir1.2-gtk-3.0 gir1.2-ayatanaappindicator3-0.1 x11-xserver-utils
  ```
- **Fedora:**
  ```bash
  sudo dnf install python3-gobject gtk3 libayatana-appindicator-gtk3 xrandr
  ```
- **Arch Linux:**
  ```bash
  sudo pacman -S python-gobject gtk3 libayatana-appindicator xorg-xrandr
  ```

Read the full [Linux Documentation](linux/README.md).

---

## 🎨 Eye Care & Circadian Tints (All Platforms)

1. **Neutral Black:** Pure brightness reduction, zero color distortion.
2. **Warm Amber:** Gentle night filter blocking aggressive blue wavelengths.
3. **Candle Warm:** Cozy, ultra-low blue-light amber glow for late-night sessions.
4. **Soft Green:** High-contrast soothing tint for reading documentation and code.

---

## 🛡️ Privacy & Security

- **100% Offline & Private:** No telemetry, no background network calls, no analytics.
- **Hardware-Safe:** Never alters physical display firmware, DDC/CI hardware registers, or dangerous backlight voltages.
- **Fail-Safe:** Capped at 90% software darkness with hardware safety reset hotkeys to prevent pitch-black lockouts.

---

## 💖 Support & Sponsorship

If **ZeroDimmer** has saved your eyes from PWM screen strobing, relieved migraines, or improved your daily working comfort, please consider buying a coffee or becoming a supporter!

[![Support on Ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/softwarebrotherhood)

Direct Link: **[ko-fi.com/softwarebrotherhood](https://ko-fi.com/softwarebrotherhood)**

Your support directly funds continued maintenance, multi-platform releases (Windows, macOS, Linux), and free community updates.

---

## 📄 License

Distributed under the [MIT License](https://opensource.org/licenses/MIT).
