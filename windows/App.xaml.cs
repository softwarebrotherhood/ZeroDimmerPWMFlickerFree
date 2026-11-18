using System;
using System.IO;
using System.Windows;

namespace AntiFlickerDimmer;

public partial class App : System.Windows.Application
{
    private ScreenOverlayManager? _overlayManager;
    private TrayIconManager? _trayManager;
    private HotkeyManager? _hotkeyManager;
    private OsdWindow? _osdWindow;
    private ControlWindow? _controlWindow;

    private static readonly string LogFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ZeroDimmer", "debug.log");

    public static void Log(string message)
    {
        try
        {
            var dir = Path.GetDirectoryName(LogFile);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.AppendAllText(LogFile, $"[{DateTime.Now:HH:mm:ss.fff}] {message}{Environment.NewLine}");
        }
        catch { }
    }

    private static void EnsureSingleInstance()
    {
        try
        {
            var current = System.Diagnostics.Process.GetCurrentProcess();
            foreach (var p in System.Diagnostics.Process.GetProcessesByName(current.ProcessName))
            {
                if (p.Id != current.Id)
                {
                    Log($"Closing prior instance PID {p.Id} to ensure fresh launch...");
                    try
                    {
                        p.Kill();
                        p.WaitForExit(1000);
                    }
                    catch { }
                }
            }
        }
        catch (Exception ex)
        {
            Log($"EnsureSingleInstance note: {ex.Message}");
        }
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        Log("=== OnStartup Begin ===");

        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            Log($"FATAL AppDomain UnhandledException: {ex}");
            System.Windows.Forms.MessageBox.Show(
                $"Unexpected Error:\n{ex?.Message}\n\n{ex?.StackTrace}",
                "ZeroDimmer - Error",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error);
        };

        DispatcherUnhandledException += (s, args) =>
        {
            Log($"UI DispatcherUnhandledException: {args.Exception}");
            System.Windows.Forms.MessageBox.Show(
                $"UI Error:\n{args.Exception.Message}\n\n{args.Exception.StackTrace}",
                "ZeroDimmer - Error",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error);
            args.Handled = true;
        };

        EnsureSingleInstance();

        base.OnStartup(e);

        Log("Loading AppSettings...");
        var settings = AppSettings.Load();
        Log($"Settings loaded. DimLevel={settings.DimLevel}, Enabled={settings.IsEnabled}");

        Log("Creating ScreenOverlayManager...");
        _overlayManager = new ScreenOverlayManager();
        Log("ScreenOverlayManager created.");

        Log("Creating TrayIconManager...");
        _trayManager = new TrayIconManager();
        Log("TrayIconManager created.");

        Log("Creating OsdWindow...");
        _osdWindow = new OsdWindow();
        Log("OsdWindow created.");

        Log("Creating HotkeyManager...");
        _hotkeyManager = new HotkeyManager();
        Log("HotkeyManager created.");

        Log("Creating ControlWindow...");
        _controlWindow = new ControlWindow(
            settings,
            _overlayManager,
            _trayManager,
            _hotkeyManager,
            _osdWindow);
        Log("ControlWindow created.");

        MainWindow = _controlWindow;

        try
        {
            Log("Showing ControlWindow...");
            _controlWindow.ShowAndActivate();
            Log("ControlWindow ShowAndActivate completed.");
        }
        catch (Exception ex)
        {
            Log($"EXCEPTION in ShowControlWindow: {ex}");
        }

        try
        {
            Log("Showing tray notification...");
            _trayManager.ShowNotification(
                "ZeroDimmer Active",
                "Use Ctrl+Alt+Down to dim, Ctrl+Alt+Up to brighten, or click the tray icon.");
            Log("Tray notification shown.");
        }
        catch (Exception ex)
        {
            Log($"Notification error (ignored): {ex.Message}");
        }

        Log("=== OnStartup Completed Successfully ===");
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log($"OnExit called. ApplicationExitCode={e.ApplicationExitCode}");
        _hotkeyManager?.Dispose();
        _trayManager?.Dispose();
        _overlayManager?.Dispose();

        base.OnExit(e);
        Log("=== Application Terminated ===");
    }
}
