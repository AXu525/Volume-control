using System.Windows;

namespace AudioVolumeControl.App
{
    /// <summary>
    /// 应用程序入口
    /// </summary>
    public partial class App : Application
    {
        private TrayIcon? _trayIcon;

        /// <summary>
        /// 应用程序启动
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 创建托盘图标
            _trayIcon = new TrayIcon();

            // 禁止多实例运行
            if (e.Args.Contains("--tray"))
            {
                // 以托盘模式启动，不显示主窗口
                return;
            }
        }

        /// <summary>
        /// 应用程序退出
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            _trayIcon?.Dispose();
            base.OnExit(e);
        }
    }
}
