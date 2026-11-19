using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using MediaColor = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;
using WpfButton = System.Windows.Controls.Button;
using WpfRadioButton = System.Windows.Controls.RadioButton;

namespace AntiFlickerDimmer;

public partial class ControlWindow : Window
{
    private readonly AppSettings _settings;
    private readonly ScreenOverlayManager _overlayManager;
    private readonly TrayIconManager _trayManager;
    private readonly HotkeyManager _hotkeyManager;
    private readonly OsdWindow _osdWindow;
    private bool _isUpdatingUi = true; // Guard to prevent events firing during InitializeComponent

    public ControlWindow(
        AppSettings settings,
        ScreenOverlayManager overlayManager,
        TrayIconManager trayManager,
        HotkeyManager hotkeyManager,
        OsdWindow osdWindow)
    {
        _isUpdatingUi = true;
        _settings = settings;
        _overlayManager = overlayManager;
        _trayManager = trayManager;
        _hotkeyManager = hotkeyManager;
        _osdWindow = osdWindow;

        InitializeComponent();

        RestoreOrSetDefaultPosition();
        LoadSettingsIntoUi();
        ApplyTheme();
        ApplyState();
        SetupEventHandlers();

        _isUpdatingUi = false;
    }

    private uint _showWindowMsg;

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        try
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            App.Log($"ControlWindow OnSourceInitialized with HWND: {hwnd}");
            _hotkeyManager.Initialize(hwnd);

            _showWindowMsg = NativeMethods.RegisterWindowMessage("ZeroDimmer_ShowWindow_Msg");
            var source = HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);

            App.Log("HotkeyManager and ShowWindowMsg registered in OnSourceInitialized.");
        }
        catch (Exception ex)
        {
            App.Log($"Error in OnSourceInitialized hotkey init: {ex.Message}");
        }
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == _showWindowMsg && _showWindowMsg != 0)
        {
            App.Log("Received ZeroDimmer_ShowWindow_Msg broadcast! Bringing window to front.");
            ShowAndActivate();
            handled = true;
        }
        return IntPtr.Zero;
    }

    private void SetupEventHandlers()
    {
        // Tray manager callbacks
        _trayManager.OnOpenRequested += () => Dispatcher.Invoke(ShowAndActivate);
        _trayManager.OnToggleRequested += () => Dispatcher.Invoke(TogglePower);
        _trayManager.OnPresetSelected += (val) => Dispatcher.Invoke(() => SetDimLevel(val));
        _trayManager.OnStartupToggleRequested += (enabled) => Dispatcher.Invoke(() =>
        {
            _settings.StartWithWindows = enabled;
            if (StartupCheckBox != null) StartupCheckBox.IsChecked = enabled;
            _settings.Save();
        });
        _trayManager.OnScreenTargetSelected += (mode) => Dispatcher.Invoke(() =>
        {
            _settings.ScreenTargetMode = mode;
            UpdateScreenModeUi();
            ApplyState();
            _settings.Save();
        });
        _trayManager.OnExitRequested += () => Dispatcher.Invoke(PerformExit);

        // Hotkey callbacks
        _hotkeyManager.OnDimDownRequested += () => Dispatcher.Invoke(() => AdjustDimLevel(5));
        _hotkeyManager.OnDimUpRequested += () => Dispatcher.Invoke(() => AdjustDimLevel(-5));
        _hotkeyManager.OnToggleRequested += () => Dispatcher.Invoke(TogglePower);
        _hotkeyManager.OnResetRequested += () => Dispatcher.Invoke(() => SetDimLevel(0));

        // System theme changed callback
        Microsoft.Win32.SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        Microsoft.Win32.SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
    }

    private void OnUserPreferenceChanged(object? sender, Microsoft.Win32.UserPreferenceChangedEventArgs e)
    {
        if (_settings?.ThemeMode == "System")
        {
            Dispatcher.Invoke(ApplyTheme);
        }
    }

    private void LoadSettingsIntoUi()
    {
        _isUpdatingUi = true;

        if (DimSlider != null) DimSlider.Value = _settings.DimLevel;
        if (DimLevelText != null) DimLevelText.Text = $"{_settings.DimLevel}%";
        if (StartupCheckBox != null) StartupCheckBox.IsChecked = _settings.StartWithWindows;
        if (ShowOsdCheckBox != null) ShowOsdCheckBox.IsChecked = _settings.ShowOsd;
        if (AllowMoveCheckBox != null) AllowMoveCheckBox.IsChecked = _settings.AllowWindowMove;
        ApplyWindowMoveSetting();

        switch (_settings.TintMode)
        {
            case "WarmAmber":
                if (TintAmberRadio != null) TintAmberRadio.IsChecked = true;
                break;
            case "Candle":
                if (TintCandleRadio != null) TintCandleRadio.IsChecked = true;
                break;
            case "SoftGreen":
                if (TintGreenRadio != null) TintGreenRadio.IsChecked = true;
                break;
            default:
                if (TintBlackRadio != null) TintBlackRadio.IsChecked = true;
                break;
        }

        if (_settings.DimmerMethod == "Overlay")
        {
            if (ModeOverlayRadio != null) ModeOverlayRadio.IsChecked = true;
        }
        else
        {
            if (ModeDwmRadio != null) ModeDwmRadio.IsChecked = true;
        }

        switch (_settings.ThemeMode)
        {
            case "Dark":
                if (ThemeDarkRadio != null) ThemeDarkRadio.IsChecked = true;
                break;
            case "Light":
                if (ThemeLightRadio != null) ThemeLightRadio.IsChecked = true;
                break;
            default:
                if (ThemeSystemRadio != null) ThemeSystemRadio.IsChecked = true;
                break;
        }

        UpdateScreenControls();

        UpdatePowerUi();

        _isUpdatingUi = false;
    }

    private MediaColor GetCurrentColor()
    {
        return _settings.TintMode switch
        {
            "WarmAmber" => MediaColor.FromRgb(48, 25, 4),
            "Candle" => MediaColor.FromRgb(58, 28, 8),
            "SoftGreen" => MediaColor.FromRgb(8, 38, 18),
            _ => Colors.Black,
        };
    }

    private void ApplyState()
    {
        var tintColor = GetCurrentColor();
        _overlayManager.UpdateDimming(
            _settings.DimLevel,
            _settings.IsEnabled,
            tintColor,
            _settings.TintMode,
            _settings.DimmerMethod,
            _settings.ScreenTargetMode,
            _settings.ScreenDimLevels);
        _trayManager.UpdateStatus(_settings.IsEnabled, _settings.DimLevel, _settings.StartWithWindows, _settings.ScreenTargetMode);
    }

    private void UpdatePowerUi()
    {
        if (StatusBadge == null || StatusBadgeText == null || PowerToggleButton == null || PowerButtonText == null || DimLevelText == null)
            return;

        if (_settings.IsEnabled)
        {
            StatusBadge.Background = new SolidColorBrush(MediaColor.FromRgb(20, 83, 45));
            StatusBadgeText.Text = "ACTIVE";
            StatusBadgeText.Foreground = new SolidColorBrush(MediaColor.FromRgb(74, 222, 128));
            PowerToggleButton.Background = new SolidColorBrush(MediaColor.FromRgb(185, 28, 28));
            PowerButtonText.Text = "Turn Off";
            if (Resources["BrushAccent"] is SolidColorBrush accentBrush)
                DimLevelText.Foreground = accentBrush;
            else
                DimLevelText.Foreground = new SolidColorBrush(MediaColor.FromRgb(56, 189, 248));
        }
        else
        {
            StatusBadge.Background = new SolidColorBrush(MediaColor.FromRgb(127, 29, 29));
            StatusBadgeText.Text = "PAUSED";
            StatusBadgeText.Foreground = new SolidColorBrush(MediaColor.FromRgb(248, 113, 113));
            PowerToggleButton.Background = new SolidColorBrush(MediaColor.FromRgb(2, 132, 199));
            PowerButtonText.Text = "Turn On";
            if (Resources["BrushTextSecondary"] is SolidColorBrush secBrush)
                DimLevelText.Foreground = secBrush;
            else
                DimLevelText.Foreground = new SolidColorBrush(MediaColor.FromRgb(148, 163, 184));
        }
    }

    private void SetDimLevel(int level)
    {
        level = Math.Clamp(level, 0, _settings.MaxDimLevel);

        if (_settings.ScreenTargetMode == "Separate" && !string.IsNullOrEmpty(_activeScreenDevice))
        {
            _settings.SetScreenDimLevel(_activeScreenDevice, level);
            UpdateChipAndRow(_activeScreenDevice, level);
        }
        else
        {
            _settings.DimLevel = level;
        }

        if (!_settings.IsEnabled && level > 0)
        {
            _settings.IsEnabled = true;
        }

        _isUpdatingUi = true;
        if (DimSlider != null) DimSlider.Value = level;
        if (DimLevelText != null) DimLevelText.Text = $"{level}%";
        UpdatePowerUi();
        _isUpdatingUi = false;

        ApplyState();
        _settings.Save();

        if (_settings.ShowOsd)
        {
            _osdWindow.ShowFeedback(level, _settings.IsEnabled);
        }
    }

    private void AdjustDimLevel(int delta)
    {
        int current = _settings.ScreenTargetMode == "Separate" && !string.IsNullOrEmpty(_activeScreenDevice)
            ? _settings.GetScreenDimLevel(_activeScreenDevice)
            : _settings.DimLevel;
        SetDimLevel(current + delta);
    }

    private void TogglePower()
    {
        _settings.IsEnabled = !_settings.IsEnabled;
        UpdatePowerUi();
        ApplyState();
        _settings.Save();

        if (_settings.ShowOsd)
        {
            _osdWindow.ShowFeedback(_settings.DimLevel, _settings.IsEnabled);
        }
    }

    private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left && _settings.AllowWindowMove)
        {
            DragMove();
            _settings.WindowLeft = Left;
            _settings.WindowTop = Top;
            _settings.Save();
        }
    }

    private void OnMinimizeClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void OnCloseToTrayClick(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Normal)
        {
            _settings.WindowLeft = Left;
            _settings.WindowTop = Top;
            _settings.Save();
        }
        Hide();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        // Don't kill application when X is clicked: hide to tray so screen stays dimmed!
        e.Cancel = true;
        if (WindowState == WindowState.Normal)
        {
            _settings.WindowLeft = Left;
            _settings.WindowTop = Top;
            _settings.Save();
        }
        Hide();
    }

    public void ShowAndActivate()
    {
        App.Log("ControlWindow: ShowAndActivate enter");
        Show();
        if (WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
        }
        Activate();
        Topmost = true;
        Topmost = false;
        Focus();
        App.Log("ControlWindow: ShowAndActivate exit");
    }

    private void OnPowerToggleClick(object sender, RoutedEventArgs e)
    {
        TogglePower();
    }

    private void OnDimSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_isUpdatingUi || _settings == null || DimLevelText == null) return;

        int newLevel = (int)Math.Round(e.NewValue);

        if (_settings.ScreenTargetMode == "Separate" && !string.IsNullOrEmpty(_activeScreenDevice))
        {
            _settings.SetScreenDimLevel(_activeScreenDevice, newLevel);
            DimLevelText.Text = $"{newLevel}%";
            UpdateChipAndRow(_activeScreenDevice, newLevel);
        }
        else
        {
            _settings.DimLevel = newLevel;
            DimLevelText.Text = $"{newLevel}%";
        }

        ApplyState();
        _settings.Save();
    }

    private void OnIncrementDimClick(object sender, RoutedEventArgs e)
    {
        AdjustDimLevel(1);
    }

    private void OnDecrementDimClick(object sender, RoutedEventArgs e)
    {
        AdjustDimLevel(-1);
    }

    private void OnPresetClick(object sender, RoutedEventArgs e)
    {
        if (sender is WpfButton btn && btn.Tag is string tagStr && int.TryParse(tagStr, out int val))
        {
            SetDimLevel(val);
        }
    }

    private void OnTintRadioChanged(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingUi || _settings == null) return;

        if (sender is WpfRadioButton rb && rb.IsChecked == true && rb.Tag is string mode)
        {
            _settings.TintMode = mode;
            ApplyState();
            _settings.Save();
        }
    }

    private void OnDimmerMethodChanged(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingUi || _settings == null) return;

        if (sender is WpfRadioButton rb && rb.IsChecked == true && rb.Tag is string method)
        {
            _settings.DimmerMethod = method;
            ApplyState();
            _settings.Save();
        }
    }

    private void OnThemeRadioChanged(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingUi || _settings == null) return;

        if (sender is WpfRadioButton rb && rb.IsChecked == true && rb.Tag is string mode)
        {
            _settings.ThemeMode = mode;
            ApplyTheme();
            _settings.Save();
        }
    }

    private void OnStartupCheckChanged(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingUi || _settings == null || _trayManager == null || StartupCheckBox == null) return;

        _settings.StartWithWindows = StartupCheckBox.IsChecked == true;
        _settings.Save();
        _trayManager.UpdateStatus(_settings.IsEnabled, _settings.DimLevel, _settings.StartWithWindows);
    }

    private void OnShowOsdCheckChanged(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingUi || _settings == null || ShowOsdCheckBox == null) return;

        _settings.ShowOsd = ShowOsdCheckBox.IsChecked == true;
        _settings.Save();
    }

    private void OnAllowMoveCheckChanged(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingUi || _settings == null || AllowMoveCheckBox == null) return;

        _settings.AllowWindowMove = AllowMoveCheckBox.IsChecked == true;
        ApplyWindowMoveSetting();
        _settings.Save();
    }

    private void OnResetPositionClick(object sender, RoutedEventArgs e)
    {
        PositionWindowBottomRight();
        _settings.WindowLeft = Left;
        _settings.WindowTop = Top;
        _settings.Save();
    }

    private void OnFixUacClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "reg.exe",
                Arguments = "add \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\" /v PromptOnSecureDesktop /t REG_DWORD /d 0 /f",
                Verb = "runas",
                UseShellExecute = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            };
            var p = System.Diagnostics.Process.Start(psi);
            p?.WaitForExit();

            System.Windows.Forms.MessageBox.Show(
                "Success! Administrator (UAC) prompts will now appear directly on your desktop and be completely dimmed by ZeroDimmer.",
                "ZeroDimmer",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show(
                "Unable to apply setting:\n" + ex.Message + "\n\nYou can also run 'Fix-UAC-Dimming.bat' as administrator.",
                "ZeroDimmer",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Warning);
        }
    }

    private void OnSupportKofiClick(object sender, RoutedEventArgs e)
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://ko-fi.com/softwarebrotherhood",
                UseShellExecute = true
            });
        }
        catch { }
    }

    private void OnQuitClick(object sender, RoutedEventArgs e)
    {
        PerformExit();
    }

    private void ApplyTheme()
    {
        bool isLight;
        if (_settings.ThemeMode == "Light")
        {
            isLight = true;
        }
        else if (_settings.ThemeMode == "Dark")
        {
            isLight = false;
        }
        else // "System"
        {
            isLight = IsWindowsInLightTheme();
        }

        if (isLight)
        {
            SetThemeBrush("BrushWindowBg", "#F1F5F9");
            SetThemeBrush("BrushWindowBorder", "#0284C7");
            SetThemeBrush("BrushCardBg", "#FFFFFF");
            SetThemeBrush("BrushCardBorder", "#CBD5E1");
            SetThemeBrush("BrushTextPrimary", "#0F172A");
            SetThemeBrush("BrushTextSecondary", "#475569");
            SetThemeBrush("BrushTextMuted", "#64748B");
            SetThemeBrush("BrushAccent", "#0284C7");
            SetThemeBrush("BrushTabContainer", "#E2E8F0");
            SetThemeBrush("BrushTabContainerBorder", "#CBD5E1");
            SetThemeBrush("BrushTabActiveBg", "#FFFFFF");
            SetThemeBrush("BrushTabInactiveFg", "#64748B");
            SetThemeBrush("BrushButtonBg", "#E2E8F0");
            SetThemeBrush("BrushButtonHover", "#CBD5E1");
            SetThemeBrush("BrushButtonFg", "#0F172A");
            SetThemeBrush("BrushChipBg", "#F8FAFC");
            SetThemeBrush("BrushChipBorder", "#CBD5E1");
            SetThemeBrush("BrushChipFg", "#334155");
            SetThemeBrush("BrushScrollThumb", "#CBD5E1");
            SetThemeBrush("BrushTipCardBg", "#EFF6FF");
            SetThemeBrush("BrushTipCardBorder", "#BFDBFE");
            SetThemeBrush("BrushTipCardTitle", "#1D4ED8");
            SetThemeBrush("BrushTipCardText", "#1E3A8A");
            SetThemeBrush("BrushKofiCardBg", "#FFFBEB");
            SetThemeBrush("BrushKofiCardBorder", "#FDE68A");
            SetThemeBrush("BrushKofiCardTitle", "#B45309");
            SetThemeBrush("BrushKofiCardText", "#78350F");
        }
        else
        {
            SetThemeBrush("BrushWindowBg", "#0F172A");
            SetThemeBrush("BrushWindowBorder", "#38BDF8");
            SetThemeBrush("BrushCardBg", "#1E293B");
            SetThemeBrush("BrushCardBorder", "#334155");
            SetThemeBrush("BrushTextPrimary", "#F8FAFC");
            SetThemeBrush("BrushTextSecondary", "#94A3B8");
            SetThemeBrush("BrushTextMuted", "#64748B");
            SetThemeBrush("BrushAccent", "#38BDF8");
            SetThemeBrush("BrushTabContainer", "#1E293B");
            SetThemeBrush("BrushTabContainerBorder", "#334155");
            SetThemeBrush("BrushTabActiveBg", "#0F172A");
            SetThemeBrush("BrushTabInactiveFg", "#94A3B8");
            SetThemeBrush("BrushButtonBg", "#334155");
            SetThemeBrush("BrushButtonHover", "#475569");
            SetThemeBrush("BrushButtonFg", "#F8FAFC");
            SetThemeBrush("BrushChipBg", "#1E293B");
            SetThemeBrush("BrushChipBorder", "#334155");
            SetThemeBrush("BrushChipFg", "#CBD5E1");
            SetThemeBrush("BrushScrollThumb", "#334155");
            SetThemeBrush("BrushTipCardBg", "#172554");
            SetThemeBrush("BrushTipCardBorder", "#1E40AF");
            SetThemeBrush("BrushTipCardTitle", "#93C5FD");
            SetThemeBrush("BrushTipCardText", "#BFDBFE");
            SetThemeBrush("BrushKofiCardBg", "#1C1917");
            SetThemeBrush("BrushKofiCardBorder", "#78350F");
            SetThemeBrush("BrushKofiCardTitle", "#F59E0B");
            SetThemeBrush("BrushKofiCardText", "#D6D3D1");
        }

        UpdatePowerUi();
    }

    private void SetThemeBrush(string key, string hex)
    {
        var color = (MediaColor)System.Windows.Media.ColorConverter.ConvertFromString(hex);
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        Resources[key] = brush;
    }

    private static bool IsWindowsInLightTheme()
    {
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key != null)
            {
                var val = key.GetValue("AppsUseLightTheme");
                if (val is int intVal)
                {
                    return intVal == 1;
                }
            }
        }
        catch { }
        return false;
    }

    private void RestoreOrSetDefaultPosition()
    {
        WindowStartupLocation = WindowStartupLocation.Manual;

        if (_settings.WindowLeft >= 0 && _settings.WindowTop >= 0 && IsPositionOnScreen(_settings.WindowLeft, _settings.WindowTop, Width, Height))
        {
            Left = _settings.WindowLeft;
            Top = _settings.WindowTop;
        }
        else
        {
            PositionWindowBottomRight();
        }
    }

    private void PositionWindowBottomRight()
    {
        var workArea = SystemParameters.WorkArea;
        double w = double.IsNaN(Width) || Width <= 0 ? 410 : Width;
        double h = double.IsNaN(Height) || Height <= 0 ? 495 : Height;
        Left = Math.Max(workArea.Left, workArea.Right - w - 16);
        Top = Math.Max(workArea.Top, workArea.Bottom - h - 16);
    }

    private static bool IsPositionOnScreen(double left, double top, double width, double height)
    {
        try
        {
            var windowRect = new System.Drawing.Rectangle((int)left, (int)top, (int)width, (int)height);
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(windowRect))
                {
                    return true;
                }
            }
        }
        catch { }
        return false;
    }

    private void ApplyWindowMoveSetting()
    {
        if (TitleBarGrid != null)
        {
            TitleBarGrid.Cursor = _settings.AllowWindowMove ? System.Windows.Input.Cursors.SizeAll : System.Windows.Input.Cursors.Arrow;
        }
    }

    private string _activeScreenDevice = "";
    private readonly Dictionary<string, System.Windows.Controls.Slider> _screenSliders = new();
    private readonly Dictionary<string, System.Windows.Controls.TextBlock> _screenLevelTexts = new();
    private readonly Dictionary<string, WpfButton> _screenChips = new();

    private void UpdateScreenControls()
    {
        var screens = System.Windows.Forms.Screen.AllScreens;
        int count = screens.Length;
        if (ScreenCountText != null)
        {
            ScreenCountText.Text = count == 1 ? "1 Screen Detected" : $"{count} Screens Connected";
        }

        if (string.IsNullOrEmpty(_activeScreenDevice) || !Array.Exists(screens, s => s.DeviceName == _activeScreenDevice))
        {
            var primary = Array.Find(screens, s => s.Primary) ?? (screens.Length > 0 ? screens[0] : null);
            _activeScreenDevice = primary?.DeviceName ?? "";
            _settings.ActiveScreenDevice = _activeScreenDevice;
        }

        if (_settings.ScreenTargetMode == "Separate")
        {
            if (ScreenSeparateRadio != null) ScreenSeparateRadio.IsChecked = true;
            if (PerScreenPanel != null) PerScreenPanel.Visibility = Visibility.Visible;
        }
        else
        {
            if (ScreenAllRadio != null) ScreenAllRadio.IsChecked = true;
            if (PerScreenPanel != null) PerScreenPanel.Visibility = Visibility.Collapsed;
        }

        BuildPerScreenControls();
    }

    private void BuildPerScreenControls()
    {
        if (ScreenChipsPanel == null || PerScreenListPanel == null) return;

        ScreenChipsPanel.Children.Clear();
        PerScreenListPanel.Children.Clear();
        _screenSliders.Clear();
        _screenLevelTexts.Clear();
        _screenChips.Clear();

        var screens = System.Windows.Forms.Screen.AllScreens;

        for (int i = 0; i < screens.Length; i++)
        {
            var scr = screens[i];
            string dev = scr.DeviceName;
            int level = _settings.GetScreenDimLevel(dev);
            string displayName = $"Display {i + 1}{(scr.Primary ? " (Primary)" : "")}";

            // 1. Chip button in ScreenChipsPanel
            var chip = new WpfButton
            {
                Content = $"{displayName}: {level}%",
                Tag = dev,
                Margin = new Thickness(0, 0, 6, 4),
                Padding = new Thickness(8, 4, 8, 4),
                FontSize = 10,
                FontWeight = FontWeights.SemiBold,
                Cursor = System.Windows.Input.Cursors.Hand
            };
            if (TryFindResource("PresetChip") is Style chipStyle)
            {
                chip.Style = chipStyle;
            }
            chip.Click += (s, e) =>
            {
                SetActiveScreenDevice(dev);
            };
            _screenChips[dev] = chip;
            ScreenChipsPanel.Children.Add(chip);

            // 2. Row in PerScreenListPanel
            var rowBorder = new System.Windows.Controls.Border
            {
                Background = TryFindResource("BrushCardBg") as System.Windows.Media.Brush ?? new SolidColorBrush(MediaColor.FromRgb(30, 41, 59)),
                BorderBrush = TryFindResource("BrushCardBorder") as System.Windows.Media.Brush ?? new SolidColorBrush(MediaColor.FromRgb(51, 65, 85)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(8, 6, 8, 6),
                Margin = new Thickness(0, 0, 0, 6)
            };

            var rowStack = new System.Windows.Controls.StackPanel();

            // Row header: name + percentage
            var rowHeader = new System.Windows.Controls.Grid { Margin = new Thickness(0, 0, 0, 4) };
            var nameBlock = new System.Windows.Controls.TextBlock
            {
                Text = $"🖥️ {displayName} — {scr.Bounds.Width}×{scr.Bounds.Height}",
                FontSize = 10,
                FontWeight = FontWeights.SemiBold,
                Foreground = TryFindResource("BrushTextPrimary") as System.Windows.Media.Brush ?? new SolidColorBrush(Colors.White),
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            };
            var lvlBlock = new System.Windows.Controls.TextBlock
            {
                Text = $"{level}%",
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Foreground = TryFindResource("BrushAccent") as System.Windows.Media.Brush ?? new SolidColorBrush(MediaColor.FromRgb(56, 189, 248)),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            };
            _screenLevelTexts[dev] = lvlBlock;
            rowHeader.Children.Add(nameBlock);
            rowHeader.Children.Add(lvlBlock);
            rowStack.Children.Add(rowHeader);

            // Slider & Stepper row
            var sliderGrid = new System.Windows.Controls.Grid();
            sliderGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            sliderGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition { Width = GridLength.Auto });

            var rowSlider = new System.Windows.Controls.Slider
            {
                Minimum = 0,
                Maximum = 90,
                Value = level,
                SmallChange = 1,
                LargeChange = 5,
                Height = 20,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 8, 0)
            };
            _screenSliders[dev] = rowSlider;

            rowSlider.ValueChanged += (s, e) =>
            {
                if (_isUpdatingUi) return;
                int val = (int)Math.Round(e.NewValue);
                _settings.SetScreenDimLevel(dev, val);
                lvlBlock.Text = $"{val}%";
                chip.Content = $"{displayName}: {val}%";

                if (dev == _activeScreenDevice)
                {
                    _isUpdatingUi = true;
                    if (DimSlider != null) DimSlider.Value = val;
                    if (DimLevelText != null) DimLevelText.Text = $"{val}%";
                    _isUpdatingUi = false;
                }

                ApplyState();
                _settings.Save();
            };

            System.Windows.Controls.Grid.SetColumn(rowSlider, 0);
            sliderGrid.Children.Add(rowSlider);

            // Steppers
            var stepperStack = new System.Windows.Controls.StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            };
            var minusBtn = new WpfButton
            {
                Content = "-",
                Width = 22,
                Height = 22,
                Padding = new Thickness(0),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 4, 0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            if (TryFindResource("ModernButton") is Style btnStyle)
            {
                minusBtn.Style = btnStyle;
            }
            minusBtn.Click += (s, e) =>
            {
                rowSlider.Value = Math.Max(0, rowSlider.Value - 5);
            };

            var plusBtn = new WpfButton
            {
                Content = "+",
                Width = 22,
                Height = 22,
                Padding = new Thickness(0),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Cursor = System.Windows.Input.Cursors.Hand
            };
            if (TryFindResource("ModernButton") is Style btnStyle2)
            {
                plusBtn.Style = btnStyle2;
            }
            plusBtn.Click += (s, e) =>
            {
                rowSlider.Value = Math.Min(_settings.MaxDimLevel, rowSlider.Value + 5);
            };

            stepperStack.Children.Add(minusBtn);
            stepperStack.Children.Add(plusBtn);
            System.Windows.Controls.Grid.SetColumn(stepperStack, 1);
            sliderGrid.Children.Add(stepperStack);

            rowStack.Children.Add(sliderGrid);
            rowBorder.Child = rowStack;
            PerScreenListPanel.Children.Add(rowBorder);
        }

        HighlightActiveChip();
    }

    private void SetActiveScreenDevice(string dev)
    {
        _activeScreenDevice = dev;
        _settings.ActiveScreenDevice = dev;
        HighlightActiveChip();

        int level = _settings.GetScreenDimLevel(dev);
        _isUpdatingUi = true;
        if (DimSlider != null) DimSlider.Value = level;
        if (DimLevelText != null) DimLevelText.Text = $"{level}%";
        _isUpdatingUi = false;
    }

    private void HighlightActiveChip()
    {
        foreach (var kvp in _screenChips)
        {
            bool isActive = kvp.Key == _activeScreenDevice;
            kvp.Value.BorderBrush = isActive 
                ? (TryFindResource("BrushAccent") as System.Windows.Media.Brush ?? new SolidColorBrush(MediaColor.FromRgb(56, 189, 248)))
                : (TryFindResource("BrushCardBorder") as System.Windows.Media.Brush ?? new SolidColorBrush(MediaColor.FromRgb(51, 65, 85)));
            kvp.Value.BorderThickness = new Thickness(isActive ? 2 : 1);
        }
    }

    private void UpdateChipAndRow(string dev, int level)
    {
        if (_screenSliders.TryGetValue(dev, out var sld) && Math.Abs(sld.Value - level) > 0.5)
        {
            _isUpdatingUi = true;
            sld.Value = level;
            _isUpdatingUi = false;
        }
        if (_screenLevelTexts.TryGetValue(dev, out var txt))
        {
            txt.Text = $"{level}%";
        }
        if (_screenChips.TryGetValue(dev, out var chip))
        {
            var screens = System.Windows.Forms.Screen.AllScreens;
            var scr = Array.Find(screens, s => s.DeviceName == dev);
            int idx = Array.IndexOf(screens, scr);
            string dispName = $"Display {idx + 1}{(scr != null && scr.Primary ? " (Primary)" : "")}";
            chip.Content = $"{dispName}: {level}%";
        }
    }

    private void OnScreenModeRadioChanged(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingUi || _settings == null) return;

        if (sender is WpfRadioButton rb && rb.IsChecked == true && rb.Tag is string mode)
        {
            _settings.ScreenTargetMode = mode;
            UpdateScreenModeUi();
            ApplyState();
            _settings.Save();
        }
    }

    private void UpdateScreenModeUi()
    {
        bool isSeparate = _settings.ScreenTargetMode == "Separate";
        if (PerScreenPanel != null)
        {
            PerScreenPanel.Visibility = isSeparate ? Visibility.Visible : Visibility.Collapsed;
        }

        if (isSeparate)
        {
            var screens = System.Windows.Forms.Screen.AllScreens;
            if (string.IsNullOrEmpty(_activeScreenDevice) || !Array.Exists(screens, s => s.DeviceName == _activeScreenDevice))
            {
                var primary = Array.Find(screens, s => s.Primary) ?? (screens.Length > 0 ? screens[0] : null);
                _activeScreenDevice = primary?.DeviceName ?? "";
                _settings.ActiveScreenDevice = _activeScreenDevice;
            }

            int activeLvl = _settings.GetScreenDimLevel(_activeScreenDevice);
            _isUpdatingUi = true;
            if (DimSlider != null) DimSlider.Value = activeLvl;
            if (DimLevelText != null) DimLevelText.Text = $"{activeLvl}%";
            _isUpdatingUi = false;
        }
        else
        {
            _isUpdatingUi = true;
            if (DimSlider != null) DimSlider.Value = _settings.DimLevel;
            if (DimLevelText != null) DimLevelText.Text = $"{_settings.DimLevel}%";
            _isUpdatingUi = false;
        }

        BuildPerScreenControls();
    }

    private void OnDisplaySettingsChanged(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            UpdateScreenControls();
            ApplyState();
        });
    }

    public void PerformExit()
    {
        Microsoft.Win32.SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
        Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
        if (WindowState == WindowState.Normal)
        {
            _settings.WindowLeft = Left;
            _settings.WindowTop = Top;
            _settings.Save();
        }
        // Clean exit
        _overlayManager.Dispose();
        _trayManager.Dispose();
        _hotkeyManager.Dispose();
        _osdWindow.Close();
        System.Windows.Application.Current.Shutdown();
    }
}
