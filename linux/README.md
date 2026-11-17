# FlickerFree PWM Dimmer for Linux

[![Support on Ko-fi](https://img.shields.io/badge/Ko--fi-Support%20Project-FF5E5B?logo=ko-fi&logoColor=white)](https://ko-fi.com/softwarebrotherhood)

Zero-PWM software screen dimmer for Linux desktops (Ubuntu, Debian, Fedora, Arch, Linux Mint, Pop!_OS, etc.).

Eliminates eye strain, headaches, and migraines caused by Pulse-Width Modulation (PWM) backlight flickering on laptops and desktop monitors by keeping hardware backlight at 100% constant DC and dimming pixels purely in software.

---

## ✨ Features

- **X11 Hardware-Free Dimming:** Uses `xrandr` hardware gamma ramps to dim the entire display buffer (panels, docks, menus, notifications) with **0% CPU usage** and zero bright leaks.
- **Wayland & Overlay Mode:** Multidisplay transparent click-through window overlay (`Gdk.Window.input_shape_combine_region`) for Wayland compositors (GNOME, KDE Plasma, Sway).
- **Circadian Eye Care Tints:** Neutral Black, Warm Amber, Candle Warm, and Soft Green (blue-light filtering).
- **System Tray & AppIndicator:** Compact tray icon with instant reset and control options.
- **Fail-Safe Restoration:** Clean shutdown signal handlers (`SIGINT`, `SIGTERM`, `atexit`) automatically restore normal display brightness on exit.

---

## 🚀 Quick Install (1-Click, No Sudo Required)

```bash
git clone https://github.com/salamimarketer/FlikerFreePWMDimmer.git
cd FlikerFreePWMDimmer/linux
chmod +x install.sh
./install.sh
```

### Dependencies
- **Ubuntu/Debian/Mint:**
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

---

## 🗑️ Uninstall
```bash
./install.sh --uninstall
```
