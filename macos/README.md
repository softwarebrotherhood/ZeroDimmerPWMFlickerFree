# FlickerFree PWM Dimmer for macOS

[![Support on Ko-fi](https://img.shields.io/badge/Ko--fi-Support%20Project-FF5E5B?logo=ko-fi&logoColor=white)](https://ko-fi.com/softwarebrotherhood)

Zero-PWM software screen dimmer for Apple Silicon (M1/M2/M3/M4) and Intel Macs running macOS 12 Monterey or newer.

Eliminates eye strain, headaches, and migraines caused by PWM (Pulse-Width Modulation) backlight strobing on MacBook Liquid Retina XDR (mini-LED) and OLED/IPS screens by keeping the hardware backlight at 100% constant DC and dimming pixels via CoreGraphics gamma tables or screen overlays.

---

## ✨ Features

- **CoreGraphics Deep Dimming:** Scales display transfer tables (`CGSetDisplayTransferByTable`) to dim the entire screen uniformly, including the macOS Menu Bar and Dock, with zero CPU load.
- **Topmost Overlay Fallback:** Multi-display transparent click-through window overlay at `.screenSaver` level.
- **Menu Bar Tray App:** Lives in your status bar (`NSStatusItem`) with a SwiftUI popover.
- **Eye Care Tints:** Neutral Black, Warm Amber, Candle Warm, and Soft Green (blue-light protection).
- **System Hotkeys:**
  - <kbd>⌥ Option</kbd> + <kbd>⌘ Command</kbd> + <kbd>↓</kbd> : Increase Dimming
  - <kbd>⌥ Option</kbd> + <kbd>⌘ Command</kbd> + <kbd>↑</kbd> : Decrease Dimming
- **Sleep & Wake Aware:** Restores normal display transfer tables before sleep and restores your dimming level on wake.

---

## 🛠️ Building from Source

### Prerequisites
- macOS 12.0 Monterey or later
- Xcode or Command Line Tools (`xcode-select --install`)
- Swift 5.7+

### Build & Run
```bash
cd macos
swift run
```

### Build `.app` Bundle
```bash
chmod +x build-app.sh
./build-app.sh
```
This generates `FlickerFreeDimmer.app`. Drag it to `/Applications` or launch directly:
```bash
open FlickerFreeDimmer.app
```
