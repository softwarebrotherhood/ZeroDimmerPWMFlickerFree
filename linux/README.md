# ZeroDimmer for Linux — Zero-PWM Screen Dimmer (X11 & Wayland)

> **Zero-PWM software screen dimmer for Linux desktops (Ubuntu, Debian, Fedora, Arch, Linux Mint, Pop!_OS, openSUSE).**  
> Completely eliminates eye strain, headaches, and migraines caused by Pulse-Width Modulation (PWM) backlight flickering by keeping hardware backlight at 100% constant DC and dimming pixels via `xrandr` gamma ramps or Wayland compositor overlays.

[![Linux Compatible](https://img.shields.io/badge/Linux-X11%20%26%20Wayland-FCC624?logo=linux&logoColor=black)](https://github.com/softwarebrotherhood/ZeroDimmerPWMFlickerFree)
[![GTK 3 & PyGObject](https://img.shields.io/badge/GTK-3.0%20Adwaita-4A90E2?logo=gnome&logoColor=white)](https://www.gtk.org/)
[![Python 3](https://img.shields.io/badge/Python-3.8%2B-3776AB?logo=python&logoColor=white)](https://www.python.org/)
[![Support on Ko-fi](https://img.shields.io/badge/Ko--fi-Support%20Project-FF5E5B?logo=ko-fi&logoColor=white)](https://ko-fi.com/softwarebrotherhood)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

---

## 📌 Table of Contents
- [Why Linux Users Need ZeroDimmer](#-why-linux-users-need-zerodimmer)
- [How It Works: X11 & Wayland Dual-Engine](#-how-it-works-x11--wayland-dual-engine)
- [Key Features](#-key-features)
- [Quick 1-Click Install (No Sudo Required)](#-quick-1-click-install-no-sudo-required)
- [Distribution Dependencies](#-distribution-dependencies)
- [Desktop Integration & Appearance](#-desktop-integration--appearance)
- [Eye Care & Circadian Tints](#-eye-care--circadian-tints)
- [Uninstalling](#-uninstalling)
- [License](#-license)

---

## ⚡ Why Linux Users Need ZeroDimmer

Most modern laptops and monitors use **Pulse-Width Modulation (PWM)** to dim backlights. Lowering hardware brightness via `brightnessctl`, `xbacklight`, or GNOME/KDE sliders triggers high-frequency strobing that causes **headaches, dry eyes, and migraine fatigue**.

- **The Problem:** The backlight rapidly switches on and off hundreds of times per second.
- **The ZeroDimmer Solution:** Keep hardware backlight at **100%** (constant direct current with zero PWM strobing), and use ZeroDimmer to comfortably darken screen pixels in software.

---

## 🔬 How It Works: X11 & Wayland Dual-Engine

ZeroDimmer automatically detects your display server (`XDG_SESSION_TYPE`) and selects the optimal dimming pipeline:

### 1. X11 Hardware-Free Gamma Dimming (Default on X11)
- Uses `xrandr --gamma` hardware video card ramps to scale pixel intensity across all connected monitors.
- **0% CPU usage**: The GPU handles the gamma transformation with zero performance penalty.
- **Uniform Coverage**: Dims the entire display buffer including GNOME Shell top bars, KDE panels, docks, context menus, and notifications with **zero bright leaks**.

### 2. Wayland Click-Through Layer Overlay (Default on Wayland)
- Spans transparent fullscreen Cairo/GTK windows across all active monitors.
- Uses `gdk_window_input_shape_combine_region` with an empty mask to make the window **100% click-through**.
- All mouse clicks, gestures, drag-and-drop operations, and keyboard focus pass seamlessly to underlying applications.

---

## ✨ Key Features

- 🛡️ **Zero PWM Flicker:** Keeps hardware backlight at 100% DC to stop strobing on OLED, mini-LED, and IPS displays.
- 🐧 **Native GNOME HeaderBar & StackSwitcher:** Authentic Adwaita look and feel with integrated window controls.
- 🌓 **System Dark & Light Theme Compliance:** Seamlessly respects GTK3 dark theme preferences.
- ☕ **System Tray & Ayatana AppIndicator:** Unobtrusive tray icon with quick darkness presets and instant controls.
- 🔒 **Fail-Safe Signal Handlers:** Registers `SIGINT`, `SIGTERM`, and `atexit` hooks to automatically restore normal gamma ramps if the application closes or crashes.
- 🎨 **Circadian Eye Care Filters:** Neutral Black, Warm Amber, Cozy Candle, and Soft Green blue-light filters.
- 🔒 **100% Offline & Private:** Zero telemetry, no network calls.

---

## 🚀 Quick 1-Click Install (No Sudo Required)

```bash
git clone https://github.com/softwarebrotherhood/ZeroDimmerPWMFlickerFree.git
cd ZeroDimmerPWMFlickerFree/linux
chmod +x install.sh
./install.sh
```

This installs:
- Executable to `~/.local/bin/zerodimmer`
- Desktop shortcut to `~/.local/share/applications/zerodimmer.desktop`
- High-resolution icons to `~/.local/share/icons/`

---

## 📦 Distribution Dependencies

### Ubuntu / Debian / Linux Mint / Pop!_OS
```bash
sudo apt update
sudo apt install python3 python3-gi python3-gi-cairo gir1.2-gtk-3.0 gir1.2-ayatanaappindicator3-0.1 x11-xserver-utils
```

### Fedora / RHEL / CentOS
```bash
sudo dnf install python3-gobject gtk3 libayatana-appindicator-gtk3 xrandr
```

### Arch Linux / Manjaro / EndeavourOS
```bash
sudo pacman -S python-gobject gtk3 libayatana-appindicator xorg-xrandr
```

### openSUSE
```bash
sudo zypper install python3-gobject typelib-1_0-Gtk-3_0 libayatana-appindicator3-1 xrandr
```

---

## 🎨 Eye Care & Circadian Tints

1. **Neutral Black:** Pure brightness reduction with neutral color fidelity.
2. **Warm Amber:** Blue-light filter blocking aggressive wavelengths for evening work.
3. **Cozy Candle:** Warm glow designed for dark rooms and late-night coding.
4. **Soft Green:** Calming high-contrast tint optimized for reading code and documentation.

---

## 🗑️ Uninstalling

To cleanly remove ZeroDimmer and its desktop entries:
```bash
cd ZeroDimmerPWMFlickerFree/linux
./install.sh --uninstall
```

---

## 📄 License
Distributed under the [MIT License](https://opensource.org/licenses/MIT).
