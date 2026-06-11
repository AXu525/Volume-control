using System.IO;
using System.Text.Json;

namespace AudioVolumeControl.App
{
    /// <summary>
    /// 应用程序设置
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// 主窗口位置 X
        /// </summary>
        public int WindowX { get; set; } = 100;

        /// <summary>
        /// 主窗口位置 Y
        /// </summary>
        public int WindowY { get; set; } = 100;

        /// <summary>
        /// 主窗口宽度
        /// </summary>
        public int WindowWidth { get; set; } = 400;

        /// <summary>
        /// 主窗口高度
        /// </summary>
        public int WindowHeight { get; set; } = 300;

        /// <summary>
        /// 音量步进值
        /// </summary>
        public int VolumeStep { get; set; } = 1;

        /// <summary>
        /// 是否最小化到托盘
        /// </summary>
        public bool MinimizeToTray { get; set; } = true;

        /// <summary>
        /// 上次选择的控制目标
        /// </summary>
        public string LastTarget { get; set; } = "系统主音量";
    }

    /// <summary>
    /// 设置管理器
    /// </summary>
    public class SettingsManager
    {
        private static readonly string SettingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AudioVolumeControl");

        private static readonly string SettingsFile = Path.Combine(SettingsDirectory, "settings.json");

        private static AppSettings? _currentSettings;

        /// <summary>
        /// 获取当前设置
        /// </summary>
        public static AppSettings Current => _currentSettings ??= Load();

        /// <summary>
        /// 加载设置
        /// </summary>
        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    string json = File.ReadAllText(SettingsFile);
                    _currentSettings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
                else
                {
                    _currentSettings = new AppSettings();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载设置失败: {ex.Message}");
                _currentSettings = new AppSettings();
            }

            return _currentSettings;
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        public static void Save()
        {
            try
            {
                // 确保目录存在
                if (!Directory.Exists(SettingsDirectory))
                {
                    Directory.CreateDirectory(SettingsDirectory);
                }

                string json = JsonSerializer.Serialize(_currentSettings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(SettingsFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存设置失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 删除设置文件
        /// </summary>
        public static void Delete()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    File.Delete(SettingsFile);
                }

                if (Directory.Exists(SettingsDirectory) && !Directory.EnumerateFiles(SettingsDirectory).Any())
                {
                    Directory.Delete(SettingsDirectory);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"删除设置失败: {ex.Message}");
            }
        }
    }
}
