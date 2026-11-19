using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.Win32;
using MediaColor = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;

namespace AntiFlickerDimmer;

public class ScreenOverlayManager : IDisposable
{
    private readonly List<OverlayWindow> _overlays = new();
    private readonly DwmDimmerEngine _dwmEngine;
    private readonly DispatcherTimer _topmostTimer;
    private int _currentDimLevel = 0;
    private bool _isEnabled = true;
    private MediaColor _currentTintColor = Colors.Black;
    private string _currentTintMode = "NeutralBlack";
    private string _dimmerMethod = "Dwm";
    private string _screenTarget = "All";

    public ScreenOverlayManager()
    {
        _dwmEngine = new DwmDimmerEngine();

        InitializeOverlays();

        SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;

        _topmostTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2)
        };
        _topmostTimer.Tick += (s, e) =>
        {
            if (_dimmerMethod == "Overlay" || _screenTarget != "All")
            {
                foreach (var overlay in _overlays)
                {
                    overlay.EnsureTopmost();
                }
            }
        };
        _topmostTimer.Start();
    }

    private void OnDisplaySettingsChanged(object? sender, EventArgs e)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            RebuildOverlays();
        });
    }

    public void InitializeOverlays()
    {
        _overlays.Clear();

        foreach (var screen in Screen.AllScreens)
        {
            var overlay = new OverlayWindow(screen);
            overlay.Show();
            _overlays.Add(overlay);
        }

        ApplyCurrentState();
    }

    public void RebuildOverlays()
    {
        foreach (var overlay in _overlays)
        {
            overlay.Close();
        }
        InitializeOverlays();
    }

    public void UpdateDimming(int level, bool isEnabled, MediaColor tintColor, string tintMode, string dimmerMethod, string screenTarget = "All")
    {
        _currentDimLevel = level;
        _isEnabled = isEnabled;
        _currentTintColor = tintColor;
        _currentTintMode = tintMode;
        _dimmerMethod = dimmerMethod;
        _screenTarget = string.IsNullOrWhiteSpace(screenTarget) ? "All" : screenTarget;

        ApplyCurrentState();
    }

    private void ApplyCurrentState()
    {
        double effectiveOpacity = _isEnabled ? (_currentDimLevel / 100.0) : 0.0;

        if (_screenTarget == "All" && _dimmerMethod == "Dwm" && _dwmEngine.IsSupported)
        {
            // DWM Engine covers EVERYTHING across all screens
            _dwmEngine.UpdateDimming(_currentDimLevel, _isEnabled, _currentTintMode);

            // Hide overlay windows so they don't double-dim
            foreach (var overlay in _overlays)
            {
                overlay.SetDimming(0.0, Colors.Black);
            }
        }
        else
        {
            // Fallback to overlay window mode (or when targeting specific screen / primary only)
            _dwmEngine.ResetToNormal();

            foreach (var overlay in _overlays)
            {
                bool shouldDim = false;
                if (_isEnabled)
                {
                    if (_screenTarget == "All")
                    {
                        shouldDim = true;
                    }
                    else if (_screenTarget == "Primary")
                    {
                        shouldDim = overlay.TargetScreen.Primary;
                    }
                    else
                    {
                        // Specific device name e.g. "\\.\DISPLAY1"
                        shouldDim = string.Equals(overlay.TargetScreen.DeviceName, _screenTarget, StringComparison.OrdinalIgnoreCase);
                    }
                }

                overlay.SetDimming(shouldDim ? effectiveOpacity : 0.0, _currentTintColor);
                overlay.ApplyScreenBounds();
            }
        }
    }

    public void Dispose()
    {
        SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
        _topmostTimer.Stop();

        _dwmEngine.Dispose();

        foreach (var overlay in _overlays)
        {
            overlay.Close();
        }
        _overlays.Clear();
    }
}
