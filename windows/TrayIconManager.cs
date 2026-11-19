using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AntiFlickerDimmer;

public class TrayIconManager : IDisposable
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool DestroyIcon(IntPtr handle);

    private readonly NotifyIcon _notifyIcon;
    private Icon? _currentIcon;
    private readonly ToolStripMenuItem _activeMenuItem;
    private readonly ToolStripMenuItem _startWithWindowsItem;
    private readonly ToolStripMenuItem _screenAllItem;
    private readonly ToolStripMenuItem _screenPrimaryItem;

    public event Action? OnOpenRequested;
    public event Action? OnToggleRequested;
    public event Action<int>? OnPresetSelected;
    public event Action? OnExitRequested;
    public event Action<bool>? OnStartupToggleRequested;
    public event Action<string>? OnScreenTargetSelected;

    public TrayIconManager()
    {
        _notifyIcon = new NotifyIcon
        {
            Visible = true,
            Text = "ZeroDimmer"
        };

        var menu = new ContextMenuStrip();

        var titleItem = new ToolStripMenuItem("ZeroDimmer")
        {
            Enabled = false,
            Font = new Font(Control.DefaultFont, FontStyle.Bold)
        };
        menu.Items.Add(titleItem);
        menu.Items.Add(new ToolStripSeparator());

        _activeMenuItem = new ToolStripMenuItem("Dimmer Enabled", null, (s, e) => OnToggleRequested?.Invoke())
        {
            Checked = true
        };
        menu.Items.Add(_activeMenuItem);

        var presetsMenu = new ToolStripMenuItem("Quick Presets");
        presetsMenu.DropDownItems.Add(new ToolStripMenuItem("Off (0%)", null, (s, e) => OnPresetSelected?.Invoke(0)));
        presetsMenu.DropDownItems.Add(new ToolStripMenuItem("25% Darkness", null, (s, e) => OnPresetSelected?.Invoke(25)));
        presetsMenu.DropDownItems.Add(new ToolStripMenuItem("40% Darkness", null, (s, e) => OnPresetSelected?.Invoke(40)));
        presetsMenu.DropDownItems.Add(new ToolStripMenuItem("60% Darkness", null, (s, e) => OnPresetSelected?.Invoke(60)));
        presetsMenu.DropDownItems.Add(new ToolStripMenuItem("80% Darkness", null, (s, e) => OnPresetSelected?.Invoke(80)));
        menu.Items.Add(presetsMenu);

        var screensSubMenu = new ToolStripMenuItem("Target Displays");
        _screenAllItem = new ToolStripMenuItem("🌐 All Connected Screens", null, (s, e) => OnScreenTargetSelected?.Invoke("All"))
        {
            Checked = true
        };
        _screenPrimaryItem = new ToolStripMenuItem("💻 Primary Screen Only", null, (s, e) => OnScreenTargetSelected?.Invoke("Primary"));
        screensSubMenu.DropDownItems.Add(_screenAllItem);
        screensSubMenu.DropDownItems.Add(_screenPrimaryItem);
        menu.Items.Add(screensSubMenu);

        menu.Items.Add(new ToolStripSeparator());

        var openItem = new ToolStripMenuItem("Open Controls...", null, (s, e) => OnOpenRequested?.Invoke());
        openItem.Font = new Font(Control.DefaultFont, FontStyle.Bold);
        menu.Items.Add(openItem);

        _startWithWindowsItem = new ToolStripMenuItem("Start with Windows");
        _startWithWindowsItem.Click += (s, e) =>
        {
            _startWithWindowsItem.Checked = !_startWithWindowsItem.Checked;
            OnStartupToggleRequested?.Invoke(_startWithWindowsItem.Checked);
        };
        menu.Items.Add(_startWithWindowsItem);

        var kofiItem = new ToolStripMenuItem("Support on Ko-fi ☕", null, (s, e) =>
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
        });
        menu.Items.Add(kofiItem);

        menu.Items.Add(new ToolStripSeparator());

        var exitItem = new ToolStripMenuItem("Exit Dimmer", null, (s, e) => OnExitRequested?.Invoke());
        menu.Items.Add(exitItem);

        _notifyIcon.ContextMenuStrip = menu;

        // Left-click directly opens control window
        _notifyIcon.MouseClick += (s, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                OnOpenRequested?.Invoke();
            }
        };

        UpdateIcon(true, 35);
    }

    public void UpdateStatus(bool isEnabled, int level, bool startWithWindows, string screenTarget = "All")
    {
        _activeMenuItem.Checked = isEnabled;
        _startWithWindowsItem.Checked = startWithWindows;
        _screenAllItem.Checked = screenTarget == "All";
        _screenPrimaryItem.Checked = screenTarget == "Primary";

        string tooltip = isEnabled && level > 0
            ? $"ZeroDimmer: {level}% (Active)"
            : "ZeroDimmer: Paused / Off";

        // NotifyIcon text max length is 63 characters
        _notifyIcon.Text = tooltip.Length > 63 ? tooltip[..63] : tooltip;

        UpdateIcon(isEnabled, level);
    }

    public void ShowNotification(string title, string text)
    {
        _notifyIcon.ShowBalloonTip(2000, title, text, ToolTipIcon.Info);
    }

    private void UpdateIcon(bool isEnabled, int level)
    {
        try
        {
            using var bitmap = new Bitmap(32, 32);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(System.Drawing.Color.Transparent);

                // Background Circle / Shield
                var bgBrush = new SolidBrush(isEnabled 
                    ? System.Drawing.Color.FromArgb(30, 41, 59) 
                    : System.Drawing.Color.FromArgb(71, 85, 105));
                g.FillEllipse(bgBrush, 2, 2, 28, 28);

                // Crescent Moon or Sun Ray Accent
                if (isEnabled && level > 0)
                {
                    // Draw stylish crescent moon
                    using var moonBrush = new SolidBrush(System.Drawing.Color.FromArgb(56, 189, 248));
                    g.FillEllipse(moonBrush, 7, 7, 18, 18);

                    using var cutoutBrush = new SolidBrush(System.Drawing.Color.FromArgb(30, 41, 59));
                    g.FillEllipse(cutoutBrush, 12, 5, 15, 15);
                }
                else
                {
                    // Sun / Day symbol
                    using var sunBrush = new SolidBrush(System.Drawing.Color.FromArgb(251, 191, 36));
                    g.FillEllipse(sunBrush, 10, 10, 12, 12);
                }
            }

            var hIcon = bitmap.GetHicon();
            var newIcon = Icon.FromHandle(hIcon);

            _notifyIcon.Icon = newIcon;

            if (_currentIcon != null)
            {
                DestroyIcon(_currentIcon.Handle);
                _currentIcon.Dispose();
            }

            _currentIcon = newIcon;
        }
        catch
        {
            // Fallback to system application icon if dynamic drawing fails
            _notifyIcon.Icon = SystemIcons.Application;
        }
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();

        if (_currentIcon != null)
        {
            DestroyIcon(_currentIcon.Handle);
            _currentIcon.Dispose();
        }
    }
}
