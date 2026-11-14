#!/usr/bin/env bash
set -e

echo "=== Building FlickerFreeDimmer for macOS ==="

# 1. Compile release binary using Swift Package Manager
swift build -c release --arch arm64 --arch x86_64

# 2. Setup .app bundle structure
APP_NAME="FlickerFreeDimmer"
BUNDLE_DIR="${APP_NAME}.app"
CONTENTS_DIR="${BUNDLE_DIR}/Contents"
MACOS_DIR="${CONTENTS_DIR}/MacOS"
RESOURCES_DIR="${CONTENTS_DIR}/Resources"

rm -rf "${BUNDLE_DIR}"
mkdir -p "${MACOS_DIR}"
mkdir -p "${RESOURCES_DIR}"

# 3. Copy binary
cp .build/apple/Products/Release/FlickerFreeDimmer "${MACOS_DIR}/" || cp .build/release/FlickerFreeDimmer "${MACOS_DIR}/"
chmod +x "${MACOS_DIR}/FlickerFreeDimmer"

# 4. Generate Info.plist
cat << 'EOF' > "${CONTENTS_DIR}/Info.plist"
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleExecutable</key>
    <string>FlickerFreeDimmer</string>
    <key>CFBundleIdentifier</key>
    <string>com.flickerfree.dimmer</string>
    <key>CFBundleName</key>
    <string>FlickerFreeDimmer</string>
    <key>CFBundleIconFile</key>
    <string>AppIcon</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0.0</string>
    <key>CFBundleVersion</key>
    <string>1</string>
    <key>LSMinimumSystemVersion</key>
    <string>12.0</string>
    <key>LSUIElement</key>
    <true/>
    <key>NSHighResolutionCapable</key>
    <true/>
</dict>
</plist>
EOF

# 5. Create icon if iconutil is available
ICON_SRC=""
if [ -f "app.png" ]; then
    ICON_SRC="app.png"
elif [ -f "../app.png" ]; then
    ICON_SRC="../app.png"
fi

if [ -n "${ICON_SRC}" ] && command -v iconutil >/dev/null 2>&1; then
    ICONSET="AppIcon.iconset"
    mkdir -p "${ICONSET}"
    sips -z 16 16     "${ICON_SRC}" --out "${ICONSET}/icon_16x16.png" >/dev/null 2>&1 || true
    sips -z 32 32     "${ICON_SRC}" --out "${ICONSET}/icon_16x16@2x.png" >/dev/null 2>&1 || true
    sips -z 32 32     "${ICON_SRC}" --out "${ICONSET}/icon_32x32.png" >/dev/null 2>&1 || true
    sips -z 64 64     "${ICON_SRC}" --out "${ICONSET}/icon_32x32@2x.png" >/dev/null 2>&1 || true
    sips -z 128 128   "${ICON_SRC}" --out "${ICONSET}/icon_128x128.png" >/dev/null 2>&1 || true
    sips -z 256 256   "${ICON_SRC}" --out "${ICONSET}/icon_128x128@2x.png" >/dev/null 2>&1 || true
    sips -z 256 256   "${ICON_SRC}" --out "${ICONSET}/icon_256x256.png" >/dev/null 2>&1 || true
    sips -z 512 512   "${ICON_SRC}" --out "${ICONSET}/icon_256x256@2x.png" >/dev/null 2>&1 || true
    iconutil -c icns "${ICONSET}" -o "${RESOURCES_DIR}/AppIcon.icns" >/dev/null 2>&1 || true
    rm -rf "${ICONSET}"
fi

echo "Created ${BUNDLE_DIR} successfully!"
