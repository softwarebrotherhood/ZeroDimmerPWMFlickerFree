# ZeroDimmer for macOS — Zero-PWM Screen Dimmer for MacBook Pro & Mac

> **Zero-PWM software screen dimmer for Apple Silicon (M1/M2/M3/M4) and Intel Macs.**  
> Completely eliminates eye strain, headaches, and migraines on MacBook Liquid Retina XDR (mini-LED), OLED, and IPS displays by maintaining 100% constant DC backlight duty cycle and scaling display transfer tables via CoreGraphics.

[![macOS 12+ Compatible](https://img.shields.io/badge/macOS-12%2B%20(Monterey%20--%20Sequoia)-000000?logo=apple&logoColor=white)](https://github.com/softwarebrotherhood/ZeroDimmerPWMFlickerFree)
[![Swift 5.7+](https://img.shields.io/badge/Swift-5.7%2B-FA7343?logo=swift&logoColor=white)](https://swift.org/)
[![Apple Silicon & Intel](https://img.shields.io/badge/Platform-Apple%20Silicon%20%7C%20Intel-blue)](https://github.com/softwarebrotherhood/ZeroDimmerPWMFlickerFree)
[![Support on Ko-fi](https://img.shields.io/badge/Ko--fi-Support%20Project-FF5E5B?logo=ko-fi&logoColor=white)](https://ko-fi.com/softwarebrotherhood)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

---

## 📌 Table of Contents
- [Why Mac Users Need ZeroDimmer](#-why-mac-users-need-zerodimmer)
- [Compatible Apple Hardware](#-compatible-apple-hardware)
- [How It Works: CoreGraphics Transfer Tables](#-how-it-works-coregraphics-transfer-tables)
- [Key Features](#-key-features)
- [Keyboard Shortcuts](#-keyboard-shortcuts)
- [Eye Care & Night Filters](#-eye-care--night-filters)
- [Building & Installing from Source](#-building--installing-from-source)
- [License](#-license)

---

## ⚡ Why Mac Users Need ZeroDimmer

Apple's modern **Liquid Retina XDR displays** (featured on the 14-inch and 16-inch MacBook Pro) use thousands of mini-LED backlight zones controlled by **Pulse-Width Modulation (PWM)** at frequencies around 4,000Hz to 15,000Hz.

- **The Problem:** When you lower macOS screen brightness using your keyboard's brightness keys, the mini-LED backlight rapidly turns on and off. For sensitive eyes, this rapid strobing causes **severe eye strain, dry eyes, tension headaches, and ocular migraines**.
- **The ZeroDimmer Solution:** Keep macOS hardware brightness at **100%** (where backlight strobing completely ceases and runs on pure constant DC), and use ZeroDimmer to dim pixels in software using macOS CoreGraphics transfer tables.

---

## 🖥️ Compatible Apple Hardware

ZeroDimmer is optimized for:
- **MacBook Pro 14" & 16"** (M1 Pro/Max, M2 Pro/Max, M3 Pro/Max, M4 Pro/Max Liquid Retina XDR displays)
- **MacBook Air 13" & 15"** (M1, M2, M3 Retina displays)
- **iMac 24"** (Apple Silicon M1/M3/M4 4.5K Retina displays)
- **Mac Studio & Mac mini** with external OLED, mini-LED, or fast IPS monitors (LG UltraFine, Dell UltraSharp, Studio Display, Pro Display XDR)
- **Intel Macs** running macOS 12 Monterey or newer

---

## 🔬 How It Works: CoreGraphics Transfer Tables

ZeroDimmer uses native Apple CoreGraphics APIs to achieve smooth, hardware-accelerated dimming with **0% CPU consumption**:

1. **CoreGraphics Deep Dimmer (Primary):**
   - Direct calls to `CGSetDisplayTransferByTable` scale red, green, and blue gamma transfer ramps before the video signal leaves the GPU.
   - Dims **everything uniformly**: the macOS Menu Bar, Dock, Notification Center, mission control, and context menus with **zero bright leaks**.
2. **Topmost Overlay Engine (Fallback):**
   - Spans multi-screen transparent click-through windows at `.screenSaver` window level (`NSWindow.Level.screenSaver`) for environments where gamma ramps are restricted.
3. **Sleep & Wake Management:**
   - Registers for `NSWorkspace.willSleepNotification` and `didWakeNotification` to safely restore normal display tables before sleeping and reapply your dimming level immediately upon wake.

---

## ✨ Key Features

- 🛡️ **Zero PWM Flicker:** Stops backlight pulsing completely across all built-in and external screens.
- 🍏 **Native Apple HIG Interface:** Built in pure SwiftUI with native segmented pickers, SF Symbols, and system color adaptation.
- 🌓 **Automatic Dark / Light Mode Adaptation:** Automatically adapts to macOS system appearance changes without restarting.
- ☕ **Native Menu Bar App:** Lives unobtrusively in your macOS status bar (`NSStatusItem`) with a compact popover.
- ⌨️ **System Hotkeys:** Adjust screen brightness instantly using customizable keyboard shortcuts.
- 🎨 **Circadian Eye Care Tints:** Neutral Black, Warm Amber, Cozy Candle, and Soft Green blue-light filters.
- 🔒 **100% Private & Offline:** Zero analytics, zero network requests, zero telemetry.

---

## ⌨️ Keyboard Shortcuts

Adjust darkness on the fly without clicking the menu bar:

| Shortcut | Action |
| :--- | :--- |
| <kbd>⌥ Option</kbd> + <kbd>⌘ Command</kbd> + <kbd>↓</kbd> | Darken screen (+5%) |
| <kbd>⌥ Option</kbd> + <kbd>⌘ Command</kbd> + <kbd>↑</kbd> | Brighten screen (-5%) |
| <kbd>⌥ Option</kbd> + <kbd>⌘ Command</kbd> + <kbd>Esc</kbd> | **Emergency Reset** (Restores 0% darkness instantly) |

---

## 🎨 Eye Care & Night Filters

1. **Neutral Black:** Pure luminescence reduction without color alteration.
2. **Warm Amber:** Blue-light filter blocking aggressive wavelengths for evening work.
3. **Cozy Candle:** Warm glow designed for dark rooms and late-night coding.
4. **Soft Green:** Calming high-contrast tint optimized for reading code and documentation.

---

## 🛠️ Building & Installing from Source

### Prerequisites
- macOS 12.0 (Monterey) or later (macOS 13 Ventura, macOS 14 Sonoma, macOS 15 Sequoia)
- Xcode or Command Line Tools: `xcode-select --install`
- Swift 5.7+

### Quick Run
```bash
cd macos
swift run
```

### Build Standalone `.app` Bundle
```bash
cd macos
chmod +x build-app.sh
./build-app.sh
```
This generates `ZeroDimmer.app`. Drag it to `/Applications` or launch directly:
```bash
open ZeroDimmer.app
```

---

## 📄 License
Distributed under the [MIT License](https://opensource.org/licenses/MIT).
