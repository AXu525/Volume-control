using NAudio.CoreAudioApi;
using System.Diagnostics;

namespace AudioVolumeControl
{
    /// <summary>
    /// 音频会话信息
    /// </summary>
    public class AudioSessionInfo
    {
        /// <summary>
        /// 会话 ID
        /// </summary>
        public string SessionId { get; set; } = string.Empty;

        /// <summary>
        /// 进程 ID
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// 进程名称
        /// </summary>
        public string ProcessName { get; set; } = string.Empty;

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 音量（0-100）
        /// </summary>
        public int Volume { get; set; }

        /// <summary>
        /// 是否静音
        /// </summary>
        public bool IsMuted { get; set; }

        /// <summary>
        /// 是否正在播放音频
        /// </summary>
        public bool IsPlaying { get; set; }

        /// <summary>
        /// 图标路径
        /// </summary>
        public string IconPath { get; set; } = string.Empty;
    }

    /// <summary>
    /// 音频会话管理器
    /// 用于管理单个应用的音量控制
    /// </summary>
    public class AudioSessionManager : IDisposable
    {
        private readonly MMDeviceEnumerator _deviceEnumerator;
        private MMDevice? _device;
        private bool _disposed;

        /// <summary>
        /// 初始化音频会话管理器
        /// </summary>
        public AudioSessionManager()
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
        /// 获取所有音频会话
        /// </summary>
        /// <returns>音频会话列表</returns>
        public List<AudioSessionInfo> GetAllSessions()
        {
            EnsureDeviceValid();
            var sessions = new List<AudioSessionInfo>();

            try
            {
                var sessionEnumerator = _device!.AudioSessionManager.Sessions;

                for (int i = 0; i < sessionEnumerator.Count; i++)
                {
                    var session = sessionEnumerator[i];
                    var simpleAudioVolume = session.SimpleAudioVolume;

                    var sessionInfo = new AudioSessionInfo
                    {
                        SessionId = session.DisplayName,
                        ProcessId = (int)session.GetProcessID,
                        Volume = (int)Math.Round(simpleAudioVolume.Volume * 100),
                        IsMuted = simpleAudioVolume.Mute,
                        IconPath = session.IconPath,
                        IsPlaying = simpleAudioVolume.Volume > 0 && !simpleAudioVolume.Mute
                    };

                    // 获取进程名称
                    try
                    {
                        var process = Process.GetProcessById(sessionInfo.ProcessId);
                        sessionInfo.ProcessName = process.ProcessName;

                        // 过滤系统进程（不在用户目录下的进程）
                        if (IsSystemProcess(process))
                            continue;

                        sessionInfo.DisplayName = string.IsNullOrEmpty(session.DisplayName)
                            ? sessionInfo.ProcessName
                            : session.DisplayName;
                    }
                    catch
                    {
                        // 无法获取进程信息的会话也跳过
                        continue;
                    }

                    sessions.Add(sessionInfo);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("获取音频会话列表失败。", ex);
            }

            return sessions;
        }

        /// <summary>
        /// 获取当前正在播放音频的会话
        /// </summary>
        /// <returns>正在播放的音频会话列表</returns>
        public List<AudioSessionInfo> GetCurrentPlayingSessions()
        {
            return GetAllSessions().Where(s => s.IsPlaying || s.Volume > 0).ToList();
        }

        /// <summary>
        /// 获取指定会话的音量
        /// </summary>
        /// <param name="processId">进程 ID</param>
        /// <returns>音量值（0-100）</returns>
        public int GetSessionVolume(int processId)
        {
            var session = FindSessionByProcessId(processId);
            if (session == null)
                throw new ArgumentException($"未找到进程 ID 为 {processId} 的音频会话");

            return session.Volume;
        }

        /// <summary>
        /// 设置指定会话的音量
        /// </summary>
        /// <param name="processId">进程 ID</param>
        /// <param name="volume">音量值（0-100）</param>
        public void SetSessionVolume(int processId, int volume)
        {
            if (volume < 0 || volume > 100)
                throw new ArgumentOutOfRangeException(nameof(volume), "音量值必须在 0-100 之间");

            EnsureDeviceValid();

            try
            {
                var sessionEnumerator = _device!.AudioSessionManager.Sessions;

                for (int i = 0; i < sessionEnumerator.Count; i++)
                {
                    var session = sessionEnumerator[i];
                    if ((int)session.GetProcessID == processId)
                    {
                        session.SimpleAudioVolume.Volume = volume / 100f;
                        return;
                    }
                }

                throw new ArgumentException($"未找到进程 ID 为 {processId} 的音频会话");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"设置进程 {processId} 的音量失败。", ex);
            }
        }

        /// <summary>
        /// 设置指定会话的静音状态
        /// </summary>
        /// <param name="processId">进程 ID</param>
        /// <param name="muted">是否静音</param>
        public void SetSessionMuted(int processId, bool muted)
        {
            EnsureDeviceValid();

            try
            {
                var sessionEnumerator = _device!.AudioSessionManager.Sessions;

                for (int i = 0; i < sessionEnumerator.Count; i++)
                {
                    var session = sessionEnumerator[i];
                    if ((int)session.GetProcessID == processId)
                    {
                        session.SimpleAudioVolume.Mute = muted;
                        return;
                    }
                }

                throw new ArgumentException($"未找到进程 ID 为 {processId} 的音频会话");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"设置进程 {processId} 的静音状态失败。", ex);
            }
        }

        /// <summary>
        /// 根据进程 ID 查找会话
        /// </summary>
        /// <param name="processId">进程 ID</param>
        /// <returns>音频会话信息，如果未找到返回 null</returns>
        private AudioSessionInfo? FindSessionByProcessId(int processId)
        {
            return GetAllSessions().FirstOrDefault(s => s.ProcessId == processId);
        }

        /// <summary>
        /// 判断是否是系统进程
        /// </summary>
        /// <param name="process">进程对象</param>
        /// <returns>是否是系统进程</returns>
        private bool IsSystemProcess(Process process)
        {
            try
            {
                string processPath = process.MainModule?.FileName ?? string.Empty;
                string systemRoot = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

                // 如果进程路径在 System32 或 SysWOW64 目录下，视为系统进程
                if (processPath.StartsWith(systemRoot, StringComparison.OrdinalIgnoreCase))
                {
                    string lowerPath = processPath.ToLower();
                    if (lowerPath.Contains("\\system32\\") || lowerPath.Contains("\\syswow64\\"))
                        return true;
                }

                // 常见系统进程名列表
                string[] systemProcessNames = new[]
                {
                    "svchost", "audiosrv", "audioendpointbuilder",
                    "runtimebroker", "shellhost", "dcomlaunch",
                    "services", "lsass", "csrss", "wininit", "winlogon"
                };

                string processName = process.ProcessName.ToLower();
                if (systemProcessNames.Any(name => processName.Contains(name)))
                    return true;

                return false;
            }
            catch
            {
                // 无法获取进程信息时，保守起见不显示
                return true;
            }
        }

        /// <summary>
        /// 确保音频设备有效
        /// </summary>
        private void EnsureDeviceValid()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AudioSessionManager));

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
