using AudioVolumeControl;
using System.Windows;
using System.Windows.Input;

namespace AudioVolumeControl.App
{
    /// <summary>
    /// 主窗口交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SystemVolumeController _systemVolumeController;
        private readonly AudioSessionManager _audioSessionManager;
        private bool _isUpdatingUI;

        /// <summary>
        /// 当前控制模式
        /// </summary>
        private enum ControlMode
        {
            SystemVolume,
            ApplicationVolume
        }

        private ControlMode _currentMode = ControlMode.SystemVolume;
        private int _selectedProcessId = -1;

        /// <summary>
        /// 初始化主窗口
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _systemVolumeController = new SystemVolumeController();
            _audioSessionManager = new AudioSessionManager();

            // 初始化控制目标下拉框
            InitializeTargetComboBox();
        }

        /// <summary>
        /// 初始化控制目标下拉框
        /// </summary>
        private void InitializeTargetComboBox()
        {
            TargetComboBox.Items.Clear();
            TargetComboBox.Items.Add("系统主音量");

            try
            {
                var sessions = _audioSessionManager.GetAllSessions();
                foreach (var session in sessions)
                {
                    TargetComboBox.Items.Add(session.DisplayName);
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"加载音频会话失败: {ex.Message}";
            }

            TargetComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// 窗口加载完成
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateVolumeDisplay();
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 隐藏到托盘而不是关闭
            e.Cancel = true;
            Hide();
        }

        /// <summary>
        /// 更新音量显示
        /// </summary>
        private void UpdateVolumeDisplay()
        {
            if (_isUpdatingUI) return;

            try
            {
                int volume = GetCurrentVolume();
                _isUpdatingUI = true;

                VolumeDisplay.Text = $"音量: {volume}";
                VolumeSlider.Value = volume;
                VolumeInput.Text = volume.ToString();
                MuteButton.Content = IsCurrentMuted() ? "取消静音" : "静音";
                StatusText.Text = "就绪";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"获取音量失败: {ex.Message}";
            }
            finally
            {
                _isUpdatingUI = false;
            }
        }

        /// <summary>
        /// 获取当前音量
        /// </summary>
        private int GetCurrentVolume()
        {
            if (_currentMode == ControlMode.SystemVolume)
            {
                return _systemVolumeController.GetVolume();
            }
            else
            {
                return _audioSessionManager.GetSessionVolume(_selectedProcessId);
            }
        }

        /// <summary>
        /// 设置当前音量
        /// </summary>
        private void SetCurrentVolume(int volume)
        {
            if (_currentMode == ControlMode.SystemVolume)
            {
                _systemVolumeController.SetVolume(volume);
            }
            else
            {
                _audioSessionManager.SetSessionVolume(_selectedProcessId, volume);
            }
        }

        /// <summary>
        /// 获取当前是否静音
        /// </summary>
        private bool IsCurrentMuted()
        {
            if (_currentMode == ControlMode.SystemVolume)
            {
                return _systemVolumeController.IsMuted();
            }
            else
            {
                var sessions = _audioSessionManager.GetAllSessions();
                var session = sessions.FirstOrDefault(s => s.ProcessId == _selectedProcessId);
                return session?.IsMuted ?? false;
            }
        }

        /// <summary>
        /// 设置当前静音状态
        /// </summary>
        private void SetCurrentMuted(bool muted)
        {
            if (_currentMode == ControlMode.SystemVolume)
            {
                _systemVolumeController.SetMuted(muted);
            }
            else
            {
                _audioSessionManager.SetSessionMuted(_selectedProcessId, muted);
            }
        }

        /// <summary>
        /// 控制目标选择改变
        /// </summary>
        private void TargetComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TargetComboBox.SelectedIndex == 0)
            {
                // 系统主音量
                _currentMode = ControlMode.SystemVolume;
                _selectedProcessId = -1;
            }
            else
            {
                // 应用音量
                _currentMode = ControlMode.ApplicationVolume;
                try
                {
                    var sessions = _audioSessionManager.GetAllSessions();
                    var selectedSession = sessions.FirstOrDefault(s => s.DisplayName == TargetComboBox.SelectedItem.ToString());
                    if (selectedSession != null)
                    {
                        _selectedProcessId = selectedSession.ProcessId;
                    }
                }
                catch (Exception ex)
                {
                    StatusText.Text = $"选择音频会话失败: {ex.Message}";
                }
            }

            UpdateVolumeDisplay();
        }

        /// <summary>
        /// 音量输入框回车
        /// </summary>
        private void VolumeInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ApplyVolumeFromInput();
            }
        }

        /// <summary>
        /// 音量输入框失去焦点
        /// </summary>
        private void VolumeInput_LostFocus(object sender, RoutedEventArgs e)
        {
            ApplyVolumeFromInput();
        }

        /// <summary>
        /// 从输入框应用音量
        /// </summary>
        private void ApplyVolumeFromInput()
        {
            if (int.TryParse(VolumeInput.Text, out int volume))
            {
                volume = VolumeCalculator.ClampVolume(volume);
                try
                {
                    SetCurrentVolume(volume);
                    UpdateVolumeDisplay();
                }
                catch (Exception ex)
                {
                    StatusText.Text = $"设置音量失败: {ex.Message}";
                }
            }
            else
            {
                StatusText.Text = "请输入有效的音量值（0-100）";
                UpdateVolumeDisplay();
            }
        }

        /// <summary>
        /// 音量滑块值改变
        /// </summary>
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUpdatingUI) return;
            if (_systemVolumeController == null) return; // 窗口初始化期间跳过

            try
            {
                int volume = (int)VolumeSlider.Value;
                SetCurrentVolume(volume);
                UpdateVolumeDisplay();
            }
            catch (Exception ex)
            {
                if (StatusText != null)
                    StatusText.Text = $"设置音量失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 应用按钮点击
        /// </summary>
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyVolumeFromInput();
        }

        /// <summary>
        /// 静音按钮点击
        /// </summary>
        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isMuted = IsCurrentMuted();
                SetCurrentMuted(!isMuted);
                UpdateVolumeDisplay();
            }
            catch (Exception ex)
            {
                StatusText.Text = $"切换静音失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 设置按钮点击
        /// </summary>
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("设置功能开发中...", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 最小化按钮点击
        /// </summary>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
