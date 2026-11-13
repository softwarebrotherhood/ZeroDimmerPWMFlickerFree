using System;
using System.Windows.Interop;

namespace AntiFlickerDimmer;

public class HotkeyManager : IDisposable
{
    public const int HOTKEY_DIM_DOWN = 9001; // Ctrl + Alt + Down (More dark)
    public const int HOTKEY_DIM_UP   = 9002; // Ctrl + Alt + Up   (Less dark)
    public const int HOTKEY_TOGGLE   = 9003; // Ctrl + Alt + D    (Toggle On/Off)
    public const int HOTKEY_RESET    = 9004; // Ctrl + Alt + Esc  (Safety emergency reset)

    private const uint VK_DOWN   = 0x28;
    private const uint VK_UP     = 0x26;
    private const uint VK_D      = 0x44;
    private const uint VK_ESCAPE = 0x1B;

    private HwndSource? _source;
    private IntPtr _hwnd = IntPtr.Zero;

    public event Action? OnDimDownRequested;
    public event Action? OnDimUpRequested;
    public event Action? OnToggleRequested;
    public event Action? OnResetRequested;

    public void Initialize(IntPtr hwnd)
    {
        _hwnd = hwnd;
        _source = HwndSource.FromHwnd(_hwnd);
        _source?.AddHook(HwndHook);

        RegisterAll();
    }

    private void RegisterAll()
    {
        if (_hwnd == IntPtr.Zero) return;

        uint mods = NativeMethods.MOD_CONTROL | NativeMethods.MOD_ALT | NativeMethods.MOD_NOREPEAT;

        NativeMethods.RegisterHotKey(_hwnd, HOTKEY_DIM_DOWN, mods, VK_DOWN);
        NativeMethods.RegisterHotKey(_hwnd, HOTKEY_DIM_UP, mods, VK_UP);
        NativeMethods.RegisterHotKey(_hwnd, HOTKEY_TOGGLE, mods, VK_D);
        NativeMethods.RegisterHotKey(_hwnd, HOTKEY_RESET, mods, VK_ESCAPE);
    }

    private void UnregisterAll()
    {
        if (_hwnd == IntPtr.Zero) return;

        NativeMethods.UnregisterHotKey(_hwnd, HOTKEY_DIM_DOWN);
        NativeMethods.UnregisterHotKey(_hwnd, HOTKEY_DIM_UP);
        NativeMethods.UnregisterHotKey(_hwnd, HOTKEY_TOGGLE);
        NativeMethods.UnregisterHotKey(_hwnd, HOTKEY_RESET);
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == NativeMethods.WM_HOTKEY)
        {
            int id = wParam.ToInt32();
            switch (id)
            {
                case HOTKEY_DIM_DOWN:
                    OnDimDownRequested?.Invoke();
                    handled = true;
                    break;
                case HOTKEY_DIM_UP:
                    OnDimUpRequested?.Invoke();
                    handled = true;
                    break;
                case HOTKEY_TOGGLE:
                    OnToggleRequested?.Invoke();
                    handled = true;
                    break;
                case HOTKEY_RESET:
                    OnResetRequested?.Invoke();
                    handled = true;
                    break;
            }
        }

        return IntPtr.Zero;
    }

    public void Dispose()
    {
        UnregisterAll();
        if (_source != null)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
        }
    }
}
