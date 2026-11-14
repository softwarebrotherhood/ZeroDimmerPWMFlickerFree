import Cocoa
import SwiftUI
import Carbon

public class AppDelegate: NSObject, NSApplicationDelegate {
    private var statusItem: NSStatusItem!
    private var popover: NSPopover!
    private var eventMonitor: Any?

    public func applicationDidFinishLaunching(_ notification: Notification) {
        // Run as menu bar agent (no dock icon needed)
        NSApp.setActivationPolicy(.accessory)

        // Setup popover with SwiftUI ContentView
        popover = NSPopover()
        popover.contentSize = NSSize(width: 320, height: 380)
        popover.behavior = .transient
        popover.contentViewController = NSHostingController(rootView: ContentView())

        // Setup Status Bar Item
        statusItem = NSStatusBar.system.statusItem(withLength: NSStatusItem.variableLength)
        if let button = statusItem.button {
            if #available(macOS 11.0, *) {
                button.image = NSImage(systemSymbolName: "sun.max.fill", accessibilityDescription: "FlickerFree Dimmer")
            } else {
                button.title = "☼"
            }
            button.action = #selector(togglePopover)
            button.target = self
        }

        // System Sleep & Wake notifications
        NSWorkspace.shared.notificationCenter.addObserver(
            self,
            selector: #selector(systemWillSleep),
            name: NSWorkspace.willSleepNotification,
            object: nil
        )
        NSWorkspace.shared.notificationCenter.addObserver(
            self,
            selector: #selector(systemDidWake),
            name: NSWorkspace.didWakeNotification,
            object: nil
        )

        // Setup global hotkeys
        registerCarbonHotkeys()
    }

    @objc private func togglePopover() {
        guard let button = statusItem.button else { return }

        if popover.isShown {
            popover.performClose(nil)
        } else {
            popover.show(relativeTo: button.bounds, of: button, preferredEdge: .minY)
            popover.contentViewController?.view.window?.makeKey()
        }
    }

    @objc private func systemWillSleep() {
        DimmerEngine.shared.restoreGamma()
    }

    @objc private func systemDidWake() {
        DispatchQueue.main.asyncAfter(deadline: .now() + 1.0) {
            AppState.shared.applySettings()
        }
    }

    public func applicationWillTerminate(_ notification: Notification) {
        DimmerEngine.shared.cleanup()
    }

    // MARK: - Carbon Global Hotkeys
    private func registerCarbonHotkeys() {
        var gMyHotKeyRef1: EventHotKeyRef?
        var gMyHotKeyRef2: EventHotKeyRef?

        var hotKeyID1 = EventHotKeyID(signature: OSType(0x464C494B), id: 1) // FLIK 1 (Dim down)
        var hotKeyID2 = EventHotKeyID(signature: OSType(0x464C494B), id: 2) // FLIK 2 (Dim up)

        // Option (2048 / 0x0800) + Command (256 / 0x0100)
        let modifiers = UInt32(cmdKey | optionKey)

        // Key code 125 = Down Arrow, Key code 126 = Up Arrow
        RegisterEventHotKey(125, modifiers, hotKeyID1, GetApplicationEventTarget(), 0, &gMyHotKeyRef1)
        RegisterEventHotKey(126, modifiers, hotKeyID2, GetApplicationEventTarget(), 0, &gMyHotKeyRef2)

        var eventType = EventTypeSpec(eventClass: OSType(kEventClassKeyboard), eventKind: UInt32(kEventHotKeyPressed))

        InstallEventHandler(GetApplicationEventTarget(), { (_, inEvent, _) -> OSStatus in
            var hotKeyID = EventHotKeyID()
            let status = GetEventParameter(
                inEvent,
                EventParamName(kEventParamDirectObject),
                EventParamType(typeEventHotKeyID),
                nil,
                MemoryLayout<EventHotKeyID>.size,
                nil,
                &hotKeyID
            )

            if status == noErr {
                DispatchQueue.main.async {
                    if hotKeyID.id == 1 {
                        AppState.shared.increaseDimming()
                    } else if hotKeyID.id == 2 {
                        AppState.shared.decreaseDimming()
                    }
                }
            }
            return noErr
        }, 1, &eventType, nil, nil)
    }
}
