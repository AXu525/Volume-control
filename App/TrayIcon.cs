using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace AudioVolumeControl.App
{
    /// <summary>
    /// 托盘图标管理器
    /// </summary>
    public class TrayIcon : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly System.Windows.Forms.Timer _updateTimer;
        private bool _disposed;

        /// <summary>
        /// 初始化托盘图标
        /// </summary>
        public TrayIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Text = "音量控制",
                Visible = true,
                Icon = SystemIcons.Application
            };

            // 创建右键菜单
            var contextMenu = new ContextMenuStrip();

            // 显示主窗口
            var showMenuItem = new ToolStripMenuItem("显示主窗口");
            showMenuItem.Click += ShowMenuItem_Click;
            contextMenu.Items.Add(showMenuItem);

            // 快捷音量调节
            contextMenu.Items.Add(new ToolStripSeparator());

            // 静音切换
            var muteMenuItem = new ToolStripMenuItem("静音");
            muteMenuItem.Click += MuteMenuItem_Click;
            contextMenu.Items.Add(muteMenuItem);

            // 退出
            contextMenu.Items.Add(new ToolStripSeparator());

            var exitMenuItem = new ToolStripMenuItem("退出");
            exitMenuItem.Click += ExitMenuItem_Click;
            contextMenu.Items.Add(exitMenuItem);

            _notifyIcon.ContextMenuStrip = contextMenu;

            // 左键点击显示主窗口
            _notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;

            // 定时更新托盘图标显示
            _updateTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000 // 每秒更新一次
            };
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        /// <summary>
        /// 显示主窗口菜单点击
        /// </summary>
        private void ShowMenuItem_Click(object? sender, EventArgs e)
        {
            ShowMainWindow();
        }

        /// <summary>
        /// 静音菜单点击
        /// </summary>
        private void MuteMenuItem_Click(object? sender, EventArgs e)
        {
            try
            {
                using var controller = new SystemVolumeController();
                controller.ToggleMute();
                UpdateTrayIcon();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 退出菜单点击
        /// </summary>
        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// 托盘图标双击
        /// </summary>
        private void NotifyIcon_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowMainWindow();
            }
        }

        /// <summary>
        /// 显示主窗口
        /// </summary>
        private void ShowMainWindow()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Activate();
            }
        }

        /// <summary>
        /// 定时更新托盘图标
        /// </summary>
        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            UpdateTrayIcon();
        }

        /// <summary>
        /// 更新托盘图标显示
        /// </summary>
        private void UpdateTrayIcon()
        {
            try
            {
                using var controller = new SystemVolumeController();
                int volume = controller.GetVolume();
                bool isMuted = controller.IsMuted();

                // 更新提示文本
                _notifyIcon.Text = isMuted ? "音量控制 - 已静音" : $"音量控制 - {volume}%";

                // 可以根据音量值显示不同的图标
                // 这里使用系统图标，实际项目中可以自定义图标
            }
            catch
            {
                // 忽略更新错误
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _updateTimer?.Stop();
                _updateTimer?.Dispose();
                _notifyIcon?.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
