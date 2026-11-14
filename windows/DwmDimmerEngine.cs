using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Microsoft.Win32;

namespace AntiFlickerDimmer;

public class DwmDimmerEngine : IDisposable
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MAGCOLOREFFECT
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 25)]
        public float[] transform;
    }

    [DllImport("magnification.dll", ExactSpelling = true, SetLastError = true)]
    private static extern bool MagInitialize();

    [DllImport("magnification.dll", ExactSpelling = true, SetLastError = true)]
    private static extern bool MagUninitialize();

    [DllImport("magnification.dll", ExactSpelling = true, SetLastError = true)]
    private static extern bool MagSetFullscreenColorEffect(ref MAGCOLOREFFECT pEffect);

    private bool _isInitialized = false;
    private int _currentDimLevel = 0;
    private bool _isEnabled = true;
    private string _currentTintMode = "NeutralBlack";
    private readonly DispatcherTimer _watchdogTimer;

    public bool IsSupported => _isInitialized;

    public DwmDimmerEngine()
    {
        try
        {
            _isInitialized = MagInitialize();
            App.Log($"DwmDimmerEngine: MagInitialize result = {_isInitialized}");
        }
        catch (Exception ex)
        {
            App.Log($"DwmDimmerEngine: MagInitialize error: {ex.Message}");
            _isInitialized = false;
        }

        SystemEvents.SessionSwitch += OnSessionSwitch;
        SystemEvents.PowerModeChanged += OnPowerModeChanged;
        SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;

        _watchdogTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2)
        };
        _watchdogTimer.Tick += (s, e) =>
        {
            if (_isInitialized && _isEnabled && _currentDimLevel > 0)
            {
                ApplyCurrentEffect();
            }
        };
        _watchdogTimer.Start();
    }

    public void UpdateDimming(int dimLevel, bool isEnabled, string tintMode)
    {
        _currentDimLevel = Math.Clamp(dimLevel, 0, 95);
        _isEnabled = isEnabled;
        _currentTintMode = tintMode;

        ApplyCurrentEffect();
    }

    private void ApplyCurrentEffect()
    {
        if (!_isInitialized) return;

        try
        {
            if (!_isEnabled || _currentDimLevel <= 0)
            {
                ResetToNormal();
                return;
            }

            // Calculate brightness factor: 0% dark = 1.0 (normal), 50% dark = 0.50, 80% dark = 0.20
            float factor = (100f - _currentDimLevel) / 100f;
            factor = Math.Clamp(factor, 0.05f, 1.0f);

            float r = factor;
            float g = factor;
            float b = factor;

            // Apply color tint adjustments to color matrix
            switch (_currentTintMode)
            {
                case "WarmAmber":
                    // Night light warmth: keep red, gently reduce green, cut blue
                    g = factor * 0.84f;
                    b = factor * 0.52f;
                    break;

                case "Candle":
                    // Ultra cozy amber-orange
                    g = factor * 0.70f;
                    b = factor * 0.38f;
                    break;

                case "SoftGreen":
                    // Soft soothing green
                    r = factor * 0.78f;
                    b = factor * 0.78f;
                    break;

                default: // NeutralBlack
                    // Pure linear dimming across all channels
                    break;
            }

            var effect = new MAGCOLOREFFECT
            {
                transform = new float[]
                {
                    r, 0, 0, 0, 0,
                    0, g, 0, 0, 0,
                    0, 0, b, 0, 0,
                    0, 0, 0, 1, 0,
                    0, 0, 0, 0, 1
                }
            };

            MagSetFullscreenColorEffect(ref effect);
        }
        catch (Exception ex)
        {
            App.Log($"DwmDimmerEngine ApplyCurrentEffect error: {ex.Message}");
        }
    }

    public void ResetToNormal()
    {
        if (!_isInitialized) return;

        try
        {
            var identity = new MAGCOLOREFFECT
            {
                transform = new float[]
                {
                    1, 0, 0, 0, 0,
                    0, 1, 0, 0, 0,
                    0, 0, 1, 0, 0,
                    0, 0, 0, 1, 0,
                    0, 0, 0, 0, 1
                }
            };
            MagSetFullscreenColorEffect(ref identity);
        }
        catch { }
    }

    private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        if (e.Reason == SessionSwitchReason.SessionUnlock)
        {
            ApplyCurrentEffect();
        }
    }

    private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        if (e.Mode == PowerModes.Resume)
        {
            ApplyCurrentEffect();
        }
    }

    private void OnDisplaySettingsChanged(object? sender, EventArgs e)
    {
        ApplyCurrentEffect();
    }

    public void Dispose()
    {
        _watchdogTimer.Stop();
        SystemEvents.SessionSwitch -= OnSessionSwitch;
        SystemEvents.PowerModeChanged -= OnPowerModeChanged;
        SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;

        ResetToNormal();

        if (_isInitialized)
        {
            try
            {
                MagUninitialize();
            }
            catch { }
            _isInitialized = false;
        }
    }
}
