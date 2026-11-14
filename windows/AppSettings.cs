using System;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;

namespace AntiFlickerDimmer;

public class AppSettings
{
    private static readonly string AppDataFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "AntiFlickerDimmer");
    private static readonly string ConfigFilePath = Path.Combine(AppDataFolder, "settings.json");
    private const string StartupRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppRegistryName = "AntiFlickerDimmer";

    public int DimLevel { get; set; } = 35; // 0 - 90 %
    public bool IsEnabled { get; set; } = true;
    public string TintMode { get; set; } = "NeutralBlack";
    public string DimmerMethod { get; set; } = "Dwm"; // "Dwm" (Deep Desktop & Taskbar) or "Overlay"
    public string CustomHexColor { get; set; } = "#000000";
    public bool StartWithWindows { get; set; } = false;
    public bool ShowOsd { get; set; } = true;
    public int MaxDimLevel { get; set; } = 90;

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(ConfigFilePath))
            {
                var json = File.ReadAllText(ConfigFilePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                if (settings != null)
                {
                    // Clamp values to safe boundaries
                    settings.DimLevel = Math.Clamp(settings.DimLevel, 0, settings.MaxDimLevel);
                    settings.MaxDimLevel = Math.Clamp(settings.MaxDimLevel, 50, 95);
                    return settings;
                }
            }
        }
        catch (Exception)
        {
            // Fallback to defaults on error
        }

        return new AppSettings();
    }

    public void Save()
    {
        try
        {
            if (!Directory.Exists(AppDataFolder))
            {
                Directory.CreateDirectory(AppDataFolder);
            }

            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFilePath, json);

            UpdateStartupRegistry();
        }
        catch (Exception)
        {
            // Ignore write failures gracefully
        }
    }

    public void UpdateStartupRegistry()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, true);
            if (key == null) return;

            string? exePath = Environment.ProcessPath;
            if (string.IsNullOrEmpty(exePath))
            {
                exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
            }

            if (StartWithWindows && !string.IsNullOrEmpty(exePath))
            {
                key.SetValue(AppRegistryName, $"\"{exePath}\"");
            }
            else
            {
                if (key.GetValue(AppRegistryName) != null)
                {
                    key.DeleteValue(AppRegistryName, false);
                }
            }
        }
        catch (Exception)
        {
            // May fail due to restricted permissions in some environments
        }
    }
}
