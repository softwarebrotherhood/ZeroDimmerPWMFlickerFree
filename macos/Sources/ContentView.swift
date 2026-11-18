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
    @State private var selectedTab: Int = 0

    let tints: [(id: String, name: String, color: Color)] = [
        ("NeutralBlack", "Neutral", Color(white: 0.15)),
        ("WarmAmber", "Amber", Color(red: 0.85, green: 0.55, blue: 0.15)),
        ("Candle", "Candle", Color(red: 0.95, green: 0.45, blue: 0.10)),
        ("SoftGreen", "Green", Color(red: 0.25, green: 0.70, blue: 0.40))
    ]

    public init() {}

    public var body: some View {
        VStack(spacing: 12) {
            // Native macOS Header with Master Toggle
            HStack(spacing: 8) {
                Image(systemName: "sun.max.fill")
                    .font(.system(size: 16, weight: .bold))
                    .foregroundColor(.cyan)

                VStack(alignment: .leading, spacing: 1) {
                    Text("FlickerFree Dimmer")
                        .font(.system(size: 13, weight: .bold))
                    Text("Zero PWM Flicker • macOS")
                        .font(.system(size: 9))
                        .foregroundColor(.secondary)
                }

                Spacer()

                Toggle("", isOn: $state.isEnabled)
                    .toggleStyle(SwitchToggleStyle(tint: .cyan))
                    .labelsHidden()
            }
            .padding(.horizontal, 2)

            // Native macOS Segmented Tab Bar on TOP
            Picker("", selection: $selectedTab) {
                Text("🎛️ Dimmer").tag(0)
                Text("⚙️ Engine").tag(1)
                Text("💡 Guide").tag(2)
            }
            .pickerStyle(SegmentedPickerStyle())

            Divider()

            // Tab Views
            if selectedTab == 0 {
                // ==========================================
                // TAB 0: DIMMER CONTROLS
                // ==========================================
                VStack(spacing: 12) {
                    // Brightness Readout & Slider
                    VStack(spacing: 4) {
                        HStack {
                            Text("Software Dimming")
                                .font(.system(size: 11, weight: .medium))
                            Spacer()
                            Text("\(Int(state.dimLevel))%")
                                .font(.system(size: 14, weight: .bold, design: .monospaced))
                                .foregroundColor(state.isEnabled ? .cyan : .secondary)
                        }

                        Slider(value: $state.dimLevel, in: 0...90, step: 1)
                            .accentColor(.cyan)
                            .disabled(!state.isEnabled)
                    }

                    // Quick Presets
                    HStack(spacing: 6) {
                        PresetButton(title: "0% Off", target: 0, current: $state.dimLevel)
                        PresetButton(title: "25%", target: 25, current: $state.dimLevel)
                        PresetButton(title: "50%", target: 50, current: $state.dimLevel)
                        PresetButton(title: "75%", target: 75, current: $state.dimLevel)
                    }
                    .disabled(!state.isEnabled)

                    // Eye Care Tints
                    VStack(alignment: .leading, spacing: 6) {
                        Text("Eye Care & Night Tint")
                            .font(.system(size: 10, weight: .semibold))
                            .foregroundColor(.secondary)

                        HStack(spacing: 6) {
                            ForEach(tints, id: \.id) { tint in
                                Button(action: { state.currentTint = tint.id }) {
                                    HStack(spacing: 4) {
                                        Circle()
                                            .fill(tint.color)
                                            .frame(width: 8, height: 8)
                                        Text(tint.name)
                                            .font(.system(size: 10))
                                    }
                                    .padding(.vertical, 4)
                                    .padding(.horizontal, 6)
                                    .background(state.currentTint == tint.id ? Color.cyan.opacity(0.2) : Color.primary.opacity(0.06))
                                    .overlay(
                                        RoundedRectangle(cornerRadius: 5)
                                            .stroke(state.currentTint == tint.id ? Color.cyan : Color.clear, lineWidth: 1)
                                    )
                                    .cornerRadius(5)
                                }
                                .buttonStyle(PlainButtonStyle())
                            }
                        }
                    }
                    .disabled(!state.isEnabled)
                }
            } else if selectedTab == 1 {
                // ==========================================
                // TAB 1: ENGINE & DISPLAY
                // ==========================================
                VStack(alignment: .leading, spacing: 10) {
                    Text("Display Dimming Method")
                        .font(.system(size: 11, weight: .semibold))

                    Picker("", selection: $state.useCoreGraphics) {
                        Text("CoreGraphics (Full Screen)").tag(true)
                        Text("Window Overlay").tag(false)
                    }
                    .pickerStyle(SegmentedPickerStyle())

                    Text(state.useCoreGraphics ? "Dims entire display including Dock and Menu Bar without altering backlight." : "Topmost transparent click-through window overlay.")
                        .font(.system(size: 10))
                        .foregroundColor(.secondary)
                        .lineLimit(3)

                    Divider()

                    Text("Features:")
                        .font(.system(size: 10, weight: .semibold))
                        .foregroundColor(.secondary)

                    VStack(alignment: .leading, spacing: 4) {
                        Text("• Zero CPU usage during CoreGraphics dimming")
                        Text("• Automatic sleep & wake restoration")
                        Text("• Multi-monitor support across all connected displays")
                    }
                    .font(.system(size: 9))
                    .foregroundColor(.secondary)
                }
            } else {
                // ==========================================
                // TAB 2: GUIDE & SUPPORT
                // ==========================================
                VStack(alignment: .leading, spacing: 10) {
                    // PWM Tip
                    VStack(alignment: .leading, spacing: 3) {
                        Text("💡 Eliminating PWM Eye Strain")
                            .font(.system(size: 11, weight: .bold))
                            .foregroundColor(.cyan)
                        Text("Keep physical brightness keys at 100%. At 100%, backlight pulsing stops. Then, use this app to dim pixels safely!")
                            .font(.system(size: 10))
                            .foregroundColor(.secondary)
                    }
                    .padding(8)
                    .background(Color.cyan.opacity(0.08))
                    .cornerRadius(6)

                    // Hotkeys
                    HStack {
                        Text("Global Hotkeys:")
                            .font(.system(size: 10, weight: .semibold))
                        Spacer()
                        Text("⌥⌘↓ (Dim) / ⌥⌘↑ (Brighten)")
                            .font(.system(size: 10, design: .monospaced))
                            .foregroundColor(.cyan)
                    }

                    // Ko-fi Support Link
                    Link(destination: URL(string: "https://ko-fi.com/softwarebrotherhood")!) {
                        HStack(spacing: 6) {
                            Text("☕")
                            Text("Support on Ko-fi")
                                .font(.system(size: 11, weight: .semibold))
                        }
                        .frame(maxWidth: .infinity)
                        .padding(.vertical, 6)
                        .background(Color(red: 1.0, green: 0.37, blue: 0.36).opacity(0.18))
                        .foregroundColor(Color(red: 1.0, green: 0.50, blue: 0.50))
                        .overlay(
                            RoundedRectangle(cornerRadius: 6)
                                .stroke(Color(red: 1.0, green: 0.37, blue: 0.36).opacity(0.4), lineWidth: 1)
                        )
                        .cornerRadius(6)
                    }
                }
            }

            Divider()

            // Footer
            HStack {
                Text("Zero PWM • Open Source")
                    .font(.system(size: 9))
                    .foregroundColor(.secondary)

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
        .padding(14)
        .frame(width: 310)
    }
}

struct PresetButton: View {
    let title: String
    let target: Double
    @Binding var current: Double

    var body: some View {
        Button(action: { current = target }) {
            Text(title)
                .font(.system(size: 10, weight: current == target ? .bold : .normal))
                .frame(maxWidth: .infinity)
                .padding(.vertical, 5)
                .background(current == target ? Color.cyan.opacity(0.25) : Color.primary.opacity(0.07))
                .foregroundColor(current == target ? .cyan : .primary)
                .overlay(
                    RoundedRectangle(cornerRadius: 5)
                        .stroke(current == target ? Color.cyan : Color.clear, lineWidth: 1)
                )
                .cornerRadius(5)
        }
        .buttonStyle(PlainButtonStyle())
    }
}
