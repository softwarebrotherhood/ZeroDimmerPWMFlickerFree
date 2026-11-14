import Cocoa
import CoreGraphics

public class DimmerEngine {
    public static let shared = DimmerEngine()

    private var originalRamps: [CGDirectDisplayID: ([CGGammaValue], [CGGammaValue], [CGGammaValue])] = [:]
    private var overlayWindows: [NSWindow] = []
    private var isDwmMode: Bool = true
    private var currentLevel: Int = 0
    private var currentTint: String = "NeutralBlack"
    private var isEnabled: Bool = true

    init() {
        saveOriginalGamma()
        NotificationCenter.default.addObserver(
            self,
            selector: #selector(screenParametersChanged),
            name: NSApplication.didChangeScreenParametersNotification,
            object: nil
        )
    }

    private func saveOriginalGamma() {
        var displayCount: UInt32 = 0
        CGGetOnlineDisplayList(0, nil, &displayCount)
        var displays = [CGDirectDisplayID](repeating: 0, count: Int(displayCount))
        CGGetOnlineDisplayList(displayCount, &displays, &displayCount)

        for d in displays {
            var red = [CGGammaValue](repeating: 0, count: 256)
            var green = [CGGammaValue](repeating: 0, count: 256)
            var blue = [CGGammaValue](repeating: 0, count: 256)
            var sampleCount: UInt32 = 0

            if CGGetDisplayTransferByTable(d, 256, &red, &green, &blue, &sampleCount) == .success {
                originalRamps[d] = (red, green, blue)
            }
        }
    }

    @objc private func screenParametersChanged() {
        rebuildOverlays()
        applyCurrentState()
    }

    public func update(level: Int, enabled: Bool, tint: String, useCoreGraphics: Bool = true) {
        currentLevel = max(0, min(90, level))
        isEnabled = enabled
        currentTint = tint
        isDwmMode = useCoreGraphics

        applyCurrentState()
    }

    private func applyCurrentState() {
        if isDwmMode {
            // Hide any window overlays
            clearOverlays()

            if !isEnabled || currentLevel <= 0 {
                restoreGamma()
                return
            }

            let factor = Float(100 - currentLevel) / 100.0

            var rFactor = factor
            var gFactor = factor
            var bFactor = factor

            switch currentTint {
            case "WarmAmber":
                gFactor = factor * 0.84
                bFactor = factor * 0.52
            case "Candle":
                gFactor = factor * 0.70
                bFactor = factor * 0.38
            case "SoftGreen":
                rFactor = factor * 0.78
                bFactor = factor * 0.78
            default:
                break
            }

            var displayCount: UInt32 = 0
            CGGetOnlineDisplayList(0, nil, &displayCount)
            var displays = [CGDirectDisplayID](repeating: 0, count: Int(displayCount))
            CGGetOnlineDisplayList(displayCount, &displays, &displayCount)

            for d in displays {
                var red = [CGGammaValue](repeating: 0, count: 256)
                var green = [CGGammaValue](repeating: 0, count: 256)
                var blue = [CGGammaValue](repeating: 0, count: 256)

                for i in 0..<256 {
                    let base = Float(i) / 255.0
                    red[i] = CGGammaValue(base * rFactor)
                    green[i] = CGGammaValue(base * gFactor)
                    blue[i] = CGGammaValue(base * bFactor)
                }

                CGSetDisplayTransferByTable(d, 256, &red, &green, &blue)
            }
        } else {
            // Overlay window mode
            restoreGamma()

            if overlayWindows.isEmpty {
                rebuildOverlays()
            }

            let opacity = isEnabled ? CGFloat(currentLevel) / 100.0 : 0.0
            let color = getTintColor().withAlphaComponent(opacity)

            for win in overlayWindows {
                win.backgroundColor = color
                win.orderFrontRegardless()
            }
        }
    }

    private func getTintColor() -> NSColor {
        switch currentTint {
        case "WarmAmber":
            return NSColor(red: 0.18, green: 0.10, blue: 0.02, alpha: 1.0)
        case "Candle":
            return NSColor(red: 0.22, green: 0.11, blue: 0.03, alpha: 1.0)
        case "SoftGreen":
            return NSColor(red: 0.03, green: 0.15, blue: 0.07, alpha: 1.0)
        default:
            return NSColor.black
        }
    }

    private func rebuildOverlays() {
        clearOverlays()

        for screen in NSScreen.screens {
            let win = NSWindow(
                contentRect: screen.frame,
                styleMask: [.borderless],
                backing: .buffered,
                defer: false,
                screen: screen
            )
            win.level = .screenSaver
            win.ignoresMouseEvents = true
            win.isOpaque = false
            win.hasShadow = false
            win.backgroundColor = NSColor.clear
            win.collectionBehavior = [.canJoinAllSpaces, .stationary, .ignoresCycle]

            win.orderFrontRegardless()
            overlayWindows.append(win)
        }
    }

    private func clearOverlays() {
        for win in overlayWindows {
            win.orderOut(nil)
        }
        overlayWindows.removeAll()
    }

    public func restoreGamma() {
        for (d, ramps) in originalRamps {
            var red = ramps.0
            var green = ramps.1
            var blue = ramps.2
            CGSetDisplayTransferByTable(d, 256, &red, &green, &blue)
        }
    }

    public func cleanup() {
        restoreGamma()
        clearOverlays()
    }
}
