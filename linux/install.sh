#!/usr/bin/env bash
set -e

# Target paths
BIN_DIR="${HOME}/.local/bin"
APP_DIR="${HOME}/.local/share/applications"
ICON_DIR="${HOME}/.local/share/icons/hicolor/256x256/apps"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

if [ "$1" == "--uninstall" ]; then
    echo "=== Uninstalling FlickerFree PWM Dimmer ==="
    rm -f "${BIN_DIR}/flickerfree-dimmer"
    rm -f "${APP_DIR}/flickerfree-dimmer.desktop"
    rm -f "${ICON_DIR}/flickerfree-dimmer.png"
    echo "Uninstalled successfully."
    exit 0
fi

echo "========================================="
echo " Installing FlickerFree PWM Dimmer       "
echo " Zero PWM Flicker Screen Dimmer for Linux"
echo "========================================="

# Create directories
mkdir -p "${BIN_DIR}"
mkdir -p "${APP_DIR}"
mkdir -p "${ICON_DIR}"

# Copy binary & assets
echo "[1/4] Installing executable to ${BIN_DIR}/flickerfree-dimmer..."
cp "${SCRIPT_DIR}/flickerfree-dimmer" "${BIN_DIR}/flickerfree-dimmer"
chmod +x "${BIN_DIR}/flickerfree-dimmer"

echo "[2/4] Installing high-res application icon..."
if [ -f "${SCRIPT_DIR}/app.png" ]; then
    cp "${SCRIPT_DIR}/app.png" "${ICON_DIR}/flickerfree-dimmer.png"
elif [ -f "${SCRIPT_DIR}/../app.png" ]; then
    cp "${SCRIPT_DIR}/../app.png" "${ICON_DIR}/flickerfree-dimmer.png"
fi

echo "[3/4] Installing desktop entry..."
cp "${SCRIPT_DIR}/flickerfree-dimmer.desktop" "${APP_DIR}/flickerfree-dimmer.desktop"
chmod +x "${APP_DIR}/flickerfree-dimmer.desktop"

# Refresh system caches
if command -v update-desktop-database >/dev/null 2>&1; then
    update-desktop-database "${APP_DIR}" >/dev/null 2>&1 || true
fi
if command -v gtk-update-icon-cache >/dev/null 2>&1; then
    gtk-update-icon-cache -f -t "${HOME}/.local/share/icons/hicolor" >/dev/null 2>&1 || true
fi

echo "[4/4] Verifying dependencies..."
MISSING=""
if ! command -v python3 >/dev/null 2>&1; then MISSING="${MISSING} python3"; fi
if ! command -v xrandr >/dev/null 2>&1; then MISSING="${MISSING} x11-xserver-utils(xrandr)"; fi

if [ -n "${MISSING}" ]; then
    echo "Notice: Some recommended tools are missing: ${MISSING}"
    echo "To install on Ubuntu/Debian: sudo apt install python3 python3-gi python3-gi-cairo gir1.2-gtk-3.0 x11-xserver-utils"
    echo "To install on Fedora: sudo dnf install python3-gobject gtk3 xrandr"
    echo "To install on Arch: sudo pacman -S python-gobject gtk3 xorg-xrandr"
fi

echo ""
echo "Installation complete!"
echo "You can launch 'FlickerFree PWM Dimmer' from your Application Menu,"
echo "or run directly in your terminal: ~/.local/bin/flickerfree-dimmer"
