import SwiftUI

public class AppState: ObservableObject {
    public static let shared = AppState()

    @Published public var dimLevel: Double = 0 {
        didSet { applySettings() }
    }
    @Published public var isEnabled: Bool = true {
        didSet { applySettings() }
    }
    @Published public var currentTint: String = "NeutralBlack" {
        didSet { applySettings() }
    }
    @Published public var useCoreGraphics: Bool = true {
        didSet { applySettings() }
    }

    private init() {}

    public func applySettings() {
        DimmerEngine.shared.update(
            level: Int(dimLevel),
            enabled: isEnabled,
            tint: currentTint,
            useCoreGraphics: useCoreGraphics
        )
    }

    public func increaseDimming() {
        dimLevel = min(90, dimLevel + 5)
    }

    public func decreaseDimming() {
        dimLevel = max(0, dimLevel - 5)
    }
}

public struct ContentView: View {
    @ObservedObject var state = AppState.shared

    let tints: [(id: String, name: String, color: Color)] = [
        ("NeutralBlack", "Neutral", Color(white: 0.1)),
        ("WarmAmber", "Amber", Color(red: 0.85, green: 0.55, blue: 0.15)),
        ("Candle", "Candle", Color(red: 0.95, green: 0.45, blue: 0.10)),
        ("SoftGreen", "Green", Color(red: 0.25, green: 0.70, blue: 0.40))
    ]

    public init() {}

    public var body: some View {
        VStack(spacing: 16) {
            // Header
            HStack {
                Image(systemName: "sun.max.trianglebadge.exclamationmark")
                    .font(.system(size: 20, weight: .bold))
                    .foregroundColor(.cyan)

                VStack(alignment: .leading, spacing: 2) {
                    Text("FlickerFree Dimmer")
                        .font(.system(size: 14, weight: .bold))
                        .foregroundColor(.white)
                    Text("Zero PWM Flicker • macOS")
                        .font(.system(size: 10))
                        .foregroundColor(.gray)
                }

                Spacer()

                Toggle("", isOn: $state.isEnabled)
                    .toggleStyle(SwitchToggleStyle(tint: .cyan))
                    .labelsHidden()
            }
            .padding(.horizontal, 4)

            Divider().background(Color.gray.opacity(0.3))

            // Brightness readout & Slider
            VStack(spacing: 6) {
                HStack {
                    Text("Software Dimming")
                        .font(.system(size: 12, weight: .medium))
                        .foregroundColor(.white)
                    Spacer()
                    Text("\(Int(state.dimLevel))%")
                        .font(.system(size: 14, weight: .bold, design: .monospaced))
                        .foregroundColor(.cyan)
                }

                Slider(value: $state.dimLevel, in: 0...90, step: 1)
                    .accentColor(.cyan)
                    .disabled(!state.isEnabled)
            }

            // Quick Presets
            HStack(spacing: 8) {
                PresetButton(title: "0% Off", target: 0, current: $state.dimLevel)
                PresetButton(title: "25%", target: 25, current: $state.dimLevel)
                PresetButton(title: "50%", target: 50, current: $state.dimLevel)
                PresetButton(title: "75%", target: 75, current: $state.dimLevel)
            }
            .disabled(!state.isEnabled)

            // Tint Picker
            VStack(alignment: .leading, spacing: 6) {
                Text("Night Light & Tint")
                    .font(.system(size: 11, weight: .semibold))
                    .foregroundColor(.gray)

                HStack(spacing: 8) {
                    ForEach(tints, id: \.id) { tint in
                        Button(action: { state.currentTint = tint.id }) {
                            HStack(spacing: 4) {
                                Circle()
                                    .fill(tint.color)
                                    .frame(width: 10, height: 10)
                                Text(tint.name)
                                    .font(.system(size: 11))
                            }
                            .padding(.vertical, 5)
                            .padding(.horizontal, 8)
                            .background(state.currentTint == tint.id ? Color.cyan.opacity(0.2) : Color.white.opacity(0.06))
                            .overlay(
                                RoundedRectangle(cornerRadius: 6)
                                    .stroke(state.currentTint == tint.id ? Color.cyan : Color.clear, lineWidth: 1)
                            )
                            .cornerRadius(6)
                        }
                        .buttonStyle(PlainButtonStyle())
                    }
                }
            }
            .disabled(!state.isEnabled)

            Divider().background(Color.gray.opacity(0.3))

            // Mode Selector
            VStack(alignment: .leading, spacing: 4) {
                HStack {
                    Text("Engine Mode:")
                        .font(.system(size: 11, weight: .semibold))
                        .foregroundColor(.gray)
                    Spacer()
                    Picker("", selection: $state.useCoreGraphics) {
                        Text("CoreGraphics (Full Screen)").tag(true)
                        Text("Window Overlay").tag(false)
                    }
                    .pickerStyle(SegmentedPickerStyle())
                    .frame(width: 200)
                }

                Text(state.useCoreGraphics ? "Dims entire display including Dock and Menu Bar without backlight flicker." : "Topmost transparent click-through window layer.")
                    .font(.system(size: 9))
                    .foregroundColor(.gray)
                    .lineLimit(2)
            }

            Divider().background(Color.gray.opacity(0.3))

            // Footer & Hotkey note
            HStack {
                Text("Hotkeys: ⌥⌘↓ / ⌥⌘↑")
                    .font(.system(size: 10))
                    .foregroundColor(.gray)

                Spacer()

                Button("Quit") {
                    DimmerEngine.shared.cleanup()
                    NSApplication.shared.terminate(nil)
                }
                .font(.system(size: 11))
                .foregroundColor(.red.opacity(0.8))
                .buttonStyle(PlainButtonStyle())
            }
        }
        .padding(16)
        .frame(width: 320)
        .background(Color(red: 0.08, green: 0.10, blue: 0.14))
    }
}

struct PresetButton: View {
    let title: String
    let target: Double
    @Binding var current: Double

    var body: some View {
        Button(action: { current = target }) {
            Text(title)
                .font(.system(size: 11, weight: current == target ? .bold : .normal))
                .frame(maxWidth: .infinity)
                .padding(.vertical, 6)
                .background(current == target ? Color.cyan.opacity(0.25) : Color.white.opacity(0.07))
                .foregroundColor(current == target ? .cyan : .white)
                .overlay(
                    RoundedRectangle(cornerRadius: 6)
                        .stroke(current == target ? Color.cyan : Color.clear, lineWidth: 1)
                )
                .cornerRadius(6)
        }
        .buttonStyle(PlainButtonStyle())
    }
}
