using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Forms;
using MediaColor = System.Windows.Media.Color;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace AntiFlickerDimmer;

public partial class OverlayWindow : Window
{
    public Screen TargetScreen { get; }
    private IntPtr _hwnd = IntPtr.Zero;

    public OverlayWindow(Screen screen)
    {
        InitializeComponent();
        TargetScreen = screen;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        _hwnd = new WindowInteropHelper(this).Handle;

        // Apply extended styles: Click-through (TRANSPARENT), ToolWindow (hide from Alt+Tab), Layered, NoActivate
        int exStyle = NativeMethods.GetWindowLong(_hwnd, NativeMethods.GWL_EXSTYLE);
        NativeMethods.SetWindowLong(_hwnd, NativeMethods.GWL_EXSTYLE,
            exStyle |
            NativeMethods.WS_EX_TRANSPARENT |
            NativeMethods.WS_EX_LAYERED |
            NativeMethods.WS_EX_TOOLWINDOW |
            NativeMethods.WS_EX_NOACTIVATE);

        ApplyScreenBounds();
    }

    public void ApplyScreenBounds()
    {
        if (_hwnd == IntPtr.Zero) return;

        // Use physical bounds directly with Win32 SetWindowPos to guarantee 100% monitor coverage
        // regardless of mixed DPI settings between displays.
        var bounds = TargetScreen.Bounds;
        NativeMethods.SetWindowPos(
            _hwnd,
            NativeMethods.HWND_TOPMOST,
            bounds.Left,
            bounds.Top,
            bounds.Width,
            bounds.Height,
            NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_SHOWWINDOW);
    }

    public void EnsureTopmost()
    {
        if (_hwnd != IntPtr.Zero)
        {
            NativeMethods.SetWindowPos(
                _hwnd,
                NativeMethods.HWND_TOPMOST,
                0, 0, 0, 0,
                NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE);
        }
    }

    public void SetDimming(double opacity, MediaColor tintColor)
    {
        // Clamp opacity safely between 0.0 and 0.95
        opacity = Math.Clamp(opacity, 0.0, 0.95);
        OverlayCanvas.Background = new SolidColorBrush(tintColor);
        OverlayCanvas.Opacity = opacity;
    }
}
