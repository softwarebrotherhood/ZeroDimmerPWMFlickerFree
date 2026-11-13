using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Forms;

namespace AntiFlickerDimmer;

public partial class OsdWindow : Window
{
    private readonly DispatcherTimer _hideTimer;
    private IntPtr _hwnd = IntPtr.Zero;

    public OsdWindow()
    {
        InitializeComponent();

        _hideTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1300)
        };
        _hideTimer.Tick += (s, e) =>
        {
            _hideTimer.Stop();
            FadeOut();
        };
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        _hwnd = new WindowInteropHelper(this).Handle;

        int exStyle = NativeMethods.GetWindowLong(_hwnd, NativeMethods.GWL_EXSTYLE);
        NativeMethods.SetWindowLong(_hwnd, NativeMethods.GWL_EXSTYLE,
            exStyle |
            NativeMethods.WS_EX_TRANSPARENT |
            NativeMethods.WS_EX_LAYERED |
            NativeMethods.WS_EX_TOOLWINDOW |
            NativeMethods.WS_EX_NOACTIVATE);
    }

    public void ShowFeedback(int level, bool isEnabled)
    {
        _hideTimer.Stop();

        if (!isEnabled || level == 0)
        {
            PercentText.Text = "OFF";
            PercentText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(248, 113, 113)); // Soft red
            IconText.Text = "☀️";
            DimmerProgress.Value = 0;
        }
        else
        {
            PercentText.Text = $"{level}% Dark";
            PercentText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(56, 189, 248)); // Sky blue
            IconText.Text = "🌙";
            DimmerProgress.Value = level;
        }

        // Position at top center of primary monitor
        var primary = Screen.PrimaryScreen?.Bounds ?? new System.Drawing.Rectangle(0, 0, 1920, 1080);
        Left = (primary.Left + (primary.Width - Width) / 2);
        Top = primary.Top + 60;

        Opacity = 1.0;
        Show();

        if (_hwnd != IntPtr.Zero)
        {
            NativeMethods.SetWindowPos(
                _hwnd,
                NativeMethods.HWND_TOPMOST,
                (int)Left, (int)Top, (int)Width, (int)Height,
                NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_SHOWWINDOW);
        }

        _hideTimer.Start();
    }

    private void FadeOut()
    {
        var anim = new DoubleAnimation(1.0, 0.0, TimeSpan.FromMilliseconds(250));
        anim.Completed += (s, e) =>
        {
            Hide();
        };
        BeginAnimation(OpacityProperty, anim);
    }
}
