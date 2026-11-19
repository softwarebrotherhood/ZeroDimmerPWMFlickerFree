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
    private string _screenTargetMode = "All";
    private Dictionary<string, int> _perScreenLevels = new();

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
            if (_dimmerMethod == "Overlay" || _screenTargetMode == "Separate")
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

    public void UpdateDimming(
        int globalLevel,
        bool isEnabled,
        MediaColor tintColor,
        string tintMode,
        string dimmerMethod,
        string screenTargetMode = "All",
        Dictionary<string, int>? perScreenLevels = null)
    {
        _currentDimLevel = globalLevel;
        _isEnabled = isEnabled;
        _currentTintColor = tintColor;
        _currentTintMode = tintMode;
        _dimmerMethod = dimmerMethod;
        _screenTargetMode = string.IsNullOrWhiteSpace(screenTargetMode) ? "All" : screenTargetMode;
        _perScreenLevels = perScreenLevels != null ? new Dictionary<string, int>(perScreenLevels) : new();

        ApplyCurrentState();
    }

    private void ApplyCurrentState()
    {
        if (!_isEnabled)
        {
            _dwmEngine.ResetToNormal();
            foreach (var overlay in _overlays)
            {
                overlay.SetDimming(0.0, Colors.Black);
            }
            return;
        }

        if (_screenTargetMode == "All" && _dimmerMethod == "Dwm" && _dwmEngine.IsSupported)
        {
            // DWM Engine covers EVERYTHING across all screens uniformly
            _dwmEngine.UpdateDimming(_currentDimLevel, _isEnabled, _currentTintMode);

            // Hide overlay windows so they don't double-dim
            foreach (var overlay in _overlays)
            {
                overlay.SetDimming(0.0, Colors.Black);
            }
        }
        else
        {
            // In Separate mode OR Overlay mode:
            // DWM cannot apply different matrices to different monitors, so reset DWM
            _dwmEngine.ResetToNormal();

            foreach (var overlay in _overlays)
            {
                int screenLevel = _currentDimLevel;
                if (_screenTargetMode == "Separate" && _perScreenLevels.TryGetValue(overlay.TargetScreen.DeviceName, out int customLevel))
                {
                    screenLevel = customLevel;
                }

                double effectiveOpacity = Math.Clamp(screenLevel / 100.0, 0.0, 0.95);
                overlay.SetDimming(effectiveOpacity, _currentTintColor);
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
