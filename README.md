# ZeroDimmer 🛡️ — Free & Open Source Zero-Flicker PWM Screen Dimmer

> **Eliminate screen flicker, eye strain, tension headaches, and migraines on OLED, mini-LED, and IPS displays.**  
> Keep hardware backlight at 100% constant direct current (DC) and dim your screen comfortably via pure software composition.

[![Windows 10 & 11](https://img.shields.io/badge/Windows-10%20%7C%2011-0078D6?logo=windows&logoColor=white)](windows/)
[![macOS Apple Silicon & Intel](https://img.shields.io/badge/macOS-12%2B%20(Monterey%20--%20Sequoia)-000000?logo=apple&logoColor=white)](macos/)
[![Linux X11 & Wayland](https://img.shields.io/badge/Linux-X11%20%26%20Wayland-FCC624?logo=linux&logoColor=black)](linux/)
[![Release: v1.1.0](https://img.shields.io/badge/Release-v1.1.0-0284C7?logo=github&logoColor=white)](https://github.com/softwarebrotherhood/ZeroDimmerPWMFlickerFree/releases)
[![Official Website](https://img.shields.io/badge/Website-Live%20Demo-38BDF8?logo=googlechrome&logoColor=white)](https://softwarebrotherhood.github.io/ZeroDimmerPWMFlickerFree/)
[![Support on Ko-fi](https://img.shields.io/badge/Ko--fi-Support%20Project-FF5E5B?logo=ko-fi&logoColor=white)](https://ko-fi.com/softwarebrotherhood)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

---

## 📌 Table of Contents
- [Instant Downloads](#-instant-downloads)
- [The Problem: Why Hardware Brightness Flickers (PWM)](#-the-problem-why-hardware-brightness-flickers-pwm)
- [The Solution: How ZeroDimmer Works](#-the-solution-how-zerodimmer-works)
- [Key Features & Benefits](#-key-features--benefits)
- [Multi-Platform Architecture Comparison](#-multi-platform-architecture-comparison)
- [Platform Quick Guides (Windows, macOS, Linux)](#-platform-quick-guides)
- [Circadian Eye Care & Night Tints](#-circadian-eye-care--night-tints)
- [Changelog & Version History](#-changelog--version-history)
- [Frequently Asked Questions (FAQ)](#-frequently-asked-questions-faq)
- [Privacy, Safety & Hardware Integrity](#-privacy-safety--hardware-integrity)
- [Support & Community](#-support--community)
- [License](#-license)

---

## 📥 Instant Downloads

Download official standalone installers and portable packages for your operating system:

| Platform | Download Package | Type | Compatibility |
| :--- | :--- | :--- | :--- |
| **🪟 Windows** | [**Download Windows Installer (.exe)**](https://github.com/softwarebrotherhood/ZeroDimmerPWMFlickerFree/raw/main/windows/bin/Release/net9.0-windows/installer/ZeroDimmer_Setup_v1.1.0.exe) | Inno Setup | Windows 10 & 11 (x64 / ARM64) |
| **🪟 Windows Portable** | [**Download Portable ZIP**](https://github.com/softwarebrotherhood/ZeroDimmerPWMFlickerFree/tree/main/windows/bin/Release/net9.0-windows/publish) | Standalone `.exe` | Windows 10 & 11 (x64) |
| **🍎 macOS** | [**macOS App Package & Guide**](macos/) | `.app` / Swift SPM | Apple Silicon (M1/M2/M3/M4) & Intel |
| **🐧 Linux** | [**Linux 1-Click Installer**](linux/) | Desktop Script / App | X11 & Wayland (Universal) |

> ☕ **Enjoying comfortable, headache-free screen time?** Support ongoing multi-platform updates on **[Ko-fi: ko-fi.com/softwarebrotherhood](https://ko-fi.com/softwarebrotherhood)**!

---

## ⚡ The Problem: Why Hardware Brightness Flickers (PWM)

Most modern laptops, smartphones, and desktop monitors (especially **OLED**, **mini-LED Liquid Retina XDR**, and fast **IPS** panels) control perceived brightness using **Pulse-Width Modulation (PWM)**:

```
Typical Hardware Dimming (<70%):
[ON] █   █   █   █   █   █   [OFF]  --> Rapid Strobe Effect (200Hz - 2000Hz)
Result: Severe Eye Strain, Visual Fatigue, Migraines, and Dry Eyes

ZeroDimmer Method (100% DC Hardware + Software Shader Dimming):
[ON] ═════════════════════════════  --> 100% Constant Direct Current (DC)
Result: ZERO Backlight Strobing, Maximum Eye Comfort, Natural Contrast
```

- **When you lower hardware brightness (Fn keys):** The display does not lower backlight voltage. Instead, it turns the backlight completely **OFF and ON hundreds of times per second**.
- **Even when flicker is invisible to the conscious eye:** The human ciliary muscles, iris, and retina constantly strain to adapt to rapid illumination bursts. This invisible flicker triggers **asthenopia, tension headaches, ocular migraine auras, and sleep disruption**.
- **At 100% hardware brightness:** The duty cycle is **100% direct current (DC)**. The backlight remains continuously illuminated with **0 Hz flicker**!

---

## 💡 The Solution: How ZeroDimmer Works

1. **Lock Hardware at 100%:** Keep your monitor or laptop physical brightness at 100% (where PWM strobing stops completely).
2. **Dim via Software Compositor:** **ZeroDimmer** lowers pixel luminance at the operating system compositor level (before video signals reach your display).
3. **Enjoy Zero Flicker:** Your eyes receive smooth, dark, and comfortable visuals with **zero backlight strobing, zero flickering, and zero headaches**.

---

## ✨ Key Features & Benefits

- 🛡️ **Zero PWM Flicker Guaranteed:** By maintaining 100% hardware backlight duty cycle, PWM strobing is completely eliminated.
- 🖥️ **Full Multi-Monitor & Connected Displays Control:** Choose to dim all connected screens/monitors together, restrict dimming to your primary screen only, or target specific individual displays.
- 🪟 **Uniform Desktop Dimming (No Bright Leaks):** Unlike primitive browser extensions or overlay apps that leak bright taskbars, Start menus, or right-click context menus, ZeroDimmer dims the **entire display buffer uniformly**.
- 🌓 **Dynamic Light & Dark Theme Adaptation:** Adapts automatically to your Windows/macOS/Linux system theme in real-time, or lets you choose manual Light or Dark appearance.
- 📍 **Smart Window Placement & Memory:** Starts docked neatly in the bottom-right corner near your system tray; freely draggable with multi-monitor position memory.
- ⌨️ **Global Keyboard Hotkeys:** Adjust screen brightness instantly with customizable hotkeys without opening the control window.
- 🔒 **Administrator (UAC) Popups Dimming:** Includes an optional one-click fix to dim Windows elevation prompts and prevent un-dimmed flashbangs.
- 🎨 **Circadian Night Tints:** Features Neutral Black, Warm Amber, Cozy Candle, and Soft Green blue-light filters for nighttime working comfort.
- 🚀 **Ultra-Low Resource Usage:** Written in native C# / .NET 9 WPF (Windows), native SwiftUI (macOS), and GTK3 (Linux) for near-zero CPU and RAM footprint.
- 🔒 **100% Offline & Private:** Zero telemetry, no background network pings, no account needed.

---

## 🖥️ Multi-Platform Architecture Comparison

| Feature | Windows 🪟 | macOS 🍎 | Linux 🐧 |
| :--- | :--- | :--- | :--- |
| **Primary Engine** | DWM Magnification API (`MagSetFullscreenColorEffect`) | CoreGraphics Transfer Table (`CGSetDisplayTransferByTable`) | `xrandr` Hardware Gamma Ramps (0% CPU) |
| **Fallback Engine** | `WS_EX_TRANSPARENT` Overlay Window | Multi-Screen `.screenSaver` Layer Overlay | GTK 3 Click-Through Transparent Window |
| **Taskbar / Panel Dimmed** | ✅ Yes (Post-DWM Composition) | ✅ Yes (Menu Bar + Dock) | ✅ Yes (Panels, Docks, Trays) |
| **Context Menus & Flyouts** | ✅ Uniformly Dimmed | ✅ Uniformly Dimmed | ✅ Uniformly Dimmed |
| **Elevation Dialogs** | ✅ Yes (via Secure Desktop bypass) | ✅ Yes | ✅ Yes (Polkit / Sudo) |
| **Technology Stack** | C# 12 / .NET 9 WPF | Swift 5.7+ / SwiftUI / AppKit | Python 3 / GTK 3 / PyGObject |

---

## 🚀 Platform Quick Guides

### 🪟 Windows (10 & 11)
- **Installer:** Download and run [`ZeroDimmer_Setup_v1.1.0.exe`](https://github.com/softwarebrotherhood/ZeroDimmerPWMFlickerFree/raw/main/windows/bin/Release/net9.0-windows/installer/ZeroDimmer_Setup_v1.1.0.exe).
- **Global Hotkeys:**
  - <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>↓</kbd> : Darken screen (+5%)
  - <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>↑</kbd> : Brighten screen (-5%)
  - <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>D</kbd> : Toggle Dimmer On/Off
  - <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>Esc</kbd> : Emergency Safety Reset (0% darkness)
- Read the full [Windows Documentation](windows/README.md).

### 🍎 macOS (Monterey to Sequoia)
- **Quick Build / Run:**
  ```bash
  cd macos
  swift run
  ```
- **Hotkeys:** <kbd>⌥ Option</kbd> + <kbd>⌘ Command</kbd> + <kbd>↓</kbd> (Dim) / <kbd>⌥ Option</kbd> + <kbd>⌘ Command</kbd> + <kbd>↑</kbd> (Brighten).
- Read the full [macOS Documentation](macos/README.md).

### 🐧 Linux (X11 & Wayland)
- **1-Click Install:**
  ```bash
  git clone https://github.com/softwarebrotherhood/ZeroDimmerPWMFlickerFree.git
  cd ZeroDimmerPWMFlickerFree/linux
  chmod +x install.sh
  ./install.sh
  ```
- Read the full [Linux Documentation](linux/README.md).

---

## 🎨 Circadian Eye Care & Night Tints

ZeroDimmer includes four specialized color calibration modes to reduce optical fatigue during different times of the day:

1. **Neutral Black (Default):** Pure luminescence reduction. Retains accurate sRGB color balance with zero color tinting.
2. **Warm Amber (Night Filter):** Blocks harsh blue wavelengths below 480nm to support natural melatonin production.
3. **Cozy Candle (Late Night):** Ultra-warm deep amber glow designed for dark rooms and late-night coding or reading sessions.
4. **Soft Green (High Contrast):** Calming tint optimized for reading documentation, spreadsheets, and dense code without optical glare.

---

## 📜 Changelog & Version History

### [v1.1.0] — Multi-Monitor & Independent Display Controls
- **🖥️ Multi-Monitor & Connected Displays Engine**:
  - **All Screens (Unified)**: Dims all connected screens simultaneously with a synchronized master darkness level.
  - **Separate Per Screen Controls**: Set independent darkness levels per display (e.g. 50% on laptop OLED, 20% on external IPS monitor).
  - **Interactive Display Switcher Chips**: One-click screen selection buttons showing live per-monitor percentages (`[Display 1: 45%]`, `[Display 2: 20%]`).
  - **Per-Monitor Quick Sliders**: Dedicated inline sliders and `[-]` / `[+]` steppers for each detected display.
  - **Dynamic Display Detection**: Real-time hotplug detection (`SystemEvents.DisplaySettingsChanged`) that updates automatically when displays or docks are connected/disconnected.
  - **System Tray Display Switcher**: Switch between "All Screens (Unified)" and "Separate Per Screen" directly from the taskbar context menu.
- **📊 Analytics & Web Integration**:
  - Added Google Tag Manager (`GTM-5KZH8C87`) & Search Console verification.
  - Added interactive Changelog section to the official website and documentation.
- **🪟 Windows UX Refinements**:
  - Enhanced bottom-right placement memory across mixed-DPI multi-monitor layouts.
  - Native Top Tabs and dynamic light/dark theme adaptation.
  - Freshly compiled Inno Setup installer: `ZeroDimmer_Setup_v1.1.0.exe`.

### [v1.0.0] — Initial Release
- **Initial ZeroDimmer Launch**:
  - Zero PWM flicker elimination by maintaining 100% constant DC backlight duty cycle.
  - Dual dimming engines: Deep DWM Magnification pipeline (`MagSetFullscreenColorEffect`) and Classic Transparent Layered Overlay.
  - Full desktop and taskbar coverage with zero white bright leaks or flashing popups.
  - 4 Circadian night tints: Neutral Black, Warm Amber, Cozy Candle, Soft Green.
  - Global hotkeys (<kbd>Ctrl</kbd>+<kbd>Alt</kbd>+<kbd>Up</kbd>/<kbd>Down</kbd>, <kbd>Ctrl</kbd>+<kbd>Alt</kbd>+<kbd>D</kbd>, <kbd>Ctrl</kbd>+<kbd>Alt</kbd>+<kbd>Esc</kbd>) with floating OSD pill feedback.
  - Inno Setup installer (per-user `%LOCALAPPDATA%`, no admin rights required) and portable standalone package.
  - macOS CoreGraphics transfer table engine and Linux xrandr / Wayland overlay implementations.
  - Ko-fi support integration for `softwarebrotherhood`.

---

## ❓ Frequently Asked Questions (FAQ)

### What is PWM screen flicker?
Pulse-Width Modulation (PWM) is a method display manufacturers use to simulate lower screen brightness by rapidly cycling the backlight on and off. At 50% brightness, the backlight is off half the time. This rapid strobing occurs between 200Hz and 2000Hz, causing subconscious ocular fatigue and migraines.

### How does ZeroDimmer eliminate PWM flicker compared to regular brightness controls?
Physical brightness keys (Fn keys) decrease the backlight duty cycle, increasing the flicker interval. **ZeroDimmer** keeps the physical backlight running at 100% continuous direct current (0 Hz flicker) and dims the screen by applying a software color transformation to pixels before they are displayed.

### Does ZeroDimmer dim the Windows Taskbar, Start Menu, and context menus?
**Yes!** Unlike simple transparent window overlays, ZeroDimmer’s primary engine hooks directly into the Windows Desktop Window Manager (DWM) composition pipeline using the Windows Magnification API (`MagSetFullscreenColorEffect`), ensuring every UI element, popup, menu, and taskbar is dimmed uniformly.

### Does ZeroDimmer protect OLED and mini-LED displays?
**Yes.** Because ZeroDimmer lowers the actual RGB sub-pixel output values, OLED and mini-LED displays draw less power and emit less light per diode, providing exceptional black levels and reducing burn-in risk while eliminating PWM strobing.

### How do I exit if my screen becomes too dark?
ZeroDimmer has built-in hardware safety caps (maximum 90% software darkness). If you ever need an immediate reset, press the global emergency safety hotkey:  
- **Windows:** <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>Esc</kbd>
- **macOS:** <kbd>⌥ Option</kbd> + <kbd>⌘ Command</kbd> + <kbd>Esc</kbd>  
This instantly restores display brightness to 0% (normal).

### Is ZeroDimmer free, open source, and offline?
**Yes.** ZeroDimmer is 100% free, open source under the MIT License, and functions entirely offline without telemetry or network tracking.

---

## 🛡️ Privacy, Safety & Hardware Integrity

- **100% Offline & Private:** Zero network requests, zero telemetry, zero analytics.
- **Hardware Safe:** Never touches monitor firmware, DDC/CI volatile memory, or hardware voltage regulators.
- **Fail-Safe Design:** Automatically restores normal brightness when closing, sleeping, or restarting your computer.

---

## 💖 Support & Community

If **ZeroDimmer** has helped relieve your eye fatigue, eliminate migraines, or made your daily computer work more comfortable, please star this repository and consider supporting ongoing multi-platform development!

[![Support on Ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/softwarebrotherhood)

- Direct Ko-fi Link: **[ko-fi.com/softwarebrotherhood](https://ko-fi.com/softwarebrotherhood)**
- GitHub Repository: **[softwarebrotherhood/ZeroDimmerPWMFlickerFree](https://github.com/softwarebrotherhood/ZeroDimmerPWMFlickerFree)**

---

## 📄 License

Distributed under the [MIT License](https://opensource.org/licenses/MIT).
