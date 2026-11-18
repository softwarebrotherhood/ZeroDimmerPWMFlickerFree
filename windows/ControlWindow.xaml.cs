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

            _showWindowMsg = NativeMethods.RegisterWindowMessage("AntiFlickerDimmer_ShowWindow_Msg");
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
            App.Log("Received AntiFlickerDimmer_ShowWindow_Msg broadcast! Bringing window to front.");
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
        _trayManager.OnExitRequested += () => Dispatcher.Invoke(PerformExit);

        // Hotkey callbacks
        _hotkeyManager.OnDimDownRequested += () => Dispatcher.Invoke(() => AdjustDimLevel(5));
        _hotkeyManager.OnDimUpRequested += () => Dispatcher.Invoke(() => AdjustDimLevel(-5));
        _hotkeyManager.OnToggleRequested += () => Dispatcher.Invoke(TogglePower);
        _hotkeyManager.OnResetRequested += () => Dispatcher.Invoke(() => SetDimLevel(0));

        // System theme changed callback
        Microsoft.Win32.SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
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
            _settings.DimmerMethod);
        _trayManager.UpdateStatus(_settings.IsEnabled, _settings.DimLevel, _settings.StartWithWindows);
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
        _settings.DimLevel = level;
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
            _osdWindow.ShowFeedback(_settings.DimLevel, _settings.IsEnabled);
        }
    }

    private void AdjustDimLevel(int delta)
    {
        SetDimLevel(_settings.DimLevel + delta);
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
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    private void OnMinimizeClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void OnCloseToTrayClick(object sender, RoutedEventArgs e)
    {
        Hide();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        // Don't kill application when X is clicked: hide to tray so screen stays dimmed!
        e.Cancel = true;
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
        _settings.DimLevel = newLevel;
        DimLevelText.Text = $"{newLevel}%";

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
                "Success! Administrator (UAC) prompts will now appear directly on your desktop and be completely dimmed by FlickerFree Dimmer.",
                "FlickerFree Dimmer",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show(
                "Unable to apply setting:\n" + ex.Message + "\n\nYou can also run 'Fix-UAC-Dimming.bat' as administrator.",
                "FlickerFree Dimmer",
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

    public void PerformExit()
    {
        Microsoft.Win32.SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
        // Clean exit
        _overlayManager.Dispose();
        _trayManager.Dispose();
        _hotkeyManager.Dispose();
        _osdWindow.Close();
        System.Windows.Application.Current.Shutdown();
    }
}
