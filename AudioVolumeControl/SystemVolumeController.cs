using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace AudioVolumeControl
{
    /// <summary>
    /// 系统音量控制器
    /// 用于控制系统主音量（0-100）
    /// </summary>
    public class SystemVolumeController : IDisposable
    {
        private readonly MMDeviceEnumerator _deviceEnumerator;
        private MMDevice? _device;
        private bool _disposed;

        /// <summary>
        /// 初始化系统音量控制器
        /// </summary>
        public SystemVolumeController()
        {
            _deviceEnumerator = new MMDeviceEnumerator();
            InitializeDevice();
        }

        /// <summary>
        /// 初始化音频设备
        /// </summary>
        private void InitializeDevice()
        {
            try
            {
                _device = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("无法获取默认音频设备，请检查音频设备是否正常连接。", ex);
            }
        }

        /// <summary>
        /// 获取系统主音量（0-100）
        /// </summary>
        /// <returns>音量值（0-100）</returns>
        public int GetVolume()
        {
            EnsureDeviceValid();
            // 将 0.0-1.0 的音量值转换为 0-100
            return (int)Math.Round(_device!.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
        }

        /// <summary>
        /// 设置系统主音量（0-100）
        /// </summary>
        /// <param name="volume">音量值（0-100）</param>
        /// <exception cref="ArgumentOutOfRangeException">音量值超出范围</exception>
        public void SetVolume(int volume)
        {
            if (volume < 0 || volume > 100)
                throw new ArgumentOutOfRangeException(nameof(volume), "音量值必须在 0-100 之间");

            EnsureDeviceValid();
            // 将 0-100 转换为 0.0-1.0
            _device!.AudioEndpointVolume.MasterVolumeLevelScalar = volume / 100f;
        }

        /// <summary>
        /// 获取静音状态
        /// </summary>
        /// <returns>是否静音</returns>
        public bool IsMuted()
        {
            EnsureDeviceValid();
            return _device!.AudioEndpointVolume.Mute;
        }

        /// <summary>
        /// 设置静音状态
        /// </summary>
        /// <param name="muted">是否静音</param>
        public void SetMuted(bool muted)
        {
            EnsureDeviceValid();
            _device!.AudioEndpointVolume.Mute = muted;
        }

        /// <summary>
        /// 切换静音状态
        /// </summary>
        public void ToggleMute()
        {
            SetMuted(!IsMuted());
        }

        /// <summary>
        /// 确保音频设备有效
        /// </summary>
        private void EnsureDeviceValid()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SystemVolumeController));

            if (_device == null)
                InitializeDevice();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _device?.Dispose();
                _deviceEnumerator?.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
